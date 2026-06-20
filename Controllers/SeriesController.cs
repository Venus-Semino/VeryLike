using Microsoft.AspNetCore.Mvc;
using VeryLike.Domain.Interfaces;
using VeryLike.Domain.Models;

namespace VeryLike.Web.Controllers
{
    public class SeriesController : Controller
    {
        private readonly ISerieRepository _serieRepository;

        public SeriesController(ISerieRepository serieRepository)
        {
            _serieRepository = serieRepository;
        }

        public IActionResult Index()
        {
            var series = _serieRepository.ObtenerTodas();
            return View(series);
        }

        [HttpPost]
        public IActionResult Registrar(Serie nuevaSerie)
        {
            if (ModelState.IsValid)
            {
                _serieRepository.Agregar(nuevaSerie);
                return RedirectToAction("Index");
            }

            var series = _serieRepository.ObtenerTodas();
            return View("Index", series);
        }
    }
}