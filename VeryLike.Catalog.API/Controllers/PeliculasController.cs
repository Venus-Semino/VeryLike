using Microsoft.AspNetCore.Mvc;
using VeryLike.Domain.Interfaces;
using VeryLike.Domain.Models;

namespace VeryLike.Catalog.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PeliculasController : ControllerBase
    {
        private readonly IPeliculaRepository _peliculaRepository;

        // El framework inyecta la infraestructura automáticamente aquí gracias al Program.cs
        public PeliculasController(IPeliculaRepository peliculaRepository)
        {
            _peliculaRepository = peliculaRepository;
        }

        // GET: api/peliculas
        [HttpGet]
        public ActionResult<IEnumerable<Pelicula>> Get()
        {
            var peliculas = _peliculaRepository.ObtenerTodas();
            return Ok(peliculas);
        }

        // POST: api/peliculas
        [HttpPost]
        public ActionResult Post([FromBody] Pelicula pelicula)
        {
            _peliculaRepository.Agregar(pelicula);
            return Ok(new { mensaje = "Película registrada exitosamente en el catálogo." });
        }
    }
}