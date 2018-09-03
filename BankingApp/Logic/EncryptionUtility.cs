using System.Security.Cryptography;
using System.Text;
using System;

namespace BankingApp.Logic
{
    public class EncryptionUtility
    {
        public string EncryptStringAndSalt(string source, string salt)
        {
            var sha256 = new SHA256Managed();
            var sha256Encrypted = sha256.ComputeHash(Encoding.UTF8.GetBytes(source + salt));
            return ConvertByteArrayToString(sha256Encrypted);
        }
        public string GenerateSalt()
        {
            var randomGenerator = new RNGCryptoServiceProvider();
            byte[] salt = new byte[16];
            randomGenerator.GetBytes(salt);
            randomGenerator.Dispose(); //RNGCrypto is not a managed resource        
            return ConvertByteArrayToString(salt);
        }

        private string ConvertByteArrayToString(byte[] hashBytes)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                stringBuilder.Append(hashBytes[i].ToString("X2"));
            }
            return stringBuilder.ToString();
        }
    }
}