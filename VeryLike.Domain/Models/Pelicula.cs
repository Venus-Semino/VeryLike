namespace VeryLike.Domain.Models
{
    public class Pelicula : ContenidoAudiovisual
    {
        public override string Tipo => "Pelicula";

        /// <summary>Ej. "1h 54m". Se guarda como texto para no perder el formato de la API externa.</summary>
        public string Duracion { get; set; } = "Desconocida";
    }
}
