using VeryLike.Domain.DTOs;

namespace VeryLike.Domain.Interfaces
{
    /// <summary>
    /// Puerto hacia una API abierta de cine (OMDb / TMDB). La implementación
    /// concreta (VeryLike.Infrastructure.ExternalServices.TmdbCatalogoExternoService)
    /// usa HttpClient y no requiere intervención humana: dado un título o un
    /// id, resuelve automáticamente sinopsis, año, estudio, calificación y
    /// póster.
    /// </summary>
    public interface ICatalogoExternoService
    {
        /// <summary>Busca un título por nombre. Devuelve null si no hay coincidencias.</summary>
        Task<ContenidoExternoDto?> BuscarPorTituloAsync(string titulo);

        /// <summary>Busca un título por su id en la API externa.</summary>
        Task<ContenidoExternoDto?> BuscarPorIdExternoAsync(string idExterno, string tipo);

        /// <summary>Trae títulos populares (mezcla de películas y series) para poblar/sincronizar el catálogo.</summary>
        Task<List<ContenidoExternoDto>> BuscarPopularesAsync(int cantidad = 20);
    }
}
