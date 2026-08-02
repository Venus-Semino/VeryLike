using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeryLike.Domain.Models
{
    public class MensajeForo
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "Escribe algo antes de publicar.")]
        [MaxLength(2000)]
        public string Contenido { get; set; } = string.Empty;

        public DateTime FechaPublicacion { get; set; } = DateTime.UtcNow;

        /// <summary>Null en las publicaciones raíz; apunta al mensaje comentado en las respuestas.</summary>
        public int? MensajePadreId { get; set; }
        public MensajeForo? MensajePadre { get; set; }
        public List<MensajeForo> Comentarios { get; set; } = new();

        /// <summary>Etiquetas del mensaje, generadas automáticamente y/o escritas por el autor.</summary>
        public List<string> Hashtags { get; set; } = new();

        [NotMapped]
        public bool EsComentario => MensajePadreId.HasValue;
    }
}
