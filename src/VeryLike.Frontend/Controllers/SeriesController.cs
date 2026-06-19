using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using VeryLike.Frontend.Models;

namespace VeryLike.Frontend.Controllers
{
    public class SeriesController : Controller
    {
        // Ubicación física exclusiva para el archivo de series
        private readonly string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "series.json");

        private List<Serie> ObtenerSeriesDelArchivo()
        {
            var directorio = Path.GetDirectoryName(_filePath);
            if (directorio != null && !Directory.Exists(directorio))
            {
                Directory.CreateDirectory(directorio);
            }

            if (!System.IO.File.Exists(_filePath))
            {
                return new List<Serie>();
            }

            var jsonContent = System.IO.File.ReadAllText(_filePath);
            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                return new List<Serie>();
            }

            try
            {
                return JsonSerializer.Deserialize<List<Serie>>(jsonContent) ?? new List<Serie>();
            }
            catch
            {
                return new List<Serie>();
            }
        }

        // Carga la pantalla principal de Series
        public IActionResult Index()
        {
            var listaSeries = ObtenerSeriesDelArchivo();
            return View(listaSeries);
        }

        // Procesa el formulario de registro de Series
        [HttpPost]
        public IActionResult Registrar(Serie nuevaSerie)
        {
            if (ModelState.IsValid)
            {
                var listaSeries = ObtenerSeriesDelArchivo();
                listaSeries.Add(nuevaSerie);

                var opciones = new JsonSerializerOptions { WriteIndented = true };
                var jsonString = JsonSerializer.Serialize(listaSeries, opciones);

                System.IO.File.WriteAllText(_filePath, jsonString);

                return RedirectToAction("Index");
            }

            var listaActual = ObtenerSeriesDelArchivo();
            return View("Index", listaActual);
        }
    }
}