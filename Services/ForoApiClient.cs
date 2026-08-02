using System.Net.Http.Json;
using System.Text.Json;
using VeryLike.Domain.Interfaces;
using VeryLike.Domain.Models;

namespace VeryLike.Web.Services
{
    public interface IForoApiClient
    {
        Task<List<MensajeForo>> ObtenerTodosAsync();
        Task<List<MensajeForo>> ObtenerPorHashtagAsync(string hashtag);
        Task<bool> PublicarAsync(MensajeForo nuevoMensaje);
    }

    /// <summary>
    /// Cliente HTTP tipado hacia VeryLike.Forum.API. Espejo de
    /// CatalogoApiClient: mientras el microservicio no esté levantado, lee y
    /// escribe sobre la base compartida a través de IMensajeForoRepository.
    /// </summary>
    public class ForoApiClient : IForoApiClient
    {
        private readonly HttpClient _http;
        private readonly IMensajeForoRepository _foroLocal;
        private readonly ILogger<ForoApiClient> _logger;
        private static readonly JsonSerializerOptions _opciones = new() { PropertyNameCaseInsensitive = true };

        public ForoApiClient(HttpClient http, IMensajeForoRepository foroLocal, ILogger<ForoApiClient> logger)
        {
            _http = http;
            _foroLocal = foroLocal;
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
                _logger.LogWarning(ex, "No se pudo contactar a Forum.API; se usa el foro local.");
                return await _foroLocal.ObtenerTodosAsync();
            }
        }

        /// <summary>El filtrado por etiqueta todavía no existe en Forum.API: se resuelve en local.</summary>
        public Task<List<MensajeForo>> ObtenerPorHashtagAsync(string hashtag)
        {
            return _foroLocal.ObtenerPorHashtagAsync(hashtag);
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
                _logger.LogWarning(ex, "No se pudo contactar a Forum.API; se publica en el foro local.");
                await _foroLocal.AgregarAsync(nuevoMensaje);
                await _foroLocal.GuardarCambiosAsync();
                return true;
            }
        }
    }
}
