using VeryLike.Domain.Models;

namespace VeryLike.Web.Models
{
    /// <summary>Catálogo presentado en filas horizontales, al estilo de un cinema.</summary>
    public class CinemaViewModel
    {
        public List<FilaCinema> Filas { get; set; } = new();
        public ContenidoAudiovisual? Destacado { get; set; }
    }

    public class FilaCinema
    {
        public FilaCinema(string titulo, List<ContenidoAudiovisual> contenidos)
        {
            Titulo = titulo;
            Contenidos = contenidos;
        }

        public string Titulo { get; }
        public List<ContenidoAudiovisual> Contenidos { get; }
    }
}
