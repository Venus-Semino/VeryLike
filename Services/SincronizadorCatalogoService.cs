using VeryLike.Domain.Factories;
using VeryLike.Domain.Interfaces;

namespace VeryLike.Web.Services
{
    /// <summary>
    /// Puebla el catálogo local con los títulos populares de la API externa
    /// (TMDB). Usa ContenidoFactory para decidir si cada DTO es Película o
    /// Serie y deduplica por IdExterno, así que se puede ejecutar las veces
    /// que haga falta sin repetir títulos.
    /// </summary>
    public class SincronizadorCatalogoService
    {
        private readonly ICatalogoExternoService _catalogoExterno;
        private readonly ICatalogoRepository _catalogoRepository;
        private readonly ContenidoFactory _factory;
        private readonly ILogger<SincronizadorCatalogoService> _logger;

        public SincronizadorCatalogoService(
            ICatalogoExternoService catalogoExterno,
            ICatalogoRepository catalogoRepository,
            ContenidoFactory factory,
            ILogger<SincronizadorCatalogoService> logger)
        {
            _catalogoExterno = catalogoExterno;
            _catalogoRepository = catalogoRepository;
            _factory = factory;
            _logger = logger;
        }

        /// <summary>Devuelve cuántos títulos nuevos se agregaron.</summary>
        public async Task<int> SincronizarPopularesAsync(int cantidad = 40)
        {
            var externos = await _catalogoExterno.BuscarPopularesAsync(cantidad);
            var agregados = 0;

            foreach (var dto in externos)
            {
                if (string.IsNullOrWhiteSpace(dto.IdExterno) ||
                    await _catalogoRepository.ExisteIdExternoAsync(dto.IdExterno))
                {
                    continue;
                }

                try
                {
                    await _catalogoRepository.AgregarAsync(_factory.Crear(dto));
                    agregados++;
                }
                catch (NotSupportedException ex)
                {
                    _logger.LogWarning(ex, "Título omitido por tipo desconocido: {Titulo}", dto.Nombre);
                }
            }

            if (agregados > 0)
            {
                await _catalogoRepository.GuardarCambiosAsync();
            }

            return agregados;
        }
    }
}
