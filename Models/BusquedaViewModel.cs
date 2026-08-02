using VeryLike.Domain.Models;

namespace VeryLike.Web.Models
{
    /// <summary>Resultado del buscador global del menú: títulos y usuarios.</summary>
    public class BusquedaViewModel
    {
        public string? Consulta { get; set; }
        public List<ContenidoAudiovisual> Titulos { get; set; } = new();
        public List<Usuario> Usuarios { get; set; } = new();

        public bool SinResultados => !Titulos.Any() && !Usuarios.Any();
    }
}
