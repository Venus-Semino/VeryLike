using Microsoft.AspNetCore.Mvc;
using VeryLike.Domain.Interfaces;
using VeryLike.Domain.Models;

namespace VeryLike.Catalog.API.Controllers
{
    /// <summary>Rutas que consume VeryLike.Web a través de CatalogoApiClient.</summary>
    [ApiController]
    [Route("api/catalogo")]
    public class CatalogoController : ControllerBase
    {
        private readonly ICatalogoRepository _catalogoRepository;

        public CatalogoController(ICatalogoRepository catalogoRepository)
        {
            _catalogoRepository = catalogoRepository;
        }

        [HttpGet("peliculas")]
        public async Task<ActionResult<List<Pelicula>>> ObtenerPeliculas()
        {
            return await _catalogoRepository.ObtenerPeliculasAsync();
        }

        [HttpGet("series")]
        public async Task<ActionResult<List<Serie>>> ObtenerSeries()
        {
            return await _catalogoRepository.ObtenerSeriesAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ContenidoAudiovisual>> ObtenerPorId(int id)
        {
            var contenido = await _catalogoRepository.ObtenerPorIdAsync(id);
            return contenido is null ? NotFound() : Ok(contenido);
        }

        [HttpGet("generos")]
        public async Task<ActionResult<List<string>>> ObtenerGeneros()
        {
            return await _catalogoRepository.ObtenerGenerosAsync();
        }
    }
}
