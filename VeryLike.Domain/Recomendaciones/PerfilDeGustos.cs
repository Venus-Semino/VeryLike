using VeryLike.Domain.Models;

namespace VeryLike.Domain.Recomendaciones
{
    /// <summary>
    /// Señales del usuario que alimentan a las estrategias: sus calificaciones
    /// (señal fuerte, porque dicen si le gustó o no) y su lista "Para Ver"
    /// (señal débil de interés). Un invitado se representa con <see cref="Anonimo"/>.
    /// </summary>
    public class PerfilDeGustos
    {
        public static readonly PerfilDeGustos Anonimo = new(null, new List<Calificacion>());

        public PerfilDeGustos(Usuario? usuario, List<Calificacion> calificaciones)
        {
            Usuario = usuario;
            Calificaciones = calificaciones;
        }

        public Usuario? Usuario { get; }

        public List<Calificacion> Calificaciones { get; }

        public bool SinHistorial =>
            Usuario is null || (Calificaciones.Count == 0 && Usuario.ListaParaVer.Count == 0);

        /// <summary>Puntaje por género: positivo si lo calificó alto, negativo si lo calificó bajo.</summary>
        public Dictionary<string, double> CalcularAfinidadPorGenero()
        {
            var afinidad = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

            foreach (var calificacion in Calificaciones)
            {
                if (calificacion.Contenido is null)
                {
                    continue;
                }

                // 3 es el punto neutro: por encima suma, por debajo resta.
                var peso = calificacion.Puntaje - 3;
                foreach (var genero in calificacion.Contenido.Genero)
                {
                    afinidad[genero] = afinidad.GetValueOrDefault(genero) + peso;
                }
            }

            foreach (var contenido in Usuario?.ListaParaVer ?? new List<ContenidoAudiovisual>())
            {
                foreach (var genero in contenido.Genero)
                {
                    afinidad[genero] = afinidad.GetValueOrDefault(genero) + PesoListaParaVer;
                }
            }

            return afinidad;
        }

        /// <summary>Géneros que mejor calificó, para explicarle al usuario de dónde salen las sugerencias.</summary>
        public List<string> GenerosFavoritos(int cantidad)
        {
            return CalcularAfinidadPorGenero()
                .Where(par => par.Value > 0)
                .OrderByDescending(par => par.Value)
                .Take(cantidad)
                .Select(par => par.Key)
                .ToList();
        }

        private const double PesoListaParaVer = 0.5;
    }
}
