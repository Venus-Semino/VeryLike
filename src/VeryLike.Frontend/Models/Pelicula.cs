namespace VeryLike.Frontend.Models
{
    public class Pelicula
    {
        public string Nombre { get; set; } = string.Empty;
        public string Genero { get; set; } = string.Empty;
        public int AnioPublicacion { get; set; }
        public string PlataformaStreaming { get; set; } = string.Empty;
        public string Duracion { get; set; } = string.Empty;
        public string Sinopsis { get; set; } = string.Empty;
        public string Studio { get; set; } = string.Empty;
        public int Calificacion { get; set; } // Valores del 1 al 5
    }
}