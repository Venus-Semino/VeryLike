using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeryLike.Domain.Models
{
    /// <summary>
    /// Clase base de todo el catálogo (Películas y Series). Se mapea con
    /// Table-per-Hierarchy (TPH) en EF Core usando la columna discriminadora
    /// "Tipo" (ver ApplicationDbContext). La propiedad Tipo es de solo lectura:
    /// cada subclase declara su propio valor, así que no puede desincronizarse
    /// del tipo real de la instancia (a diferencia del campo de texto libre
    /// que se usaba en los JSON locales).
    /// </summary>
    public abstract class ContenidoAudiovisual
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Nombre { get; set; } = string.Empty;

        public List<string> Genero { get; set; } = new();

        public int AnioPublicacion { get; set; }

        [MaxLength(100)]
        public string PlataformaStreaming { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string Sinopsis { get; set; } = string.Empty;

        [MaxLength(150)]
        public string Studio { get; set; } = string.Empty;

        public double Calificacion { get; set; }

        /// <summary>URL del póster obtenida automáticamente de la API de cine externa.</summary>
        public string? PosterUrl { get; set; }

        /// <summary>
        /// Id del título en la API externa (TMDB/OMDb). Permite detectar
        /// duplicados al sincronizar el catálogo sin volver a insertar lo mismo.
        /// </summary>
        [MaxLength(50)]
        public string? IdExterno { get; set; }

        /// <summary>Discriminador de tipo, fijado por cada subclase concreta.</summary>
        [NotMapped]
        public abstract string Tipo { get; }

        /// <summary>
        /// Enlace profundo calculado dinámicamente hacia la plataforma de
        /// streaming declarada, para redirigir al usuario con un clic.
        /// No se persiste: se recalcula siempre a partir de PlataformaStreaming.
        /// </summary>
        [NotMapped]
        public string EnlaceStreaming => CalcularEnlaceStreaming();

        private string CalcularEnlaceStreaming()
        {
            var query = Uri.EscapeDataString(Nombre);

            return PlataformaStreaming.Trim().ToLowerInvariant() switch
            {
                "netflix" => $"https://www.netflix.com/search?q={query}",
                "prime video" or "amazon prime video" => $"https://www.amazon.com/s?k={query}&i=instant-video",
                "disney+" or "disney plus" => $"https://www.disneyplus.com/search?q={query}",
                "hbo max" or "max" => $"https://play.max.com/search?q={query}",
                "apple tv+" or "apple tv" => $"https://tv.apple.com/search?term={query}",
                "crunchyroll" => $"https://www.crunchyroll.com/search?q={query}",
                "mubi" => $"https://mubi.com/search/films?query={query}",
                "" => $"https://www.google.com/search?q={query}+donde+ver",
                _ => $"https://www.google.com/search?q={query}+ver+en+{Uri.EscapeDataString(PlataformaStreaming)}"
            };
        }
    }
}
