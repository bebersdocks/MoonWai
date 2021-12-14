using System.Security.Cryptography;

namespace MoonWai.Api.Utils
{
    public static class Crypto
    {
        public const int byteSize = 64;
        private const int iterations = 10000;

        public static byte[] GenerateHash(string password, byte[] salt)
        {
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
            var hash = pbkdf2.GetBytes(byteSize);

            return hash;
        }

        public static (byte[] salt, byte[] hash) GenerateSaltHash(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(byteSize);       
            var hash = GenerateHash(password, salt);

            return (salt, hash);
        }
        
        public static bool AreBytesEqual(byte[] arr1, byte[] arr2)
        {
            if (arr1 == null || arr2 == null)
                return false;

            if (arr1.Length != arr2.Length)
                return false;

            for (var i = 0; i < arr1.Length; i++)
                if (arr1[i] != arr2[i])
                    return false;

            return true;
        }

        public static bool ValidatePassword(string password, byte[] salt, byte[] hash)
        {
            return AreBytesEqual(hash, GenerateHash(password, salt));
        }
    }
}