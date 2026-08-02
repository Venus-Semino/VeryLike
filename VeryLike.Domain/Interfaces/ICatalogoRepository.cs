using VeryLike.Domain.Models;

namespace VeryLike.Domain.Interfaces
{
    /// <summary>
    /// Abstracción de acceso al catálogo. La implementación concreta vive en
    /// VeryLike.Infrastructure y usa ApplicationDbContext (EF Core) de forma
    /// totalmente asíncrona; ya no hay lectura/escritura de peliculas.json.
    /// </summary>
    public interface ICatalogoRepository
    {
        Task<List<ContenidoAudiovisual>> ObtenerTodoAsync();
        Task<List<Pelicula>> ObtenerPeliculasAsync();
        Task<List<Serie>> ObtenerSeriesAsync();
        Task<ContenidoAudiovisual?> ObtenerPorIdAsync(int id);

        /// <summary>Géneros distintos presentes en el catálogo, para poblar el filtro de la vista.</summary>
        Task<List<string>> ObtenerGenerosAsync();

        Task<bool> ExisteIdExternoAsync(string idExterno);

        Task AgregarAsync(ContenidoAudiovisual contenido);

        Task GuardarCambiosAsync();

    }
}
