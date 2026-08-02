using VeryLike.Domain.Models;

namespace VeryLike.Domain.Interfaces
{
    public interface ICalificacionRepository
    {
        Task<Calificacion?> ObtenerDelUsuarioAsync(int usuarioId, int contenidoId);

        /// <summary>Reseñas públicas de un contenido, de la más reciente a la más antigua.</summary>
        Task<List<Calificacion>> ObtenerResenasPublicasAsync(int contenidoId);

        /// <summary>Promedio y cantidad de calificaciones de la comunidad.</summary>
        Task<(double Promedio, int Total)> ObtenerResumenAsync(int contenidoId);

        /// <summary>Crea o actualiza la calificación del usuario y persiste los cambios.</summary>
        Task GuardarAsync(int usuarioId, int contenidoId, double puntaje, string? resenaPublica, string? resenaPrivada);
    }
}
