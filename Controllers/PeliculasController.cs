using Microsoft.AspNetCore.Mvc;
using VeryLike.Domain.Interfaces;

namespace VeryLike.Web.Controllers
{
    public class PeliculasController : Controller
    {
        private readonly ICatalogoRepository _catalogoRepository;

        public PeliculasController(ICatalogoRepository catalogoRepository)
        {
            _catalogoRepository = catalogoRepository;
        }

        public async Task<IActionResult> Index(string? genero)
        {
            var peliculas = await _catalogoRepository.ObtenerPeliculasAsync();

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
