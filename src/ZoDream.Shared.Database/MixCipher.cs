using System;
using System.Security.Cryptography;

namespace ZoDream.Shared.Database
{
    public class MixCipher(params ICipher[] items) : ICipher
    {
        public byte[] Decrypt(byte[] input)
        {
            foreach (var item in items)
            {
                input = item.Decrypt(input);
            }
            return input;
        }

        public byte[] Encrypt(byte[] input)
        {
            foreach (var item in items)
            {
                input = item.Encrypt(input);
            }
            return input;
        }

        public byte[] Signature()
        {
            var buffer = new byte[32 * items.Length];
            for (var i = 0; i < items.Length; i++)
            {
                Array.Copy(items[i].Signature(), 0, buffer, i * 32, 32);
            }
            return MD5.HashData(Encrypt(buffer));
        }

        public void Dispose()
        {
            foreach(var item in items)
            {
                item.Dispose();
            }
        }
    }
}
