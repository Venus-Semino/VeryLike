using Microsoft.EntityFrameworkCore;
using VeryLike.Domain.Interfaces;
using VeryLike.Domain.Models;
using VeryLike.Infrastructure.Data;

namespace VeryLike.Infrastructure.Repositories
{
    public class CatalogoRepository : ICatalogoRepository
    {
        private readonly ApplicationDbContext _context;

        public CatalogoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ContenidoAudiovisual>> ObtenerTodoAsync()
        {
            return await _context.Contenidos.OrderBy(c => c.Nombre).ToListAsync();
        }

        public Task<List<Pelicula>> ObtenerPeliculasAsync()
        {
            return _context.Peliculas.OrderBy(p => p.Nombre).ToListAsync();
        }

        public Task<List<Serie>> ObtenerSeriesAsync()
        {
            return _context.Series.OrderBy(s => s.Nombre).ToListAsync();
        }

        public async Task<ContenidoAudiovisual?> ObtenerPorIdAsync(int id)
        {
            return await _context.Contenidos.FindAsync(id);
        }

        public async Task<List<string>> ObtenerGenerosAsync()
        {
            var contenidos = await _context.Contenidos.ToListAsync();

            return contenidos
                .SelectMany(c => c.Genero)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(g => g)
                .ToList();
        }

        public Task<bool> ExisteIdExternoAsync(string idExterno)
        {
            return _context.Contenidos.AnyAsync(c => c.IdExterno == idExterno);
        }

        public async Task AgregarAsync(ContenidoAudiovisual contenido)
        {
            await _context.Contenidos.AddAsync(contenido);
        }

        public Task GuardarCambiosAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
