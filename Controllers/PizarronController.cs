using Microsoft.AspNetCore.Mvc;
using VeryLike.Domain.Interfaces;
using VeryLike.Domain.Models;
using VeryLike.Domain.Recomendaciones;
using VeryLike.Web.Models;

namespace VeryLike.Web.Controllers
{
    public class PizarronController : Controller
    {
        private readonly ICatalogoRepository _catalogoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly OrdenarPorCalificacionStrategy _estrategiaCalificacion;
        private readonly RecomendacionInteligenteIaStrategy _estrategiaIa;

        public PizarronController(
            ICatalogoRepository catalogoRepository,
            IUsuarioRepository usuarioRepository,
            OrdenarPorCalificacionStrategy estrategiaCalificacion,
            RecomendacionInteligenteIaStrategy estrategiaIa)
        {
            _catalogoRepository = catalogoRepository;
            _usuarioRepository = usuarioRepository;
            _estrategiaCalificacion = estrategiaCalificacion;
            _estrategiaIa = estrategiaIa;
        }

        // modo=ia (por defecto) o modo=calificacion, elegido por el usuario desde la vista.
        public async Task<IActionResult> Index(string modo = "ia")
        {
            var nombreSesion = HttpContext.Session.GetString("UsuarioNombre");
            var catalogo = await _catalogoRepository.ObtenerTodoAsync();

            var modelo = new PizarronViewModel
            {
                NombreUsuario = nombreSesion ?? "Invitado",
                ModoRecomendacion = modo
            };

            Usuario? usuario = null;
            if (nombreSesion != null)
            {
                usuario = await _usuarioRepository.ObtenerPorNombreOCorreoAsync(nombreSesion);
                if (usuario != null)
                {
                    modelo.ParaVer = await _usuarioRepository.ObtenerParaVerAsync(usuario.Id);
                }
            }

            IEstrategiaRecomendacion estrategia = modo == "calificacion"
                ? _estrategiaCalificacion
                : _estrategiaIa;

            var motor = new MotorDeRecomendacion(estrategia);
            var paraVerIds = modelo.ParaVer.Select(p => p.Id).ToHashSet();

            modelo.Recomendadas = motor.Recomendar(usuario, catalogo)
                .Where(c => !paraVerIds.Contains(c.Id))
                .Take(10)
                .ToList();

            return View(modelo);
        }

        public async Task<IActionResult> ParaVer()
        {
            var nombreSesion = HttpContext.Session.GetString("UsuarioNombre");
            if (nombreSesion is null) return RedirectToAction("Login", "Auth");

            var usuario = await _usuarioRepository.ObtenerPorNombreOCorreoAsync(nombreSesion);
            var paraVer = usuario != null
                ? await _usuarioRepository.ObtenerParaVerAsync(usuario.Id)
                : new List<ContenidoAudiovisual>();

            return View(paraVer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarAParaVer(int contenidoId)
        {
            var nombreSesion = HttpContext.Session.GetString("UsuarioNombre");
            if (nombreSesion is null) return RedirectToAction("Login", "Auth");

            var usuario = await _usuarioRepository.ObtenerPorNombreOCorreoAsync(nombreSesion);
            if (usuario != null)
            {
                await _usuarioRepository.AgregarAParaVerAsync(usuario.Id, contenidoId);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuitarDeParaVer(int contenidoId)
        {
            var nombreSesion = HttpContext.Session.GetString("UsuarioNombre");
            if (nombreSesion is null) return RedirectToAction("Login", "Auth");

            var usuario = await _usuarioRepository.ObtenerPorNombreOCorreoAsync(nombreSesion);
            if (usuario != null)
            {
                await _usuarioRepository.QuitarDeParaVerAsync(usuario.Id, contenidoId);
            }

            return RedirectToAction(nameof(ParaVer));
        }
    }
}
