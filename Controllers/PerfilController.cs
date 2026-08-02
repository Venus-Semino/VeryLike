using Microsoft.AspNetCore.Mvc;
using VeryLike.Domain.Interfaces;
using VeryLike.Web.Models;

namespace VeryLike.Web.Controllers
{
    public class PerfilController : Controller
    {
        private const string ClaveSesion = "UsuarioNombre";

        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ICalificacionRepository _calificacionRepository;
        private readonly IMensajeForoRepository _foroRepository;

        public PerfilController(
            IUsuarioRepository usuarioRepository,
            ICalificacionRepository calificacionRepository,
            IMensajeForoRepository foroRepository)
        {
            _usuarioRepository = usuarioRepository;
            _calificacionRepository = calificacionRepository;
            _foroRepository = foroRepository;
        }

        public async Task<IActionResult> Index()
        {
            var nombreSesion = HttpContext.Session.GetString(ClaveSesion);
            if (nombreSesion is null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var modelo = await ArmarPerfilAsync(nombreSesion, esPropio: true);
            return modelo is null ? RedirectToAction("Login", "Auth") : View(modelo);
        }

        /// <summary>Perfil público: sin reseñas privadas ni lista "Para Ver".</summary>
        public async Task<IActionResult> Ver(string nombreUsuario)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
            {
                return NotFound();
            }

            var esPropio = string.Equals(
                nombreUsuario,
                HttpContext.Session.GetString(ClaveSesion),
                StringComparison.OrdinalIgnoreCase);

            var modelo = await ArmarPerfilAsync(nombreUsuario, esPropio);
            return modelo is null ? NotFound() : View(nameof(Index), modelo);
        }

        private async Task<PerfilViewModel?> ArmarPerfilAsync(string nombreUsuario, bool esPropio)
        {
            var usuario = await _usuarioRepository.ObtenerPorNombreOCorreoAsync(nombreUsuario);
            if (usuario is null)
            {
                return null;
            }

            var calificaciones = await _calificacionRepository.ObtenerDeUsuarioAsync(usuario.Id);
            if (!esPropio)
            {
                calificaciones.ForEach(c => c.ResenaPrivada = null);
            }

            return new PerfilViewModel
            {
                NombreUsuario = usuario.NombreUsuario,
                EsPropio = esPropio,
                Calificaciones = calificaciones,
                Publicaciones = await _foroRepository.ObtenerDeUsuarioAsync(usuario.NombreUsuario),
                ParaVer = esPropio
                    ? await _usuarioRepository.ObtenerParaVerAsync(usuario.Id)
                    : new()
            };
        }
    }
}
