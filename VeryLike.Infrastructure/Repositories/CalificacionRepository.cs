using Microsoft.EntityFrameworkCore;
using VeryLike.Domain.Interfaces;
using VeryLike.Domain.Models;
using VeryLike.Infrastructure.Data;

namespace VeryLike.Infrastructure.Repositories
{
    public class CalificacionRepository : ICalificacionRepository
    {
        private readonly ApplicationDbContext _context;

        public CalificacionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<Calificacion?> ObtenerDelUsuarioAsync(int usuarioId, int contenidoId)
        {
            return _context.Calificaciones
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId && c.ContenidoId == contenidoId);
        }

        public Task<List<Calificacion>> ObtenerResenasPublicasAsync(int contenidoId)
        {
            return _context.Calificaciones
                .Include(c => c.Usuario)
                .Where(c => c.ContenidoId == contenidoId && c.ResenaPublica != null && c.ResenaPublica != "")
                .OrderByDescending(c => c.FechaActualizacion)
                .ToListAsync();
        }

        public async Task<(double Promedio, int Total)> ObtenerResumenAsync(int contenidoId)
        {
            var resumen = await _context.Calificaciones
                .Where(c => c.ContenidoId == contenidoId)
                .GroupBy(c => c.ContenidoId)
                .Select(g => new { Promedio = g.Average(c => c.Puntaje), Total = g.Count() })
                .FirstOrDefaultAsync();

            return resumen is null ? (0, 0) : (Math.Round(resumen.Promedio, 1), resumen.Total);
        }

        public async Task GuardarAsync(int usuarioId, int contenidoId, double puntaje, string? resenaPublica, string? resenaPrivada)
        {
            var calificacion = await ObtenerDelUsuarioAsync(usuarioId, contenidoId);

            if (calificacion is null)
            {
                calificacion = new Calificacion
                {
                    UsuarioId = usuarioId,
                    ContenidoId = contenidoId
                };
                await _context.Calificaciones.AddAsync(calificacion);
            }

            calificacion.Puntaje = Calificacion.NormalizarPuntaje(puntaje);
            calificacion.ResenaPublica = string.IsNullOrWhiteSpace(resenaPublica) ? null : resenaPublica.Trim();
            calificacion.ResenaPrivada = string.IsNullOrWhiteSpace(resenaPrivada) ? null : resenaPrivada.Trim();
            calificacion.FechaActualizacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await ActualizarPromedioDelContenidoAsync(contenidoId);
        }

        /// <summary>
        /// Mantiene <see cref="ContenidoAudiovisual.Calificacion"/> como promedio
        /// de la comunidad, que es lo que muestran las tarjetas del catálogo.
        /// </summary>
        private async Task ActualizarPromedioDelContenidoAsync(int contenidoId)
        {
            var contenido = await _context.Contenidos.FindAsync(contenidoId);
            if (contenido is null)
            {
                return;
            }

            var (promedio, _) = await ObtenerResumenAsync(contenidoId);
            contenido.Calificacion = promedio;
            await _context.SaveChangesAsync();
        }
    }
}
