
using System.Security.Cryptography;
using System.Text;
using Application.Interfaces.Services.Pans;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.PanServices
{
    public class PanEncryptor : IPanEncryptor
    {
        private readonly byte[] _key;
        public PanEncryptor(byte[] key)
        {
            _key = key;
        }
        public (byte[] CipherText, byte[] Nonce, byte[] Tag) Encrypt(string pan)
        {
            byte[] nonce = RandomNumberGenerator.GetBytes(12);
            byte[] plainText = Encoding.UTF8.GetBytes(pan);
            byte[] cipherText = new byte[plainText.Length];
            byte[] tag = new byte[16];

            using var aes = new AesGcm(_key, 16);
            aes.Encrypt(nonce, plainText, cipherText, tag);

            return (cipherText, nonce, tag);
        }
        public string Decrypt(byte[] CipherText, byte[] Nonce, byte[] Tag)
        {
            var plaintext = new byte[CipherText.Length];
            using var aes = new AesGcm (_key, 16);
            aes.Decrypt(Nonce, CipherText, Tag, plaintext);
            return Encoding.UTF8.GetString(plaintext);
        }
    }
}
