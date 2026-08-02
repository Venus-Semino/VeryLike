using System.ComponentModel.DataAnnotations;

namespace VeryLike.Domain.Models
{
    /// <summary>
    /// Calificación de un usuario sobre un título del catálogo: puntaje en
    /// pasos de 0.5 (1 a 5), reseña pública para el foro y reseña privada que
    /// solo ve su autor. Un usuario tiene a lo sumo una por contenido.
    /// </summary>
    public class Calificacion
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public int ContenidoId { get; set; }
        public ContenidoAudiovisual? Contenido { get; set; }

        [Range(0.5, 5)]
        public double Puntaje { get; set; }

        [MaxLength(2000)]
        public string? ResenaPublica { get; set; }

        [MaxLength(2000)]
        public string? ResenaPrivada { get; set; }

        public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;

        /// <summary>Redondea al múltiplo de 0.5 más cercano dentro de [0.5, 5].</summary>
        public static double NormalizarPuntaje(double puntaje)
        {
            var redondeado = Math.Round(puntaje * 2, MidpointRounding.AwayFromZero) / 2;
            return Math.Clamp(redondeado, 0.5, 5);
        }
    }
}
