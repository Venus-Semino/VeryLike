using VeryLike.Domain.Models;

namespace VeryLike.Web.Models
{
    public class ForoViewModel
    {
        public List<MensajeForo> Mensajes { get; set; } = new();

        /// <summary>Reseñas públicas recientes del catálogo, para darle contenido al foro.</summary>
        public List<Calificacion> ResenasRecientes { get; set; } = new();

        public string? HashtagActivo { get; set; }
    }
}
