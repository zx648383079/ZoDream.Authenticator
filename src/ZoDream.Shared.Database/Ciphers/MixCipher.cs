using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace ZoDream.Shared.Database
{
    public class MixCipher(params ICipher[] items) : ICipher, ICipherIV
    {
        public byte RandomKey => items.Select(i => i.RandomKey).Max();
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
        public Stream Decrypt(Stream input)
        {
            foreach (var item in items)
            {
                input = item.Decrypt(input);
            }
            return input;
        }

        public int Decrypt(byte[] input, int index, int count)
        {
            foreach (var item in items)
            {
                count = item.Decrypt(input, index, count);
                index = 0;
            }
            return count;
        }

        public int Encrypt(byte[] input, int index, int count)
        {
            foreach (var item in items)
            {
                count = item.Encrypt(input, index, count);
                index = 0;
            }
            return count;
        }

        public Stream Encrypt(Stream input)
        {
            foreach (var item in items)
            {
                input = item.Encrypt(input);
            }
            return input;
        }


        public void Write(Stream input)
        {
            foreach (var item in items)
            {
                if (item is ICipherIV c)
                {
                    c.Write(input);
                }
            }
        }

        public void Read(Stream input)
        {
            foreach (var item in items)
            {
                if (item is ICipherIV c)
                {
                    c.Read(input);
                }
            }
        }

        public void Seek(long position)
        {
            foreach (var item in items)
            {
                item.Seek(position);
            }
        }

        public byte[] Signature()
        {
            var buffer = new byte[16 * items.Length];
            for (var i = 0; i < items.Length; i++)
            {
                Array.Copy(items[i].Signature(), 0, buffer, i * 16, 16);
            }
            Seek(0);
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
