namespace VeryLike.Web.Models
{
    /// <summary>Datos que el modal de detalle pide por AJAX al abrirse.</summary>
    public class DetalleCalificacionViewModel
    {
        public double Promedio { get; set; }
        public int Total { get; set; }
        public bool Autenticado { get; set; }
        public MiCalificacionViewModel? MiCalificacion { get; set; }
        public List<ResenaViewModel> Resenas { get; set; } = new();
    }

    public class MiCalificacionViewModel
    {
        public double Puntaje { get; set; }
        public string? ResenaPublica { get; set; }
        public string? ResenaPrivada { get; set; }
    }

    public class ResenaViewModel
    {
        public string Autor { get; set; } = string.Empty;
        public double Puntaje { get; set; }
        public string Texto { get; set; } = string.Empty;
    }

    /// <summary>Cuerpo del POST que envía el formulario del modal.</summary>
    public class GuardarCalificacionRequest
    {
        public int ContenidoId { get; set; }
        public double Puntaje { get; set; }
        public string? ResenaPublica { get; set; }
        public string? ResenaPrivada { get; set; }
    }
}
