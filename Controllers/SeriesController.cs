using Microsoft.AspNetCore.Mvc;
using VeryLike.Web.Services;

namespace VeryLike.Web.Controllers
{
    public class SeriesController : Controller
    {
        private readonly ICatalogoApiClient _catalogoApiClient;

        public SeriesController(ICatalogoApiClient catalogoApiClient)
        {
            _catalogoApiClient = catalogoApiClient;
        }

        public async Task<IActionResult> Index(string? genero)
        {
            var series = await _catalogoApiClient.ObtenerSeriesAsync();

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
