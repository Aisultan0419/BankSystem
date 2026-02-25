using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services.Pans
{
    public interface IPanEncryptor
    {
        (byte[] CipherText, byte[] Nonce, byte[] Tag) Encrypt(string pan);
        string Decrypt(byte[] cipherText, byte[] nonce, byte[] tag);
    }
}
