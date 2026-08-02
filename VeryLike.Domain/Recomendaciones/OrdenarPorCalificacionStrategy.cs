using VeryLike.Domain.Models;

namespace VeryLike.Domain.Recomendaciones
{
    /// <summary>Estrategia determinista: simplemente ordena por calificación comunitaria descendente.</summary>
    public class OrdenarPorCalificacionStrategy : IEstrategiaRecomendacion
    {
        public string Nombre => "Mejor calificadas";

        public List<ContenidoAudiovisual> Recomendar(Usuario? usuario, List<ContenidoAudiovisual> catalogo)
        {
            ArgumentNullException.ThrowIfNull(catalogo);
            return catalogo.OrderByDescending(c => c.Calificacion).ToList();
        }
    }
}
