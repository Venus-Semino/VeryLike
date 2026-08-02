using Microsoft.AspNetCore.Mvc;
using VeryLike.Domain.Interfaces;
using VeryLike.Domain.Models;

namespace VeryLike.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPasswordHasher _passwordHasher;
        private const string ClaveSesion = "UsuarioNombre";

        public AuthController(IUsuarioRepository usuarioRepository, IPasswordHasher passwordHasher)
        {
            _usuarioRepository = usuarioRepository;
            _passwordHasher = passwordHasher;
        }

        // --- PÁGINA DE REGISTRO ---
        public IActionResult Registro()
        {
            ViewData["HideMenu"] = true;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(Usuario nuevoUsuario)
        {
            if (!ModelState.IsValid)
            {
                ViewData["HideMenu"] = true;
                return View(nuevoUsuario);
            }

            if (await _usuarioRepository.ObtenerPorNombreOCorreoAsync(nuevoUsuario.NombreUsuario) != null)
            {
                ModelState.AddModelError("NombreUsuario", "Este nombre de usuario ya está en uso. Elige otro.");
                ViewData["HideMenu"] = true;
                return View(nuevoUsuario);
            }

            nuevoUsuario.Contrasena = _passwordHasher.Hash(nuevoUsuario.Contrasena);
            await _usuarioRepository.AgregarAsync(nuevoUsuario);
            await _usuarioRepository.GuardarCambiosAsync();

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string identificador, string contrasena)
        {
            var usuario = await _usuarioRepository.ObtenerPorNombreOCorreoAsync(identificador);

            if (usuario != null && _passwordHasher.Verificar(contrasena, usuario.Contrasena))
            {
                HttpContext.Session.SetString(ClaveSesion, usuario.NombreUsuario);
                return RedirectToAction("Index", "Pizarron");
            }

            ModelState.AddModelError(string.Empty, "Credenciales incorrectas.");
            ViewData["HideMenu"] = true;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove(ClaveSesion);
            return RedirectToAction("Index", "Home");
        }
    }
}
