using System.Security.Cryptography;
using System.Text;

namespace ProyectoSolveCore.Models
{
    /// <summary>
    /// Clase que provee métodos para encriptar y verificar contraseña usando el algoritmo SHA256.
    /// </summary>
    public class Encrypt
    {
        /// <summary>
        /// Encripta una contraseña usando el algoritmo SHA256 y devuelve el resultado en formato hexadecimal.
        /// </summary>
        /// <param name="password">La contraseña a encriptar.</param>
        /// <returns>La contraseña encriptada en formato hexadecimal.</returns>
        public static string EncryptPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                StringBuilder builder = new();
                // Recorre el arreglo de bytes del hash y los convierte a su representación hexadecimal.
                for (int i = 0; i < hash.Length; i++)
                {
                    builder.Append(hash[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        /// <summary>
        /// Verifica si la contraseña coincide con una contraseña encriptada usando el algoritmo SHA256.
        /// </summary>
        /// <param name="password">La contraseña a verificar.</param>
        /// <param name="encryptedPassword">La contraseña encriptada con la que se compara.</param>
        /// <returns>Verdadero si las contraseñas coinciden, falso si no.</returns>
        public static bool VerifyPassword(string password, string encryptedPassword)
        {
            return string.Equals(encryptedPassword, EncryptPassword(password));
        }
    }
}
