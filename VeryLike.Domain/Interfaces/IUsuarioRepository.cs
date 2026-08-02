using VeryLike.Domain.Models;

namespace VeryLike.Domain.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObtenerPorNombreOCorreoAsync(string identificador);
        Task<Usuario?> ObtenerPorIdAsync(int id);
        Task AgregarAsync(Usuario usuario);

        /// <summary>Usuarios cuyo nombre contiene el texto buscado, para el buscador global.</summary>
        Task<List<Usuario>> BuscarPorNombreAsync(string texto);

        Task<List<ContenidoAudiovisual>> ObtenerParaVerAsync(int usuarioId);
        Task AgregarAParaVerAsync(int usuarioId, int contenidoId);
        Task QuitarDeParaVerAsync(int usuarioId, int contenidoId);

        Task GuardarCambiosAsync();
    }
}
