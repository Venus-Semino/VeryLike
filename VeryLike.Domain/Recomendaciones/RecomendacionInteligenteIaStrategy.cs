using VeryLike.Domain.Models;

namespace VeryLike.Domain.Recomendaciones
{
    /// <summary>
    /// Simulación profesional de un motor de recomendación con IA.
    ///
    /// No llama a un modelo externo de pago: en su lugar reproduce, de forma
    /// determinista y explicable, el comportamiento que tendría un motor real
    /// de "content-based filtering":
    ///   1. Extrae los géneros más frecuentes en la lista "Para Ver" del
    ///      usuario (su señal de interés más reciente/explícita).
    ///   2. Pondera cada título del catálogo según cuánto coincide con esos
    ///      géneros favoritos (más peso a los géneros más repetidos).
    ///   3. Combina esa afinidad con la calificación comunitaria del título.
    ///
    /// Esta clase implementa la misma interfaz IEstrategiaRecomendacion que
    /// OrdenarPorCalificacionStrategy, así que en el futuro puede sustituirse
    /// por una llamada real a un servicio de IA (ej. un endpoint de
    /// embeddings) sin tocar ni el motor ni los controladores.
    /// </summary>
    public class RecomendacionInteligenteIaStrategy : IEstrategiaRecomendacion
    {
        private const double PesoAfinidadGenero = 0.7;
        private const double PesoCalificacionComunidad = 0.3;

        public string Nombre => "Recomendación inteligente (IA)";

        public List<ContenidoAudiovisual> Recomendar(Usuario? usuario, List<ContenidoAudiovisual> catalogo)
        {
            ArgumentNullException.ThrowIfNull(catalogo);

            // Sin usuario o sin historial: no hay señal suficiente para
            // personalizar, así que cae de forma segura al orden por calificación.
            if (usuario is null || usuario.ListaParaVer.Count == 0)
            {
                return catalogo.OrderByDescending(c => c.Calificacion).ToList();
            }

            var generosFavoritos = ExtraerGenerosFavoritos(usuario);
            var idsYaEnListaParaVer = usuario.ListaParaVer.Select(c => c.Id).ToHashSet();

            return catalogo
                .Where(c => !idsYaEnListaParaVer.Contains(c.Id))
                .Select(c => new
                {
                    Contenido = c,
                    Puntaje = CalcularPuntajeAfinidad(c, generosFavoritos)
                })
                .OrderByDescending(x => x.Puntaje)
                .ThenByDescending(x => x.Contenido.Calificacion)
                .Select(x => x.Contenido)
                .ToList();
        }

        /// <summary>Géneros de la lista "Para Ver", ordenados del más repetido al menos repetido.</summary>
        private static List<string> ExtraerGenerosFavoritos(Usuario usuario) =>
            usuario.ListaParaVer
                .SelectMany(c => c.Genero)
                .GroupBy(g => g, StringComparer.OrdinalIgnoreCase)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .ToList();

        private static double CalcularPuntajeAfinidad(ContenidoAudiovisual contenido, List<string> generosFavoritos)
        {
            double puntajeGenero = 0;

            for (int i = 0; i < generosFavoritos.Count; i++)
            {
                if (contenido.Genero.Contains(generosFavoritos[i], StringComparer.OrdinalIgnoreCase))
                {
                    // Los géneros que más se repiten en el historial pesan más.
                    puntajeGenero += generosFavoritos.Count - i;
                }
            }

            return (puntajeGenero * PesoAfinidadGenero) + (contenido.Calificacion * PesoCalificacionComunidad);
        }
    }
}
