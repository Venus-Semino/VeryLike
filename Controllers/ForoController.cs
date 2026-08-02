using Microsoft.AspNetCore.Mvc;
using VeryLike.Domain.Interfaces;
using VeryLike.Domain.Models;
using VeryLike.Domain.Servicios;
using VeryLike.Web.Services;

namespace VeryLike.Web.Controllers
{
    public class ForoController : Controller
    {
        private const string ClaveSesion = "UsuarioNombre";

        private readonly IForoApiClient _foroApiClient;
        private readonly IMensajeForoRepository _foroRepository;
        private readonly ICatalogoRepository _catalogoRepository;
        private readonly GeneradorHashtags _generadorHashtags;

        public ForoController(
            IForoApiClient foroApiClient,
            IMensajeForoRepository foroRepository,
            ICatalogoRepository catalogoRepository,
            GeneradorHashtags generadorHashtags)
        {
            _foroApiClient = foroApiClient;
            _foroRepository = foroRepository;
            _catalogoRepository = catalogoRepository;
            _generadorHashtags = generadorHashtags;
        }

        public async Task<IActionResult> Index(string? hashtag)
        {
            var mensajes = string.IsNullOrWhiteSpace(hashtag)
                ? await _foroApiClient.ObtenerTodosAsync()
                : await _foroApiClient.ObtenerPorHashtagAsync(hashtag);

            ViewData["HashtagActivo"] = hashtag;
            return View(mensajes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Publicar(MensajeForo nuevoMensaje)
        {
            var autor = HttpContext.Session.GetString(ClaveSesion);
            if (string.IsNullOrEmpty(autor))
            {
                return RedirectToAction("Login", "Auth");
            }

            nuevoMensaje.NombreUsuario = autor;
            ModelState.Remove(nameof(MensajeForo.NombreUsuario));

            if (!ModelState.IsValid)
            {
                return View("Index", await _foroApiClient.ObtenerTodosAsync());
            }

            nuevoMensaje.Hashtags = await GenerarHashtagsAsync(nuevoMensaje.Contenido);
            await _foroApiClient.PublicarAsync(nuevoMensaje);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Comentar(int mensajePadreId, string contenido)
        {
            var autor = HttpContext.Session.GetString(ClaveSesion);
            if (string.IsNullOrEmpty(autor))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (string.IsNullOrWhiteSpace(contenido))
            {
                return RedirectToAction(nameof(Index));
            }

            var padre = await _foroRepository.ObtenerPorIdAsync(mensajePadreId);
            if (padre is null)
            {
                return NotFound();
            }

            await _foroRepository.AgregarAsync(new MensajeForo
            {
                NombreUsuario = autor,
                Contenido = contenido.Trim(),
                MensajePadreId = mensajePadreId,
                Hashtags = await GenerarHashtagsAsync(contenido)
            });
            await _foroRepository.GuardarCambiosAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task<List<string>> GenerarHashtagsAsync(string contenido)
        {
            var catalogo = await _catalogoRepository.ObtenerTodoAsync();
            return _generadorHashtags.Generar(contenido, catalogo.Select(c => c.Nombre));
        }
    }
}
