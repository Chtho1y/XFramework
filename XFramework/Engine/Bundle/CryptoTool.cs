using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;


namespace XEngine.Engine
{
    public class CryptoTool
    {
        // Set up your key seed
        private static readonly byte[] defaultSalt = Encoding.UTF8.GetBytes("");
        private static readonly byte[] defaultPwd = Encoding.UTF8.GetBytes("");

        private static byte[] Encrypt(byte[] clearData, byte[] Key, byte[] IV)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var memoryStream = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(clearData, 0, clearData.Length);
                        cryptoStream.FlushFinalBlock();
                    }
                    return memoryStream.ToArray();
                }
            }
        }

        public static byte[] Encrypt(byte[] rawData)
        {
            try
            {
                var salt = GenerateSalt(16);
                using (var passwordDeriveBytes = new Rfc2898DeriveBytes(defaultPwd, salt, 10000))
                {
                    var key = passwordDeriveBytes.GetBytes(32);
                    var iv = GenerateIV();
                    var encryptedData = Encrypt(rawData, key, iv);
                    return salt.Concat(iv).Concat(encryptedData).ToArray();
                }
            }
            catch (Exception ex)
            {
                throw new CryptographicException("Encryption failed.", ex);
            }
        }

        private static byte[] Decrypt(byte[] cipherData, byte[] Key, byte[] IV)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var memoryStream = new MemoryStream(cipherData))
                using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    byte[] decryptedData = new byte[cipherData.Length];
                    int decryptedByteCount = cryptoStream.Read(decryptedData, 0, decryptedData.Length);
                    return decryptedData.Take(decryptedByteCount).ToArray();
                }
            }
        }

        public static byte[] Decrypt(byte[] cipherData)
        {
            try
            {
                if (cipherData.Length < 48)
                {
                    throw new CryptographicException("Cipher data is too short.");
                }

                var salt = cipherData.Take(16).ToArray();
                var iv = cipherData.Skip(16).Take(16).ToArray();
                var actualCipherData = cipherData.Skip(32).ToArray();

                using (var passwordDeriveBytes = new Rfc2898DeriveBytes(defaultPwd, salt, 10000))
                {
                    var key = passwordDeriveBytes.GetBytes(32);
                    return Decrypt(actualCipherData, key, iv);
                }
            }
            catch (Exception ex)
            {
                throw new CryptographicException("Decryption failed.", ex);
            }
        }

        private static byte[] GenerateSalt(int size)
        {
            var salt = new byte[size];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        private static byte[] GenerateIV()
        {
            var iv = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(iv);
            }
            return iv;
        }
    }
}