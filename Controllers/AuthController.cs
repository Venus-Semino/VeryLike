using Microsoft.AspNetCore.Mvc;
using VeryLike.Domain.Interfaces;
using VeryLike.Domain.Models;

namespace VeryLike.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private const string ClaveSesion = "UsuarioNombre";

        public AuthController(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        // --- PÁGINA DE REGISTRO ---
        public IActionResult Registro()
        {
            ViewData["HideMenu"] = true;
            return View();
        }

        [HttpPost]
        public IActionResult Registro(Usuario nuevoUsuario)
        {
            if (_usuarioRepository.ObtenerPorNombreOCorreo(nuevoUsuario.NombreUsuario) != null)
            {
                ModelState.AddModelError("NombreUsuario", "Este nombre de usuario ya está en uso. Elige otro.");
                ViewData["HideMenu"] = true;
                return View(nuevoUsuario);
            }

            _usuarioRepository.Agregar(nuevoUsuario);
            HttpContext.Session.SetString(ClaveSesion, nuevoUsuario.NombreUsuario);
            return RedirectToAction("Index", "Pizarron");
        }

        // --- PÁGINA DE INICIAR SESIÓN ---
        public IActionResult Login()
        {
            ViewData["HideMenu"] = true;
            return View();
        }

        [HttpPost]
        public IActionResult Login(string identificador, string contrasena)
        {
            var usuario = _usuarioRepository.ObtenerPorNombreOCorreo(identificador);

            if (usuario != null && usuario.Contrasena == contrasena)
            {
                HttpContext.Session.SetString(ClaveSesion, usuario.NombreUsuario);
                return RedirectToAction("Index", "Pizarron");
            }

            ModelState.AddModelError(string.Empty, "Credenciales incorrectas.");
            ViewData["HideMenu"] = true;
            return View();
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove(ClaveSesion);
            return RedirectToAction("Index", "Home");
        }
    }
}
