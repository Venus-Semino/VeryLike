using Microsoft.AspNetCore.Mvc;
using VeryLike.Domain.Interfaces;
using VeryLike.Domain.Models;
using VeryLike.Web.Models;
using VeryLike.Web.Services;

namespace VeryLike.Web.Controllers
{
    public class PizarronController : Controller
    {
        private readonly ICatalogoApiClient _catalogoApiClient;
        private readonly IUsuarioRepository _usuarioRepository;

        public PizarronController(ICatalogoApiClient catalogoApiClient, IUsuarioRepository usuarioRepository)
        {
            _catalogoApiClient = catalogoApiClient;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<IActionResult> Index()
        {
            var nombreSesion = HttpContext.Session.GetString("UsuarioNombre");
            var catalogo = await _catalogoApiClient.ObtenerTodoAsync();

            var modelo = new PizarronViewModel { NombreUsuario = nombreSesion ?? "Invitado" };

            if (nombreSesion != null)
            {
                var usuario = _usuarioRepository.ObtenerPorNombreOCorreo(nombreSesion);
                if (usuario != null)
                {
                    // Asumiendo que ListaParaVer tiene los IDs
                    modelo.ParaVer = catalogo.Where(c => usuario.ListaParaVer.Contains(c.Id)).ToList();
                }
            }

            // GOF - Strategy: Invocamos el patrón de comportamiento
            var motor = new MotorDeRecomendacion(new OrdenarPorCalificacionStrategy());

            // Verificamos que ParaVer no sea null para evitar errores
            var paraVerIds = modelo.ParaVer?.Select(p => p.Id).ToList() ?? new List<int>(); modelo.Recomendadas = motor.Recomendar(catalogo.ToList())
                .Where(c => !paraVerIds.Contains(c.Id))
                .Take(10)
                .ToList();

            return View(modelo);
        }

        public IActionResult ParaVer()
        {
            return View();
        }
    }

    // ====================================================================
    // PATRÓN STRATEGY (Ubicado aquí temporalmente para garantizar compilación)
    // ====================================================================
    public interface IEstrategiaRecomendacion
    {
        List<ContenidoAudiovisual> AplicarEstrategia(List<ContenidoAudiovisual> catalogo);
    }

    public class OrdenarPorCalificacionStrategy : IEstrategiaRecomendacion
    {
        public List<ContenidoAudiovisual> AplicarEstrategia(List<ContenidoAudiovisual> catalogo)
        {
            // Ordena los contenidos de mayor a menor calificación
            return catalogo.OrderByDescending(c => c.Calificacion).ToList();
        }
    }

    public class MotorDeRecomendacion
    {
        private readonly IEstrategiaRecomendacion _estrategia;

        public MotorDeRecomendacion(IEstrategiaRecomendacion estrategia)
        {
            _estrategia = estrategia;
        }

        public List<ContenidoAudiovisual> Recomendar(List<ContenidoAudiovisual> catalogo)
        {
            return _estrategia.AplicarEstrategia(catalogo);
        }
    }
}