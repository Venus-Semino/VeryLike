using Microsoft.AspNetCore.Mvc;
using VeryLike.Domain.Interfaces;
using VeryLike.Domain.Models;

namespace VeryLike.Web.Controllers
{
    public class ForoController : Controller
    {
        private readonly IMensajeForoRepository _foroRepository;

        public ForoController(IMensajeForoRepository foroRepository)
        {
            _foroRepository = foroRepository;
        }

        public async Task<IActionResult> Index()
        {
            var mensajes = await _foroRepository.ObtenerTodosAsync();
            return View(mensajes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Publicar(MensajeForo nuevoMensaje)
        {
            if (ModelState.IsValid)
            {
                await _foroRepository.AgregarAsync(nuevoMensaje);
                return RedirectToAction(nameof(Index));
            }

            var mensajes = await _foroRepository.ObtenerTodosAsync();
            return View("Index", mensajes);
        }
    }
}
