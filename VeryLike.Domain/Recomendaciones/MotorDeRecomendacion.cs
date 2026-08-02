using VeryLike.Domain.Models;

namespace VeryLike.Domain.Recomendaciones
{
    /// <summary>Contexto del patrón Strategy: delega el cálculo a la estrategia inyectada.</summary>
    public class MotorDeRecomendacion
    {
        private readonly IEstrategiaRecomendacion _estrategia;

        public MotorDeRecomendacion(IEstrategiaRecomendacion estrategia)
        {
            _estrategia = estrategia ?? throw new ArgumentNullException(nameof(estrategia));
        }

        public List<ContenidoAudiovisual> Recomendar(PerfilDeGustos perfil, List<ContenidoAudiovisual> catalogo) =>
            _estrategia.Recomendar(perfil, catalogo);
    }
}
