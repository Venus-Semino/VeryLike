using Microsoft.AspNetCore.Mvc;
using VeryLike.Domain.Interfaces;

namespace VeryLike.Web.Controllers
{
    /// <summary>
    /// Base de los controladores que muestran tarjetas del catálogo: deja en
    /// ViewData los ids de la lista "Para Ver" del usuario, que es lo que usa
    /// el parcial _BotonParaVer para saber si el título ya está agregado.
    /// </summary>
    public abstract class ControladorConParaVer : Controller
    {
        protected const string ClaveSesion = "UsuarioNombre";

        private readonly IUsuarioRepository _usuarioRepositorio;

        protected ControladorConParaVer(IUsuarioRepository usuarioRepositorio)
        {
            _usuarioRepositorio = usuarioRepositorio;
        }

        protected async Task<HashSet<int>> CargarParaVerIdsAsync()
        {
            var ids = new HashSet<int>();
            var nombreSesion = HttpContext.Session.GetString(ClaveSesion);

            if (!string.IsNullOrEmpty(nombreSesion))
            {
                var usuario = await _usuarioRepositorio.ObtenerPorNombreOCorreoAsync(nombreSesion);
                if (usuario is not null)
                {
                    var paraVer = await _usuarioRepositorio.ObtenerParaVerAsync(usuario.Id);
                    ids = paraVer.Select(c => c.Id).ToHashSet();
                }
            }

            ViewData["ParaVerIds"] = ids;
            return ids;
        }
    }
}
