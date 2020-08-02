using System.IO;
using System.Security.Cryptography;

namespace Functions.Crypto
{
    public static class CryptoFns
    {
        /// <remarks>
        /// RFC 3394 Key unwrapping (thanks to RFC3394 Key Wrapping Algorithm written by Jay Miller)
        /// </remarks>
        public static byte[] DecryptKey(byte[] key, byte[] iv, byte[] cipherText)//MemoryStream inputStream)
        {
            var decryptedData = KeyWrapAlgorithm.UnwrapKey(key, cipherText, iv);
            return decryptedData;
        }

        /// <remarks>
        /// Regular AES (256 bit) CBC mode decryption
        /// </remarks>
        public static byte[] Decrypt(byte[] key, byte[] iv, byte[] cipherText)
        {
            var a = new AesManaged();
            a.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = a.CreateDecryptor(key, iv);
            var cipherStream = new MemoryStream(cipherText);
            cipherStream.Position = 0L;
            var outputBytes = new byte[cipherText.Length];

            using (var cryptoStream = new CryptoStream(cipherStream, decryptor, CryptoStreamMode.Read))
            {
                cryptoStream.Read(outputBytes, 0, cipherText.Length);
                return outputBytes;
            }
        }
    }
}