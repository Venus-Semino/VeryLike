using Microsoft.AspNetCore.Mvc;
using VeryLike.Domain.Interfaces;
using VeryLike.Domain.Models;
using VeryLike.Web.Models;

namespace VeryLike.Web.Controllers
{
    /// <summary>
    /// Endpoints JSON que alimentan el modal de detalle: resumen de la
    /// comunidad, reseñas públicas y guardado de la calificación del usuario.
    /// </summary>
    [Route("[controller]/[action]")]
    public class CalificacionesController : Controller
    {
        private readonly ICalificacionRepository _calificacionRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public CalificacionesController(
            ICalificacionRepository calificacionRepository,
            IUsuarioRepository usuarioRepository)
        {
            _calificacionRepository = calificacionRepository;
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Detalle(int contenidoId)
        {
            var (promedio, total) = await _calificacionRepository.ObtenerResumenAsync(contenidoId);
            var usuario = await ObtenerUsuarioDeSesionAsync();

            var modelo = new DetalleCalificacionViewModel
            {
                Promedio = promedio,
                Total = total,
                Autenticado = usuario is not null,
                Resenas = (await _calificacionRepository.ObtenerResenasPublicasAsync(contenidoId))
                    .Select(c => new ResenaViewModel
                    {
                        Autor = c.Usuario?.NombreUsuario ?? "Anónimo",
                        Puntaje = c.Puntaje,
                        Texto = c.ResenaPublica ?? string.Empty
                    })
                    .ToList()
            };

            if (usuario is not null)
            {
                var mia = await _calificacionRepository.ObtenerDelUsuarioAsync(usuario.Id, contenidoId);
                if (mia is not null)
                {
                    modelo.MiCalificacion = new MiCalificacionViewModel
                    {
                        Puntaje = mia.Puntaje,
                        ResenaPublica = mia.ResenaPublica,
                        ResenaPrivada = mia.ResenaPrivada
                    };
                }
            }

            return Json(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Guardar([FromBody] GuardarCalificacionRequest peticion)
        {
            var usuario = await ObtenerUsuarioDeSesionAsync();
            if (usuario is null)
            {
                return Unauthorized(new { mensaje = "Inicia sesión para calificar." });
            }

            if (peticion.Puntaje < 0.5 || peticion.Puntaje > 5)
            {
                return BadRequest(new { mensaje = "El puntaje debe estar entre 0.5 y 5." });
            }

            await _calificacionRepository.GuardarAsync(
                usuario.Id,
                peticion.ContenidoId,
                peticion.Puntaje,
                peticion.ResenaPublica,
                peticion.ResenaPrivada);

            var (promedio, total) = await _calificacionRepository.ObtenerResumenAsync(peticion.ContenidoId);
            return Json(new { promedio, total });
        }

        private Task<Usuario?> ObtenerUsuarioDeSesionAsync()
        {
            var nombreSesion = HttpContext.Session.GetString("UsuarioNombre");
            return nombreSesion is null
                ? Task.FromResult<Usuario?>(null)
                : _usuarioRepository.ObtenerPorNombreOCorreoAsync(nombreSesion);
        }
    }
}
