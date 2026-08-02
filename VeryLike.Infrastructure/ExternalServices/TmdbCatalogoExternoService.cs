using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VeryLike.Domain.DTOs;
using VeryLike.Domain.Interfaces;

namespace VeryLike.Infrastructure.ExternalServices
{
    /// <summary>
    /// Consume la API abierta de TMDB (The Movie Database) por HTTP para
    /// resolver automáticamente metadatos de películas y series: sinopsis,
    /// año, estudio (aproximado), calificación, géneros y, de forma crítica,
    /// la URL del póster. No requiere intervención humana: basta un título
    /// o un id.
    /// </summary>
    public class TmdbCatalogoExternoService : ICatalogoExternoService
    {
        private readonly HttpClient _http;
        private readonly TmdbOptions _opciones;
        private readonly IMemoryCache _cache;
        private readonly ILogger<TmdbCatalogoExternoService> _logger;

        public TmdbCatalogoExternoService(
            HttpClient http,
            IOptions<TmdbOptions> opciones,
            IMemoryCache cache,
            ILogger<TmdbCatalogoExternoService> logger)
        {
            _http = http;
            _opciones = opciones.Value;
            _cache = cache;
            _logger = logger;

            if (_http.BaseAddress is null)
            {
                _http.BaseAddress = new Uri(_opciones.BaseUrl);
            }
        }

        public async Task<ContenidoExternoDto?> BuscarPorTituloAsync(string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo)) return null;

            var pelicula = await BuscarAsync<TmdbPeliculaResultado>("search/movie", titulo);
            if (pelicula != null)
            {
                return await MapearPeliculaAsync(pelicula);
            }

            var serie = await BuscarAsync<TmdbSerieResultado>("search/tv", titulo);
            if (serie != null)
            {
                return await MapearSerieAsync(serie);
            }

