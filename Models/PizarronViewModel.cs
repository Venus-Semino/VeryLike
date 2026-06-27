using VeryLike.Domain.Models;

namespace VeryLike.Web.Models
{
    public class PizarronViewModel
    {
        public string NombreUsuario { get; set; } = "Invitado";
        public List<ContenidoAudiovisual> ParaVer { get; set; } = new();
        public List<ContenidoAudiovisual> Recomendadas { get; set; } = new();
    }
}
