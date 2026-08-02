using Microsoft.AspNetCore.Mvc;
using VeryLike.Domain.Interfaces;
using VeryLike.Web.Services;

namespace VeryLike.Web.Controllers
{
    public class SeriesController : ControladorConParaVer
    {
        private readonly ICatalogoApiClient _catalogoApiClient;

        public SeriesController(ICatalogoApiClient catalogoApiClient, IUsuarioRepository usuarioRepositorio)
            : base(usuarioRepositorio)
        {
            _catalogoApiClient = catalogoApiClient;
        }

        public async Task<IActionResult> Index(string? genero)
        {
            var series = await _catalogoApiClient.ObtenerSeriesAsync();
            await CargarParaVerIdsAsync();

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
