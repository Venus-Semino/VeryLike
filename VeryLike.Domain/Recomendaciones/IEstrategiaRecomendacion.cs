using VeryLike.Domain.Models;

namespace VeryLike.Domain.Recomendaciones
{
    /// <summary>
    /// PATRÓN GoF: STRATEGY.
    /// Contrato común para cualquier algoritmo de recomendación. El perfil
    /// puede ser <see cref="PerfilDeGustos.Anonimo"/> (invitado sin sesión):
    /// cada estrategia decide cómo degradar de forma segura en ese caso.
    /// </summary>
    public interface IEstrategiaRecomendacion
    {
        /// <summary>Nombre corto para mostrar en la UI (ej. selector "Modo de recomendación").</summary>
        string Nombre { get; }

        List<ContenidoAudiovisual> Recomendar(PerfilDeGustos perfil, List<ContenidoAudiovisual> catalogo);
    }
}
