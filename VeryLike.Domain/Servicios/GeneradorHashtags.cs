using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace VeryLike.Domain.Servicios
{
    /// <summary>
    /// Genera las etiquetas de una publicación del foro sin intervención del
    /// usuario: respeta los "#tag" que haya escrito, detecta títulos del
    /// catálogo mencionados en el texto y agrega temas reconocidos por
    /// palabras clave.
    /// </summary>
    public class GeneradorHashtags
    {
        private const int MaximoHashtags = 5;

        private static readonly Regex HashtagExplicito =
            new(@"#(\w{2,30})", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Dictionary<string, string[]> TemasPorPalabraClave = new()
        {
            ["Recomendacion"] = new[] { "recomiendo", "recomendacion", "recomendable", "miren", "vean" },
            ["Critica"] = new[] { "critica", "resena", "opinion", "analisis" },
            ["Spoiler"] = new[] { "spoiler", "final", "muere", "desenlace" },
            ["Terror"] = new[] { "terror", "miedo", "horror", "aterrador" },
            ["Animacion"] = new[] { "anime", "animacion", "animada", "ghibli" },
            ["CienciaFiccion"] = new[] { "ciencia ficcion", "scifi", "futurista", "distopia" },
            ["Drama"] = new[] { "drama", "dramatica", "lloré", "llore", "emotiva" },
            ["Comedia"] = new[] { "comedia", "gracioso", "divertida", "risa" },
            ["Serie"] = new[] { "temporada", "capitulo", "episodio" },
            ["Estreno"] = new[] { "estreno", "salio", "premiere", "recien" }
        };

        /// <param name="titulosCatalogo">Nombres del catálogo para detectar menciones a títulos concretos.</param>
        public List<string> Generar(string contenido, IEnumerable<string> titulosCatalogo)
        {
            if (string.IsNullOrWhiteSpace(contenido))
            {
                return new List<string>();
            }

            var etiquetas = new List<string>();
            var normalizado = Normalizar(contenido);

            foreach (Match coincidencia in HashtagExplicito.Matches(contenido))
            {
                Agregar(etiquetas, AFormatoEtiqueta(coincidencia.Groups[1].Value));
            }

            foreach (var titulo in titulosCatalogo)
            {
                if (!string.IsNullOrWhiteSpace(titulo) && normalizado.Contains(Normalizar(titulo)))
                {
                    Agregar(etiquetas, AFormatoEtiqueta(titulo));
                }
            }

            foreach (var (tema, palabras) in TemasPorPalabraClave)
            {
                if (palabras.Any(palabra => normalizado.Contains(Normalizar(palabra))))
                {
                    Agregar(etiquetas, tema);
                }
            }

            return etiquetas.Take(MaximoHashtags).ToList();
        }

        private static void Agregar(List<string> etiquetas, string etiqueta)
        {
            if (etiqueta.Length > 1 && !etiquetas.Contains(etiqueta, StringComparer.OrdinalIgnoreCase))
            {
                etiquetas.Add(etiqueta);
            }
        }

        /// <summary>"perfect blue" -> "PerfectBlue".</summary>
        private static string AFormatoEtiqueta(string texto)
        {
            var palabras = Normalizar(texto)
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => char.ToUpperInvariant(p[0]) + p[1..]);

            return string.Concat(palabras);
        }

        /// <summary>Minúsculas, sin acentos y sin signos, para comparar de forma tolerante.</summary>
        private static string Normalizar(string texto)
        {
            var descompuesto = texto.ToLowerInvariant().Normalize(NormalizationForm.FormD);
            var limpio = new StringBuilder();

            foreach (var caracter in descompuesto)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(caracter) == UnicodeCategory.NonSpacingMark)
                {
                    continue;
                }

                limpio.Append(char.IsLetterOrDigit(caracter) ? caracter : ' ');
            }

            return Regex.Replace(limpio.ToString(), @"\s+", " ").Trim();
        }
    }
}
