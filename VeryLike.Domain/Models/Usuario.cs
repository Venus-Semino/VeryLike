using System.ComponentModel.DataAnnotations;

namespace VeryLike.Domain.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Ingresa un correo válido.")]
        [MaxLength(150)]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "El nombre de usuario debe tener entre 3 y 30 caracteres.")]
        public string NombreUsuario { get; set; } = string.Empty;

        /// <summary>
        /// Se guarda como hash (ver IPasswordHasher / Sha256PasswordHasher en
        /// VeryLike.Infrastructure), nunca en texto plano.
        /// </summary>
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        public string Contrasena { get; set; } = string.Empty;

        /// <summary>
        /// Lista "Para Ver" del usuario. Se mapea como relación muchos-a-muchos
        /// con ContenidoAudiovisual (tabla puente "UsuariosParaVer"), en vez del
        /// arreglo de IDs sueltos que se guardaba antes en usuarios.json.
        /// </summary>
        public List<ContenidoAudiovisual> ListaParaVer { get; set; } = new();
    }
}
