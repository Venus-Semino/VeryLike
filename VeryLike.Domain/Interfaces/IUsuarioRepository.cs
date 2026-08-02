using VeryLike.Domain.Models;

namespace VeryLike.Domain.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObtenerPorNombreOCorreoAsync(string identificador);
        Task<Usuario?> ObtenerPorIdAsync(int id);
        Task AgregarAsync(Usuario usuario);

        Task<List<ContenidoAudiovisual>> ObtenerParaVerAsync(int usuarioId);
        Task AgregarAParaVerAsync(int usuarioId, int contenidoId);
        Task QuitarDeParaVerAsync(int usuarioId, int contenidoId);

        Task GuardarCambiosAsync();
    }
}
