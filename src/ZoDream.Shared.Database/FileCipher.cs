using System;
using System.IO;
using System.Security.Cryptography;

namespace ZoDream.Shared.Database
{
    public class FileCipher(Stream stream) : ICipher, ICipherFile
    {
        public FileCipher(string fileName): this(File.OpenRead(fileName))
        {
            
        }
        public byte[] Decrypt(byte[] input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = (byte)(input[i] ^ stream.ReadByte());
            }
            return input;
        }

        public byte[] Encrypt(byte[] input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = (byte)(input[i] ^ stream.ReadByte());
            }
            return input;
        }

        public byte[] Signature()
        {
            using var md5 = MD5.Create();
            return md5.ComputeHash(stream);
        }

        public void Generate()
        {
            stream.Seek(0, SeekOrigin.Begin);
            var random = new Random();
            var length = random.Next(10, 100) * 1000;
            var buffer = new byte[100];
            var i = 0;
            while (i < length)
            {
                random.NextBytes(buffer);
                stream.Write(buffer, 0, buffer.Length);
                i += buffer.Length;
            }
            stream.Flush();
            stream.Seek(0, SeekOrigin.Begin);
        }

        public void Dispose()
        {
            stream.Dispose();
        }
    }
}