            return null;
        }

        public async Task<ContenidoExternoDto?> BuscarPorIdExternoAsync(string idExterno, string tipo)
        {
            if (!int.TryParse(idExterno, out var id)) return null;

            try
            {
                if (string.Equals(tipo, "Serie", StringComparison.OrdinalIgnoreCase))
                {
                    var serie = await _http.GetFromJsonAsync<TmdbSerieResultado>(
                        $"tv/{id}?api_key={_opciones.ApiKey}&language=es-MX");
                    return serie is null ? null : await MapearSerieAsync(serie);
                }

                var pelicula = await _http.GetFromJsonAsync<TmdbPeliculaResultado>(
                    $"movie/{id}?api_key={_opciones.ApiKey}&language=es-MX");
                return pelicula is null ? null : await MapearPeliculaAsync(pelicula);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, "No se pudo resolver el id externo {IdExterno} ({Tipo}) en TMDB.", idExterno, tipo);
                return null;
            }
        }

        public async Task<List<ContenidoExternoDto>> BuscarPopularesAsync(int cantidad = 20)
        {
            var resultado = new List<ContenidoExternoDto>();

            try
            {
                var mitad = Math.Max(1, cantidad / 2);

                var peliculasPopulares = await _http.GetFromJsonAsync<TmdbBusquedaResponse<TmdbPeliculaResultado>>(
                    $"movie/popular?api_key={_opciones.ApiKey}&language=es-MX&page=1");
                var seriesPopulares = await _http.GetFromJsonAsync<TmdbBusquedaResponse<TmdbSerieResultado>>(
                    $"tv/popular?api_key={_opciones.ApiKey}&language=es-MX&page=1");

                foreach (var p in (peliculasPopulares?.Results ?? new()).Take(mitad))
                {
                    resultado.Add(await MapearPeliculaAsync(p));
                }

                foreach (var s in (seriesPopulares?.Results ?? new()).Take(cantidad - resultado.Count))
                {
                    resultado.Add(await MapearSerieAsync(s));
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, "No se pudo sincronizar el catálogo desde TMDB.");
            }

            return resultado;
        }

        private async Task<T?> BuscarAsync<T>(string endpoint, string titulo) where T : class
        {
            try
            {
                var query = Uri.EscapeDataString(titulo);
                var respuesta = await _http.GetFromJsonAsync<TmdbBusquedaResponse<T>>(
                    $"{endpoint}?api_key={_opciones.ApiKey}&language=es-MX&query={query}");

                return respuesta?.Results.FirstOrDefault();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, "Fallo al buscar '{Titulo}' en TMDB ({Endpoint}).", titulo, endpoint);
                return default;
            }
        }

        private async Task<ContenidoExternoDto> MapearPeliculaAsync(TmdbPeliculaResultado p)
        {
            var generos = await ResolverGenerosAsync(p.GenreIds, esSerie: false);
            var plataforma = await ResolverPlataformaStreamingAsync(p.Id, esSerie: false);

            return new ContenidoExternoDto
            {
                IdExterno = p.Id.ToString(),
                Tipo = "Pelicula",
                Nombre = p.Title,
                Genero = generos,
                AnioPublicacion = ExtraerAnio(p.ReleaseDate),
                PlataformaStreaming = plataforma,
                Sinopsis = p.Overview,
                Studio = "TMDB", // TMDB no expone "estudio" en la búsqueda simple sin una llamada extra a /credits
                Calificacion = Math.Round(p.VoteAverage / 2, 1), // normaliza de escala 0-10 a 0-5
                PosterUrl = ConstruirUrlPoster(p.PosterPath),
                Duracion = p.Runtime is > 0 ? $"{p.Runtime / 60}h {p.Runtime % 60}m" : null
            };
        }

        private async Task<ContenidoExternoDto> MapearSerieAsync(TmdbSerieResultado s)
        {
            var generos = await ResolverGenerosAsync(s.GenreIds, esSerie: true);
            var plataforma = await ResolverPlataformaStreamingAsync(s.Id, esSerie: true);

            return new ContenidoExternoDto
            {
                IdExterno = s.Id.ToString(),
                Tipo = "Serie",
                Nombre = s.Name,
                Genero = generos,
                AnioPublicacion = ExtraerAnio(s.FirstAirDate),
                PlataformaStreaming = plataforma,
                Sinopsis = s.Overview,
                Studio = "TMDB",
                Calificacion = Math.Round(s.VoteAverage / 2, 1),
                PosterUrl = ConstruirUrlPoster(s.PosterPath),
                Temporadas = s.NumberOfSeasons is > 0 ? s.NumberOfSeasons : 1
            };
        }

        private string? ConstruirUrlPoster(string? posterPath) =>
            string.IsNullOrWhiteSpace(posterPath) ? null : $"{_opciones.ImageBaseUrl}{posterPath}";

        private static int ExtraerAnio(string? fecha) =>
            !string.IsNullOrWhiteSpace(fecha) && DateTime.TryParse(fecha, out var dt) ? dt.Year : 0;

        /// <summary>Traduce genre_ids a nombres legibles, cacheando el catálogo de géneros de TMDB en memoria.</summary>
        private async Task<List<string>> ResolverGenerosAsync(List<int> generoIds, bool esSerie)
        {
            var mapa = await ObtenerMapaGenerosAsync(esSerie);
            return generoIds
                .Where(mapa.ContainsKey)
                .Select(id => mapa[id])
                .ToList();
        }

        private async Task<Dictionary<int, string>> ObtenerMapaGenerosAsync(bool esSerie)
        {
            var clave = esSerie ? "tmdb-generos-tv" : "tmdb-generos-movie";

            if (_cache.TryGetValue(clave, out Dictionary<int, string>? mapaCacheado) && mapaCacheado != null)
            {
                return mapaCacheado;
            }

            try
            {
                var endpoint = esSerie ? "genre/tv/list" : "genre/movie/list";
                var respuesta = await _http.GetFromJsonAsync<TmdbGeneroListaResponse>(
                    $"{endpoint}?api_key={_opciones.ApiKey}&language=es-MX");

                var mapa = respuesta?.Genres.ToDictionary(g => g.Id, g => g.Name) ?? new();
                _cache.Set(clave, mapa, TimeSpan.FromHours(6));
                return mapa;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, "No se pudo obtener el catálogo de géneros de TMDB.");
                return new();
            }
        }

        /// <summary>Resuelve la primera plataforma de streaming disponible por suscripción en la región configurada.</summary>
        private async Task<string> ResolverPlataformaStreamingAsync(int idExterno, bool esSerie)
        {
            try
            {
                var endpoint = esSerie ? $"tv/{idExterno}/watch/providers" : $"movie/{idExterno}/watch/providers";
                var respuesta = await _http.GetFromJsonAsync<TmdbWatchProvidersResponse>(
                    $"{endpoint}?api_key={_opciones.ApiKey}");

                if (respuesta?.Results != null &&
                    respuesta.Results.TryGetValue(_opciones.Region, out var region) &&
                    region.Flatrate is { Count: > 0 })
                {
                    return region.Flatrate[0].ProviderName;
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, "No se pudo resolver la plataforma de streaming para el id {Id} en TMDB.", idExterno);
            }

            return "No disponible en streaming";
        }
    }
}
