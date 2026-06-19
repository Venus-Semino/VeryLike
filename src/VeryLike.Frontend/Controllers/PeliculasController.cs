using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using VeryLike.Frontend.Models;

namespace VeryLike.Frontend.Controllers
{
    public class PeliculasController : Controller
    {
        // Ubicación física del archivo dentro de la carpeta wwwroot/data/
        private readonly string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "peliculas.json");

        // Método auxiliar para leer el archivo JSON y convertirlo en una lista de C#
        private List<Pelicula> ObtenerPeliculasDelArchivo()
        {
            var directorio = Path.GetDirectoryName(_filePath);
            if (directorio != null && !Directory.Exists(directorio))
            {
                Directory.CreateDirectory(directorio); // Crea la carpeta si no existe
            }

            // CORRECCIÓN: Usamos System.IO.File para evitar confusiones con el método del Controller
            if (!System.IO.File.Exists(_filePath))
            {
                return new List<Pelicula>();
            }

            var jsonContent = System.IO.File.ReadAllText(_filePath);
            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                return new List<Pelicula>();
            }

            try
            {
                // Deserialización: Convierte el texto plano JSON a objetos de C#
                return JsonSerializer.Deserialize<List<Pelicula>>(jsonContent) ?? new List<Pelicula>();
            }
            catch
            {
                return new List<Pelicula>();
            }
        }

        // 1. Cargar la página principal con el catálogo actual
        public IActionResult Index()
        {
            var listaPeliculas = ObtenerPeliculasDelArchivo();
            return View(listaPeliculas);
        }

        // 2. Procesar el formulario enviado por el usuario
        [HttpPost]
        public IActionResult Registrar(Pelicula nuevaPelicula)
        {
            if (ModelState.IsValid)
            {
                var listaPeliculas = ObtenerPeliculasDelArchivo();
                listaPeliculas.Add(nuevaPelicula); // Agrega el nuevo registro a la lista

                // Serialización: Convierte la lista de C# a texto plano ordenado
                var opciones = new JsonSerializerOptions { WriteIndented = true };
                var jsonString = JsonSerializer.Serialize(listaPeliculas, opciones);

                // CORRECCIÓN: Usamos System.IO.File
                System.IO.File.WriteAllText(_filePath, jsonString); // Sobrescribe el archivo con los datos actualizados

                return RedirectToAction("Index"); // Recarga la página para mostrar los cambios
            }

            var listaActual = ObtenerPeliculasDelArchivo();
            return View("Index", listaActual);
        }
    }
}