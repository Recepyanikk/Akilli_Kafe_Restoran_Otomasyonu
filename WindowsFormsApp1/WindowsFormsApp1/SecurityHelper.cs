using System;
using System.Security.Cryptography;
using System.Text;

namespace WindowsFormsApp1
{
    public static class SecurityHelper
    {
        /// <summary>
        /// Gelen açık metin şifreyi SHA-256 özetleme algoritması ile güvenli 64 karakter hexadecimal hash dizesine çevirir.
        /// </summary>
        public static string HashPassword(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return string.Empty;

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(plainText));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
