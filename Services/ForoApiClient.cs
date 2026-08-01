using System.Net.Http.Json;
using System.Text.Json;
using VeryLike.Domain.Models;

namespace VeryLike.Web.Services
{
    public interface IForoApiClient
    {
        Task<List<MensajeForo>> ObtenerTodosAsync();
        Task<bool> PublicarAsync(MensajeForo nuevoMensaje);
    }

    /// <summary>
    /// Cliente HTTP tipado hacia VeryLike.Forum.API. Espejo de
    /// CatalogoApiClient: VeryLike.Web no toca la tabla MensajesForo
    /// directamente, le pide los datos a este microservicio.
    /// </summary>
    public class ForoApiClient : IForoApiClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<ForoApiClient> _logger;
        private static readonly JsonSerializerOptions _opciones = new() { PropertyNameCaseInsensitive = true };

        public ForoApiClient(HttpClient http, ILogger<ForoApiClient> logger)
        {
            _http = http;
            _logger = logger;
        }

        public async Task<List<MensajeForo>> ObtenerTodosAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<List<MensajeForo>>("api/foro", _opciones) ?? new();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, "No se pudo contactar a Forum.API para obtener los mensajes.");
                return new();
            }
        }

        public async Task<bool> PublicarAsync(MensajeForo nuevoMensaje)
        {
            try
            {
                var respuesta = await _http.PostAsJsonAsync("api/foro", nuevoMensaje);
                return respuesta.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, "No se pudo contactar a Forum.API para publicar el mensaje.");
                return false;
            }
        }
    }
}
