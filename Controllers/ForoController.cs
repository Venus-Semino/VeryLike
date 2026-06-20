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

        public IActionResult Index()
        {
            var mensajes = _foroRepository.ObtenerTodos();
            return View(mensajes);
        }

        [HttpPost]
        public IActionResult Publicar(MensajeForo nuevoMensaje)
        {
            if (ModelState.IsValid)
            {
                _foroRepository.Agregar(nuevoMensaje);
                return RedirectToAction("Index");
            }

            var mensajes = _foroRepository.ObtenerTodos();
            return View("Index", mensajes);
        }
    }
}