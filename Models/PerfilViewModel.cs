using VeryLike.Domain.Models;

namespace VeryLike.Web.Models
{
    /// <summary>
    /// Perfil de un cinéfilo. La vista es la misma para el perfil propio y el
    /// público; <see cref="EsPropio"/> decide qué datos privados se muestran.
    /// </summary>
    public class PerfilViewModel
    {
        public string NombreUsuario { get; set; } = string.Empty;
        public bool EsPropio { get; set; }

        public List<Calificacion> Calificaciones { get; set; } = new();
        public List<MensajeForo> Publicaciones { get; set; } = new();

        /// <summary>Solo se completa en el perfil propio: la lista "Para Ver" es privada.</summary>
        public List<ContenidoAudiovisual> ParaVer { get; set; } = new();

        public int TotalVistas => Calificaciones.Count;
        public int TotalResenas => Calificaciones.Count(c => !string.IsNullOrWhiteSpace(c.ResenaPublica));
        public double PromedioPropio => Calificaciones.Count == 0
            ? 0
            : Math.Round(Calificaciones.Average(c => c.Puntaje), 1);
    }
}
