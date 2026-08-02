using Microsoft.AspNetCore.Mvc;
using VeryLike.Domain.Interfaces;
using VeryLike.Domain.Models;
using VeryLike.Domain.Recomendaciones;
using VeryLike.Web.Models;
using VeryLike.Web.Services;

namespace VeryLike.Web.Controllers
{
    public class PizarronController : Controller
    {
        private readonly ICatalogoApiClient _catalogoApiClient;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ICalificacionRepository _calificacionRepository;
        private readonly IMensajeForoRepository _foroRepository;
        private readonly OrdenarPorCalificacionStrategy _estrategiaCalificacion;
        private readonly RecomendacionInteligenteIaStrategy _estrategiaIa;

        public PizarronController(
            ICatalogoApiClient catalogoApiClient,
            IUsuarioRepository usuarioRepository,
            ICalificacionRepository calificacionRepository,
            IMensajeForoRepository foroRepository,
            OrdenarPorCalificacionStrategy estrategiaCalificacion,
            RecomendacionInteligenteIaStrategy estrategiaIa)
        {
            _catalogoApiClient = catalogoApiClient;
            _usuarioRepository = usuarioRepository;
            _calificacionRepository = calificacionRepository;
            _foroRepository = foroRepository;
            _estrategiaCalificacion = estrategiaCalificacion;
            _estrategiaIa = estrategiaIa;
        }

        // modo=ia (por defecto) o modo=calificacion, elegido por el usuario desde la vista.
        public async Task<IActionResult> Index(string modo = "ia")
        {
            var nombreSesion = HttpContext.Session.GetString("UsuarioNombre");
            var catalogo = await _catalogoApiClient.ObtenerTodoAsync();

            var modelo = new PizarronViewModel
            {
                NombreUsuario = nombreSesion ?? "Invitado",
                ModoRecomendacion = modo
            };

            var perfil = PerfilDeGustos.Anonimo;
            if (nombreSesion != null)
            {
                var usuario = await _usuarioRepository.ObtenerPorNombreOCorreoAsync(nombreSesion);
                if (usuario != null)
                {
                    modelo.ParaVer = await _usuarioRepository.ObtenerParaVerAsync(usuario.Id);
                    usuario.ListaParaVer = modelo.ParaVer;

                    var calificaciones = await _calificacionRepository.ObtenerDeUsuarioAsync(usuario.Id);
                    var calificados = calificaciones.Select(c => c.ContenidoId).ToHashSet();

                    modelo.TotalCalificadas = calificaciones.Count;
                    modelo.TotalResenas = calificaciones.Count(c => !string.IsNullOrWhiteSpace(c.ResenaPublica));
                    modelo.PendientesDeCalificar = modelo.ParaVer.Where(c => !calificados.Contains(c.Id)).ToList();
                    modelo.MisPublicaciones = await _foroRepository.ObtenerDeUsuarioAsync(usuario.NombreUsuario);

                    perfil = new PerfilDeGustos(usuario, calificaciones);
                    modelo.GenerosFavoritos = perfil.GenerosFavoritos(3);
                }
            }

            IEstrategiaRecomendacion estrategia = modo == "calificacion"
                ? _estrategiaCalificacion
                : _estrategiaIa;

            var motor = new MotorDeRecomendacion(estrategia);
            var paraVerIds = modelo.ParaVer.Select(p => p.Id).ToHashSet();
            ViewData["ParaVerIds"] = paraVerIds;

            modelo.Recomendadas = motor.Recomendar(perfil, catalogo)
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
        public async Task<IActionResult> AgregarAParaVer(int contenidoId, string? retorno = null)
        {
            var nombreSesion = HttpContext.Session.GetString("UsuarioNombre");
            if (nombreSesion is null) return RedirectToAction("Login", "Auth");

            var usuario = await _usuarioRepository.ObtenerPorNombreOCorreoAsync(nombreSesion);
            if (usuario != null)
            {
                await _usuarioRepository.AgregarAParaVerAsync(usuario.Id, contenidoId);
                await _usuarioRepository.GuardarCambiosAsync();
            }

            return Volver(retorno, nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuitarDeParaVer(int contenidoId, string? retorno = null)
        {
            var nombreSesion = HttpContext.Session.GetString("UsuarioNombre");
            if (nombreSesion is null) return RedirectToAction("Login", "Auth");

            var usuario = await _usuarioRepository.ObtenerPorNombreOCorreoAsync(nombreSesion);
            if (usuario != null)
            {
                await _usuarioRepository.QuitarDeParaVerAsync(usuario.Id, contenidoId);
                await _usuarioRepository.GuardarCambiosAsync();
            }

            return Volver(retorno, nameof(ParaVer));
        }

        /// <summary>Vuelve a la página desde la que se tocó el botón, si es una ruta local.</summary>
        private IActionResult Volver(string? retorno, string accionPorDefecto)
        {
            return !string.IsNullOrWhiteSpace(retorno) && Url.IsLocalUrl(retorno)
                ? Redirect(retorno)
                : RedirectToAction(accionPorDefecto);
        }
    }
}
