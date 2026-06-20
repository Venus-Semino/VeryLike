using Microsoft.AspNetCore.Mvc;
using VeryLike.Domain.Interfaces;
using VeryLike.Domain.Models;

namespace VeryLike.Web.Controllers
{
    public class PeliculasController : Controller
    {
        private readonly IPeliculaRepository _peliculaRepository;

        // El framework inyecta el repositorio automáticamente aquí
        public PeliculasController(IPeliculaRepository peliculaRepository)
        {
            _peliculaRepository = peliculaRepository;
        }

        public IActionResult Index()
        {
            var peliculas = _peliculaRepository.ObtenerTodas();
            return View(peliculas);
        }

        [HttpPost]
        public IActionResult Registrar(Pelicula nuevaPelicula)
        {
            if (ModelState.IsValid)
            {
                _peliculaRepository.Agregar(nuevaPelicula);
                return RedirectToAction("Index");
            }

            var peliculas = _peliculaRepository.ObtenerTodas();
            return View("Index", peliculas);
        }
    }
}