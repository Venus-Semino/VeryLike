using VeryLike.Domain.Models;

namespace VeryLike.Domain.Interfaces
{
    public interface IMensajeForoRepository
    {
        Task<List<MensajeForo>> ObtenerTodosAsync();
        Task AgregarAsync(MensajeForo mensaje);
        Task GuardarCambiosAsync();
    }
}
