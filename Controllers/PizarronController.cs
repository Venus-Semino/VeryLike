using Microsoft.AspNetCore.Mvc;

namespace VeryLike.Web.Controllers
{
    public class PizarronController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.NombreUsuario = "Cineasta99"; // Usuario simulado temporalmente
            return View();
        }

        // Nueva ruta para la vista expandida
        public IActionResult ParaVer()
        {
            return View();
        }
    }
}