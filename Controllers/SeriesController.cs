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

        public async Task<IActionResult> Index()
        {
            var series = await _catalogoApiClient.ObtenerSeriesAsync();
            return View(series);
        }
    }
}
