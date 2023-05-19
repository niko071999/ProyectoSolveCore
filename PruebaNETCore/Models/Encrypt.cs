﻿using System.Security.Cryptography;
using System.Text;

namespace ProyectoSolveCore.Models
{
    public class Encrypt
    {
        public static string EncryptPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < hash.Length; i++)
                {
                    builder.Append(hash[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }

        public static bool VerifyPassword(string password, string encryptedPassword)
        {
            string encryptedInput = EncryptPassword(password);
            return encryptedInput.Equals(encryptedPassword);
        }
    }
}
