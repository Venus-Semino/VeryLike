using Microsoft.AspNetCore.Mvc;
using VeryLike.Web.Services;

namespace VeryLike.Web.Controllers
{
    public class CatalogoController : Controller
    {
        private readonly SincronizadorCatalogoService _sincronizador;

        public CatalogoController(SincronizadorCatalogoService sincronizador)
        {
            _sincronizador = sincronizador;
        }

        /// <summary>
        /// Trae títulos populares desde TMDB y los guarda en el catálogo local,
        /// para que haya contenido que calificar sin cargarlo a mano.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sincronizar(string origen = "Peliculas")
        {
            try
            {
                var agregados = await _sincronizador.SincronizarPopularesAsync();
                TempData["MensajeCatalogo"] = agregados > 0
                    ? $"Se agregaron {agregados} título(s) desde TMDB."
                    : "El catálogo ya está al día (no hubo títulos nuevos).";
            }
            catch (HttpRequestException)
            {
                TempData["MensajeCatalogo"] = "No se pudo contactar a TMDB. Revisa tu conexión y la API key (Tmdb:ApiKey).";
            }

            return RedirectToAction("Index", origen);
        }
    }
}
