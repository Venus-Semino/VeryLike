using Microsoft.AspNetCore.Mvc;
using VeryLike.Domain.Interfaces;

namespace VeryLike.Catalog.API.Controllers
{
    /// <summary>
    /// Microservicio de catálogo. Esta es la única pieza del sistema que lee
    /// catalogo.json (vía ICatalogoRepository); VeryLike.Web ya NO lee el
    /// archivo directamente, le pide los datos a esta API por HTTP.
    /// </summary>
    [ApiController]
    [Route("api/catalogo")]
    public class CatalogoController : ControllerBase
    {
        private readonly ICatalogoRepository _catalogoRepository;

        public CatalogoController(ICatalogoRepository catalogoRepository)
        {
            _catalogoRepository = catalogoRepository;
        }

        [HttpGet]
        public IActionResult ObtenerTodo()
        {
            // Se castea a object para que System.Text.Json serialice cada
            // elemento con su tipo real (Pelicula o Serie), no solo con las
            // propiedades de la clase base abstracta ContenidoAudiovisual.
            var lista = _catalogoRepository.ObtenerTodo().Select(c => (object)c);
            return Ok(lista);
        }

        [HttpGet("peliculas")]
        public IActionResult ObtenerPeliculas() => Ok(_catalogoRepository.ObtenerPeliculas());

        [HttpGet("series")]
        public IActionResult ObtenerSeries() => Ok(_catalogoRepository.ObtenerSeries());

        [HttpGet("{id:int}")]
        public IActionResult ObtenerPorId(int id)
        {
            var item = _catalogoRepository.ObtenerPorId(id);
            return item is null ? NotFound() : Ok((object)item);
        }
    }
}
