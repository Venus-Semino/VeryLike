using Microsoft.AspNetCore.Mvc;
using VeryLike.Domain.Interfaces;
using VeryLike.Domain.Models;
using VeryLike.Web.Models;
using VeryLike.Web.Services;

namespace VeryLike.Web.Controllers
{
    public class CatalogoController : Controller
    {
        private readonly SincronizadorCatalogoService _sincronizador;
        private readonly ICatalogoRepository _catalogoRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public CatalogoController(
            SincronizadorCatalogoService sincronizador,
            ICatalogoRepository catalogoRepository,
            IUsuarioRepository usuarioRepository)
        {
            _sincronizador = sincronizador;
            _catalogoRepository = catalogoRepository;
            _usuarioRepository = usuarioRepository;
        }

        /// <summary>Catálogo completo en filas: tendencias, tipos y géneros.</summary>
        public async Task<IActionResult> Cinema()
        {
            var catalogo = await _catalogoRepository.ObtenerTodoAsync();

            var modelo = new CinemaViewModel
            {
                Destacado = catalogo.OrderByDescending(c => c.Calificacion).FirstOrDefault()
            };

            if (catalogo.Count > 0)
            {
                modelo.Filas.Add(new FilaCinema(
                    "Tendencias",
                    catalogo.OrderByDescending(c => c.Calificacion).Take(12).ToList()));

                modelo.Filas.Add(new FilaCinema(
                    "Estrenos",
                    catalogo.OrderByDescending(c => c.AnioPublicacion).Take(12).ToList()));

                var generos = catalogo
                    .SelectMany(c => c.Genero)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(g => g);

                foreach (var genero in generos)
                {
                    var deGenero = catalogo
                        .Where(c => c.Genero.Contains(genero, StringComparer.OrdinalIgnoreCase))
                        .ToList();

                    modelo.Filas.Add(new FilaCinema(genero, deGenero));
                }
            }

            return View(modelo);
        }

        /// <summary>Buscador global del menú lateral: títulos del catálogo y usuarios.</summary>
        public async Task<IActionResult> Buscar(string? q)
        {
            var modelo = new BusquedaViewModel { Consulta = q };

            if (!string.IsNullOrWhiteSpace(q))
            {
                var catalogo = await _catalogoRepository.ObtenerTodoAsync();

                modelo.Titulos = catalogo
                    .Where(c => c.Nombre.Contains(q, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                modelo.Usuarios = await _usuarioRepository.BuscarPorNombreAsync(q.Trim());
            }

            return View(modelo);
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
