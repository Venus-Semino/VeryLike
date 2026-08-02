namespace VeryLike.Domain.Interfaces
{
    /// <summary>
    /// Abstrae el hashing de contraseñas para que Usuario.Contrasena nunca se
    /// compare ni se guarde en texto plano (a diferencia de la versión anterior).
    /// </summary>
    public interface IPasswordHasher
    {
        string Hash(string contrasenaPlano);
        bool Verificar(string contrasenaPlano, string hashAlmacenado);
    }
}
