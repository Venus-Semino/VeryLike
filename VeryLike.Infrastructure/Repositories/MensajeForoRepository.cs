using Microsoft.EntityFrameworkCore;
using VeryLike.Domain.Interfaces;
using VeryLike.Domain.Models;
using VeryLike.Infrastructure.Data;

namespace VeryLike.Infrastructure.Repositories
{
    public class MensajeForoRepository : IMensajeForoRepository
    {
        private readonly ApplicationDbContext _context;

        public MensajeForoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<List<MensajeForo>> ObtenerTodosAsync()
        {
            return _context.MensajesForo
                .Where(m => m.MensajePadreId == null)
                .Include(m => m.Comentarios)
                .OrderByDescending(m => m.FechaPublicacion)
                .ToListAsync();
        }

        public async Task<List<MensajeForo>> ObtenerPorHashtagAsync(string hashtag)
        {
            // Hashtags se persiste como texto con conversor de valor, así que
            // el filtro se resuelve en memoria sobre las publicaciones raíz.
            var publicaciones = await ObtenerTodosAsync();

            return publicaciones
                .Where(m => m.Hashtags.Contains(hashtag, StringComparer.OrdinalIgnoreCase))
                .ToList();
        }

        public Task<MensajeForo?> ObtenerPorIdAsync(int id)
        {
            return _context.MensajesForo
                .Include(m => m.Comentarios)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task AgregarAsync(MensajeForo mensaje)
        {
            await _context.MensajesForo.AddAsync(mensaje);
        }

        public Task GuardarCambiosAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
