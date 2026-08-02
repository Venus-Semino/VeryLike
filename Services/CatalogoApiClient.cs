using System.Net.Http.Json;
using System.Text.Json;
using VeryLike.Domain.Interfaces;
using VeryLike.Domain.Models;

namespace VeryLike.Web.Services
{
    public interface ICatalogoApiClient
    {
        Task<List<Pelicula>> ObtenerPeliculasAsync();
        Task<List<Serie>> ObtenerSeriesAsync();
        Task<List<ContenidoAudiovisual>> ObtenerTodoAsync();
    }

    /// <summary>
    /// Cliente HTTP tipado hacia VeryLike.Catalog.API. VeryLike.Web ya no lee
    /// la base de datos de catálogo directamente: le pide los datos a este
    /// microservicio (ver Program.cs, AddHttpClient&lt;ICatalogoApiClient, ...&gt;,
    /// donde se configura BaseAddress a partir de ServiceUrlsOptions).
    /// </summary>
    public class CatalogoApiClient : ICatalogoApiClient
    {
        private readonly HttpClient _http;
        private readonly ICatalogoRepository _catalogoLocal;
        private readonly ILogger<CatalogoApiClient> _logger;
        private static readonly JsonSerializerOptions _opciones = new() { PropertyNameCaseInsensitive = true };

        public CatalogoApiClient(HttpClient http, ICatalogoRepository catalogoLocal, ILogger<CatalogoApiClient> logger)
        {
            _http = http;
            _catalogoLocal = catalogoLocal;
            _logger = logger;
        }

        public async Task<List<Pelicula>> ObtenerPeliculasAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<List<Pelicula>>("api/catalogo/peliculas", _opciones) ?? new();
            }
            catch (HttpRequestException ex)
            {
                // Catalog.API todavía no está levantado: se sirve el catálogo
                // desde la base compartida para no dejar la vista vacía.
                _logger.LogWarning(ex, "No se pudo contactar a Catalog.API; se usa el catálogo local.");
                return await _catalogoLocal.ObtenerPeliculasAsync();
            }
        }

        public async Task<List<Serie>> ObtenerSeriesAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<List<Serie>>("api/catalogo/series", _opciones) ?? new();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, "No se pudo contactar a Catalog.API; se usan las series locales.");
                return await _catalogoLocal.ObtenerSeriesAsync();
            }
        }

        public async Task<List<ContenidoAudiovisual>> ObtenerTodoAsync()
        {
            var peliculas = await ObtenerPeliculasAsync();
            var series = await ObtenerSeriesAsync();
            return peliculas.Cast<ContenidoAudiovisual>().Concat(series).ToList();
        }
    }
}
