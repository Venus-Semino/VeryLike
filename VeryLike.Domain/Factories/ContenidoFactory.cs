using VeryLike.Domain.DTOs;
using VeryLike.Domain.Models;

namespace VeryLike.Domain.Factories
{
    /// <summary>
    /// PATRÓN GoF: FACTORY METHOD.
    /// Punto único donde se decide, a partir del campo discriminador "Tipo"
    /// del DTO (venga de la API externa o de la base de datos), qué subclase
    /// concreta de ContenidoAudiovisual instanciar. Ni los controladores ni
    /// los repositorios necesitan conocer Pelicula/Serie directamente: solo
    /// hablan con esta fábrica y con el tipo base.
    /// </summary>
    public class ContenidoFactory
    {
        public ContenidoAudiovisual Crear(ContenidoExternoDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ContenidoAudiovisual contenido = NormalizarTipo(dto.Tipo) switch
            {
                "pelicula" => new Pelicula
                {
                    Duracion = string.IsNullOrWhiteSpace(dto.Duracion) ? "Desconocida" : dto.Duracion
                },
                "serie" => new Serie
                {
                    Temporadas = dto.Temporadas is > 0 ? dto.Temporadas.Value : 1
                },
                _ => throw new NotSupportedException(
                    $"Tipo de contenido no soportado: '{dto.Tipo}'. Se esperaba 'Pelicula' o 'Serie'.")
            };

            contenido.IdExterno = dto.IdExterno;
            contenido.Nombre = dto.Nombre;
            contenido.Genero = dto.Genero is { Count: > 0 } ? new List<string>(dto.Genero) : new List<string>();
            contenido.AnioPublicacion = dto.AnioPublicacion;
            contenido.PlataformaStreaming = dto.PlataformaStreaming ?? string.Empty;
            contenido.Sinopsis = dto.Sinopsis ?? string.Empty;
            contenido.Studio = dto.Studio ?? string.Empty;
            contenido.Calificacion = dto.Calificacion;
            contenido.PosterUrl = dto.PosterUrl;

            return contenido;
        }

        private static string NormalizarTipo(string? tipo) => tipo?.Trim().ToLowerInvariant() switch
        {
            "pelicula" or "película" or "movie" or "film" => "pelicula",
            "serie" or "series" or "tv" or "tv show" => "serie",
            _ => tipo?.Trim().ToLowerInvariant() ?? string.Empty
        };
    }
}
