using Microsoft.EntityFrameworkCore;
using VeryLike.Domain.Interfaces;
using VeryLike.Domain.Models;
using VeryLike.Infrastructure.Data;

namespace VeryLike.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación con EF Core de <see cref="IUsuarioRepository"/>: reemplaza
    /// la lectura/escritura de usuarios.json por consultas sobre SQL Server.
    /// </summary>
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ApplicationDbContext _context;

        public UsuarioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<Usuario?> ObtenerPorNombreOCorreoAsync(string identificador)
        {
            return _context.Usuarios
                .Include(u => u.ListaParaVer)
                .FirstOrDefaultAsync(u => u.NombreUsuario == identificador || u.Correo == identificador);
        }

        public Task<Usuario?> ObtenerPorIdAsync(int id)
        {
            return _context.Usuarios
                .Include(u => u.ListaParaVer)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public Task<List<Usuario>> BuscarPorNombreAsync(string texto)
        {
            return _context.Usuarios
                .Where(u => EF.Functions.Like(u.NombreUsuario, $"%{texto}%"))
                .OrderBy(u => u.NombreUsuario)
                .Take(20)
                .ToListAsync();
        }

        public async Task AgregarAsync(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
        }

        public async Task<List<ContenidoAudiovisual>> ObtenerParaVerAsync(int usuarioId)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.ListaParaVer)
                .FirstOrDefaultAsync(u => u.Id == usuarioId);

            return usuario?.ListaParaVer ?? new List<ContenidoAudiovisual>();
        }

        public async Task AgregarAParaVerAsync(int usuarioId, int contenidoId)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.ListaParaVer)
                .FirstOrDefaultAsync(u => u.Id == usuarioId);

            if (usuario is null || usuario.ListaParaVer.Any(c => c.Id == contenidoId))
            {
                return;
            }

            var contenido = await _context.Contenidos.FindAsync(contenidoId);
            if (contenido is null)
            {
                return;
            }

            usuario.ListaParaVer.Add(contenido);
        }

        public async Task QuitarDeParaVerAsync(int usuarioId, int contenidoId)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.ListaParaVer)
                .FirstOrDefaultAsync(u => u.Id == usuarioId);

            var contenido = usuario?.ListaParaVer.FirstOrDefault(c => c.Id == contenidoId);
            if (usuario is null || contenido is null)
            {
                return;
            }

            usuario.ListaParaVer.Remove(contenido);
        }

        public Task GuardarCambiosAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
