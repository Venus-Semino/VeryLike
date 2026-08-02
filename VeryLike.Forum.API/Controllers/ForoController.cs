using Microsoft.AspNetCore.Mvc;
using VeryLike.Domain.Interfaces;
using VeryLike.Domain.Models;

namespace VeryLike.Forum.API.Controllers
{
    /// <summary>Rutas que consume VeryLike.Web a través de ForoApiClient.</summary>
    [ApiController]
    [Route("api/foro")]
    public class ForoController : ControllerBase
    {
        private readonly IMensajeForoRepository _foroRepository;

        public ForoController(IMensajeForoRepository foroRepository)
        {
            _foroRepository = foroRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<MensajeForo>>> ObtenerTodos()
        {
            return await _foroRepository.ObtenerTodosAsync();
        }

        [HttpGet("hashtag/{hashtag}")]
        public async Task<ActionResult<List<MensajeForo>>> ObtenerPorHashtag(string hashtag)
        {
            return await _foroRepository.ObtenerPorHashtagAsync(hashtag);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<MensajeForo>> ObtenerPorId(int id)
        {
            var mensaje = await _foroRepository.ObtenerPorIdAsync(id);
            return mensaje is null ? NotFound() : Ok(mensaje);
        }

        [HttpPost]
        public async Task<ActionResult<MensajeForo>> Publicar([FromBody] MensajeForo mensaje)
        {
            if (string.IsNullOrWhiteSpace(mensaje.Contenido) || string.IsNullOrWhiteSpace(mensaje.NombreUsuario))
            {
                return BadRequest("El mensaje necesita autor y contenido.");
            }

            await _foroRepository.AgregarAsync(mensaje);
            await _foroRepository.GuardarCambiosAsync();

            return CreatedAtAction(nameof(ObtenerPorId), new { id = mensaje.Id }, mensaje);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var mensaje = await _foroRepository.ObtenerPorIdAsync(id);
            if (mensaje is null)
            {
                return NotFound();
            }

            await _foroRepository.EliminarAsync(mensaje);
            await _foroRepository.GuardarCambiosAsync();
            return NoContent();
        }
    }
}
