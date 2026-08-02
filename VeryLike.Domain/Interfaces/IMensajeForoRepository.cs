using VeryLike.Domain.Models;

namespace VeryLike.Domain.Interfaces
{
    public interface IMensajeForoRepository
    {
        /// <summary>Publicaciones raíz con sus comentarios, de la más reciente a la más antigua.</summary>
        Task<List<MensajeForo>> ObtenerTodosAsync();

        /// <summary>Publicaciones que llevan la etiqueta indicada.</summary>
        Task<List<MensajeForo>> ObtenerPorHashtagAsync(string hashtag);

        Task<MensajeForo?> ObtenerPorIdAsync(int id);

        Task AgregarAsync(MensajeForo mensaje);
        Task GuardarCambiosAsync();
    }
}
