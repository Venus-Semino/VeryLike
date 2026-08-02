namespace VeryLike.Domain.DTOs
{
    /// <summary>
    /// DTO neutral que representa un título tal como llega desde la API de
    /// cine externa (o desde cualquier otra fuente, incluida la propia base
    /// de datos si algún día se necesita re-mapear). ContenidoFactory lo
    /// traduce a una instancia concreta de Pelicula o Serie evaluando "Tipo".
    /// </summary>
    public class ContenidoExternoDto
    {
        /// <summary>Id del título en la API externa (ej. TMDB). Usado para deduplicar.</summary>
        public string? IdExterno { get; set; }

        /// <summary>Discriminador leído de la API externa: "Pelicula" o "Serie".</summary>
        public string Tipo { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;
        public List<string> Genero { get; set; } = new();
        public int AnioPublicacion { get; set; }
        public string PlataformaStreaming { get; set; } = string.Empty;
        public string Sinopsis { get; set; } = string.Empty;
        public string Studio { get; set; } = string.Empty;
        public double Calificacion { get; set; }
        public string? PosterUrl { get; set; }

        /// <summary>Solo aplica cuando Tipo == "Pelicula".</summary>
        public string? Duracion { get; set; }

        /// <summary>Solo aplica cuando Tipo == "Serie".</summary>
        public int? Temporadas { get; set; }
    }
}
