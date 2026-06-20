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

        public PeliculasController(IPeliculaRepository peliculaRepository)
        {
            _peliculaRepository = peliculaRepository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Pelicula>> Get() 
        {
            return Ok(_peliculaRepository.ObtenerTodas());
        }

        [HttpPost]
        public ActionResult Post([FromBody] Pelicula pelicula) 
        {
            _peliculaRepository.Agregar(pelicula);
            return Ok();
        }
    }
}