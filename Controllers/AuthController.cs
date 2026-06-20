using Microsoft.AspNetCore.Mvc;
using VeryLike.Domain.Interfaces;
using VeryLike.Domain.Models;

namespace VeryLike.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public AuthController(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        // --- PÁGINA DE REGISTRO ---
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registro(Usuario nuevoUsuario)
        {
            if (_usuarioRepository.ExisteNombreUsuario(nuevoUsuario.NombreUsuario))
            {
                // Si el usuario existe, mandamos el error a la vista
                ModelState.AddModelError("NombreUsuario", "Este nombre de usuario ya está en uso. Elige otro.");
                return View(nuevoUsuario);
            }

            _usuarioRepository.Agregar(nuevoUsuario);
            // Si se registra con éxito, lo mandamos directo al "Pizarrón" (lo haremos en el siguiente paso)
            return RedirectToAction("Index", "Peliculas");
        }

        // --- PÁGINA DE INICIAR SESIÓN ---
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string identificador, string contrasena)
        {
            var usuarios = _usuarioRepository.ObtenerTodos();

            // Corregido: Ahora comparamos contra u.Contrasena
            var usuarioValido = usuarios.FirstOrDefault(u =>
                (u.NombreUsuario == identificador || u.Correo == identificador) &&
                u.Contrasena == contrasena);

            if (usuarioValido != null)
            {
                return RedirectToAction("Index", "Peliculas");
            }

            ModelState.AddModelError(string.Empty, "Credenciales incorrectas.");
            return View();
        }
    }
}