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
    /// Esta clase es la pieza que faltaba: en vez de leer catalogo.json
    /// directamente (como hacía antes), VeryLike.Web le pide los datos a
    /// VeryLike.Catalog.API por HTTP. Así la API que ya existía deja de
    /// estar "huérfana" y el catálogo vive en un solo lugar.
    /// </summary>
    public class CatalogoApiClient : ICatalogoApiClient
    {
        private readonly HttpClient _http;
        private static readonly JsonSerializerOptions _opciones = new() { PropertyNameCaseInsensitive = true };

        public CatalogoApiClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Pelicula>> ObtenerPeliculasAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<List<Pelicula>>("api/catalogo/peliculas", _opciones)
                       ?? new List<Pelicula>();
            }
            catch (HttpRequestException)
            {
                // El microservicio no está levantado todavía: la vista no debe
                // tronar, solo se queda sin datos hasta que Catalog.API esté arriba.
                return new List<Pelicula>();
            }
        }

        public async Task<List<Serie>> ObtenerSeriesAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<List<Serie>>("api/catalogo/series", _opciones)
                       ?? new List<Serie>();
            }
            catch (HttpRequestException)
            {
                return new List<Serie>();
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
