using VeryLike.Domain.Models;

namespace VeryLike.Domain.Recomendaciones
{
    /// <summary>
    /// Motor de recomendación "content-based filtering", determinista y explicable:
    ///   1. Arma un perfil de gustos a partir de las calificaciones del usuario
    ///      (3 es neutro: por encima suma afínidad al género, por debajo la resta)
    ///      y, con menos peso, de su lista "Para Ver".
    ///   2. Puntea cada título del catálogo según esa afinidad por género.
    ///   3. Combina esa afinidad con la calificación de la comunidad.
    ///
    /// Comparte la interfaz IEstrategiaRecomendacion con OrdenarPorCalificacionStrategy,
    /// así que puede sustituirse por un servicio externo de IA sin tocar ni el
    /// motor ni los controladores.
    /// </summary>
    public class RecomendacionInteligenteIaStrategy : IEstrategiaRecomendacion
    {
        private const double PesoAfinidadGenero = 0.7;
        private const double PesoCalificacionComunidad = 0.3;

        public string Nombre => "Recomendación inteligente (IA)";

        public List<ContenidoAudiovisual> Recomendar(PerfilDeGustos perfil, List<ContenidoAudiovisual> catalogo)
        {
            ArgumentNullException.ThrowIfNull(perfil);
            ArgumentNullException.ThrowIfNull(catalogo);

            // Sin historial no hay señal para personalizar: cae al orden por calificación.
            if (perfil.SinHistorial)
            {
                return catalogo.OrderByDescending(c => c.Calificacion).ToList();
            }

            var afinidadPorGenero = perfil.CalcularAfinidadPorGenero();

            // Lo que ya vio y calificó, o lo que ya tiene anotado, no se sugiere de nuevo.
            var yaConocidos = perfil.Calificaciones.Select(c => c.ContenidoId).ToHashSet();
            foreach (var contenido in perfil.Usuario?.ListaParaVer ?? new List<ContenidoAudiovisual>())
            {
                yaConocidos.Add(contenido.Id);
            }

            return catalogo
                .Where(c => !yaConocidos.Contains(c.Id))
                .Select(c => new
                {
                    Contenido = c,
                    Puntaje = CalcularPuntajeAfinidad(c, afinidadPorGenero)
                })
                .OrderByDescending(x => x.Puntaje)
                .ThenByDescending(x => x.Contenido.Calificacion)
                .Select(x => x.Contenido)
                .ToList();
        }

        private static double CalcularPuntajeAfinidad(
            ContenidoAudiovisual contenido,
            Dictionary<string, double> afinidadPorGenero)
        {
            var puntajeGenero = contenido.Genero.Sum(afinidadPorGenero.GetValueOrDefault);

            return (puntajeGenero * PesoAfinidadGenero) + (contenido.Calificacion * PesoCalificacionComunidad);
        }
    }
}
