using System.Net.Http.Json;
using System.Text.Json;
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
        private readonly ILogger<CatalogoApiClient> _logger;
        private static readonly JsonSerializerOptions _opciones = new() { PropertyNameCaseInsensitive = true };

        public CatalogoApiClient(HttpClient http, ILogger<CatalogoApiClient> logger)
        {
            _http = http;
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
                // Catalog.API no está levantado: la vista no debe tronar, solo
                // se queda sin datos hasta que el microservicio esté arriba.
                _logger.LogWarning(ex, "No se pudo contactar a Catalog.API para obtener películas.");
                return new();
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
                _logger.LogWarning(ex, "No se pudo contactar a Catalog.API para obtener series.");
                return new();
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
