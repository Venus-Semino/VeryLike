using VeryLike.Domain.Models;

namespace VeryLike.Web.Models
{
    public class PizarronViewModel
    {
        public string NombreUsuario { get; set; } = "Invitado";
        public List<ContenidoAudiovisual> ParaVer { get; set; } = new();
        public List<ContenidoAudiovisual> Recomendadas { get; set; } = new();

        /// <summary>Títulos calificados y publicaciones propias, para el panel lateral.</summary>
        public int TotalCalificadas { get; set; }
        public int TotalResenas { get; set; }
        public List<MensajeForo> MisPublicaciones { get; set; } = new();

        /// <summary>Títulos de la lista "Para Ver" que el usuario todavía no calificó.</summary>
        public List<ContenidoAudiovisual> PendientesDeCalificar { get; set; } = new();

        /// <summary>"ia" o "calificacion". Controla qué IEstrategiaRecomendacion se usó.</summary>
        public string ModoRecomendacion { get; set; } = "ia";
    }
}
