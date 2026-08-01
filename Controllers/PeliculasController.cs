using Microsoft.AspNetCore.Mvc;
using VeryLike.Web.Services;

namespace VeryLike.Web.Controllers
{
    public class PeliculasController : Controller
    {
        private readonly ICatalogoApiClient _catalogoApiClient;

        public PeliculasController(ICatalogoApiClient catalogoApiClient)
        {
            _catalogoApiClient = catalogoApiClient;
        }

        public async Task<IActionResult> Index(string? genero)
        {
            var peliculas = await _catalogoApiClient.ObtenerPeliculasAsync();

            ViewData["Generos"] = peliculas.SelectMany(p => p.Genero)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(g => g)
                .ToList();
            ViewData["GeneroSeleccionado"] = genero;

            if (!string.IsNullOrWhiteSpace(genero))
            {
                peliculas = peliculas
                    .Where(p => p.Genero.Contains(genero, StringComparer.OrdinalIgnoreCase))
                    .ToList();
            }

            return View(peliculas);
        }
    }
}
