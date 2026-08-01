using Microsoft.AspNetCore.Mvc;
using VeryLike.Domain.Models;
using VeryLike.Web.Services;

namespace VeryLike.Web.Controllers
{
    public class ForoController : Controller
    {
        private readonly IForoApiClient _foroApiClient;

        public ForoController(IForoApiClient foroApiClient)
        {
            _foroApiClient = foroApiClient;
        }

        public async Task<IActionResult> Index()
        {
            var mensajes = await _foroApiClient.ObtenerTodosAsync();
            return View(mensajes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Publicar(MensajeForo nuevoMensaje)
        {
            if (ModelState.IsValid)
            {
                await _foroApiClient.PublicarAsync(nuevoMensaje);
                return RedirectToAction(nameof(Index));
            }

            var mensajes = await _foroApiClient.ObtenerTodosAsync();
            return View("Index", mensajes);
        }
    }
}
