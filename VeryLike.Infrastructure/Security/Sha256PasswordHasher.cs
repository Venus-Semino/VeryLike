using System.Security.Cryptography;
using VeryLike.Domain.Interfaces;

namespace VeryLike.Infrastructure.Security
{
    /// <summary>
    /// Hash de contraseñas con PBKDF2 (SHA-256) y sal aleatoria por usuario.
    /// El formato almacenado es "iteraciones.salBase64.hashBase64".
    /// </summary>
    public class Sha256PasswordHasher : IPasswordHasher
    {
        private const int Iteraciones = 210_000;
        private const int TamanioSal = 16;
        private const int TamanioHash = 32;

        public string Hash(string contrasenaPlano)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(contrasenaPlano);

            var sal = RandomNumberGenerator.GetBytes(TamanioSal);
            var hash = Derivar(contrasenaPlano, sal, Iteraciones);

            return $"{Iteraciones}.{Convert.ToBase64String(sal)}.{Convert.ToBase64String(hash)}";
        }

        public bool Verificar(string contrasenaPlano, string hashAlmacenado)
        {
            if (string.IsNullOrWhiteSpace(contrasenaPlano) || string.IsNullOrWhiteSpace(hashAlmacenado))
            {
                return false;
            }

            var partes = hashAlmacenado.Split('.');
            if (partes.Length != 3 || !int.TryParse(partes[0], out var iteraciones))
            {
                return false;
            }

            byte[] sal;
            byte[] esperado;
            try
            {
                sal = Convert.FromBase64String(partes[1]);
                esperado = Convert.FromBase64String(partes[2]);
            }
            catch (FormatException)
            {
                return false;
            }

            var calculado = Derivar(contrasenaPlano, sal, iteraciones, esperado.Length);
            return CryptographicOperations.FixedTimeEquals(calculado, esperado);
        }

        private static byte[] Derivar(string contrasena, byte[] sal, int iteraciones, int tamanio = TamanioHash)
        {
            return Rfc2898DeriveBytes.Pbkdf2(contrasena, sal, iteraciones, HashAlgorithmName.SHA256, tamanio);
        }
    }
}
