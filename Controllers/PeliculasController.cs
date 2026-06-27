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

        // El catálogo es de solo lectura en este MVP: las películas se
        // gestionan en VeryLike.Catalog.API, no se agregan desde aquí.
        public async Task<IActionResult> Index()
        {
            var peliculas = await _catalogoApiClient.ObtenerPeliculasAsync();
            return View(peliculas);
        }
    }
}
