using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Database
{
    public class PasswordCipher(byte[] iv) : ICipher
    {
        public PasswordCipher(string key)
            : this(Encoding.UTF8.GetBytes(key))
        {
        }

        public byte[] Decrypt(byte[] input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = (byte)(input[i] ^ iv[i % iv.Length]);
            }
            return input;
        }



        public byte[] Encrypt(byte[] input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = (byte)(input[i] ^ iv[i % iv.Length]);
            }
            return input;
        }

        public byte[] Signature()
        {
            return MD5.HashData(iv);
        }

        public void Dispose()
        {
        }
    }
}
