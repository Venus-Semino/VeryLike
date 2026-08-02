using System.ComponentModel.DataAnnotations;

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
    }
}
