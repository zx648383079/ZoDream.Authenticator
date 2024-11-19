using System;
using System.IO;
using System.Security.Cryptography;

namespace ZoDream.Shared.Database.Ciphers
{
    public class RsaCipher(string fileName) : ICipher, ICipherFile
    {

        public byte[] Decrypt(byte[] input)
        {
            using var fs = File.OpenRead(fileName);
            using var rsa = RSA.Create();
            var buffer = new byte[2];
            fs.ReadExactly(buffer);
            var length = BitConverter.ToUInt16(buffer);
            fs.Seek(length, SeekOrigin.Current);
            buffer = new byte[fs.Length - fs.Position];
            fs.ReadExactly(buffer);
            rsa.ImportRSAPrivateKey(buffer, out _);
            var maxBlockSize = rsa.KeySize / 8;
            return rsa.Decrypt(input, RSAEncryptionPadding.Pkcs1);
        }

        public Stream Decrypt(Stream input)
        {

        }

        public Stream Encrypt(Stream input)
        {

        }

        public byte[] Encrypt(byte[] input)
        {
            using var fs = File.OpenRead(fileName);
            using var rsa = RSA.Create();
            var buffer = new byte[2];
            fs.ReadExactly(buffer);
            var length = BitConverter.ToUInt16(buffer);
            buffer = new byte[length];
            fs.ReadExactly(buffer);
            rsa.ImportRSAPublicKey(buffer, out _);
            var maxBlockSize = rsa.KeySize / 8 - 11;
            
            return rsa.Encrypt(input, RSAEncryptionPadding.Pkcs1);
        }

        public void Seek(long position)
        {
        }

        public byte[] Signature()
        {
            using var fs = File.OpenRead(fileName);
            using var md5 = MD5.Create();
            return md5.ComputeHash(fs);
        }

        public void Generate()
        {
            using var rsa = RSA.Create();
            //rsa.ExportRSAPrivateKeyPem();
            //rsa.ExportRSAPublicKeyPem();
            using var fs = File.OpenRead(fileName);
            var buffer = rsa.ExportRSAPublicKey();
            fs.Write(BitConverter.GetBytes((ushort)buffer.Length));
            fs.Write(buffer);
            fs.Write(rsa.ExportRSAPrivateKey());
        }

        public void Dispose()
        {
        }
    }
}
