using Microsoft.AspNetCore.Mvc;
using VeryLike.Domain.Interfaces;

namespace VeryLike.Web.Controllers
{
    public class SeriesController : Controller
    {
        private readonly ICatalogoRepository _catalogoRepository;

        public SeriesController(ICatalogoRepository catalogoRepository)
        {
            _catalogoRepository = catalogoRepository;
        }

        public async Task<IActionResult> Index(string? genero)
        {
            var series = await _catalogoRepository.ObtenerSeriesAsync();

            ViewData["Generos"] = series.SelectMany(s => s.Genero)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(g => g)
                .ToList();
            ViewData["GeneroSeleccionado"] = genero;

            if (!string.IsNullOrWhiteSpace(genero))
            {
                series = series
                    .Where(s => s.Genero.Contains(genero, StringComparer.OrdinalIgnoreCase))
                    .ToList();
            }

            return View(series);
        }
    }
}
