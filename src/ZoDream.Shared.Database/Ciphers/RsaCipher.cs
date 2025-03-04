using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace ZoDream.Shared.Database.Ciphers
{
    public class RsaCipher(string fileName) : ICipher, ICipherFile
    {

        private RSA? _encrytor;
        private RSA? _decrytor;

        public int DecryptBlockSize => _decrytor is null ? 1024 : (_decrytor.KeySize / 8);
        public int EncryptBlockSize => _encrytor is null ? 1024 : (_encrytor.KeySize / 8 - 11);

        private void TryLoadKey()
        {
            if (_encrytor is not null)
            {
                return;
            }
            using var fs = File.OpenRead(fileName);
            var buffer = new byte[2];
            fs.ReadExactly(buffer);
            var length = BitConverter.ToUInt16(buffer);
            _encrytor = RSA.Create();
            buffer = new byte[length];
            fs.ReadExactly(buffer);
            _encrytor.ImportRSAPublicKey(buffer, out _);

            _decrytor = RSA.Create();
            buffer = new byte[fs.Length - fs.Position];
            fs.ReadExactly(buffer);
            _decrytor.ImportRSAPrivateKey(buffer, out _);
        }


        public byte[] Decrypt(byte[] input)
        {
            TryLoadKey();
            return _decrytor!.Decrypt(input, RSAEncryptionPadding.Pkcs1);
        }

        public Stream Decrypt(Stream input)
        {
            TryLoadKey();
            return new CryptoStream(input, this, DecryptBlockSize, false);
        }

        public int Decrypt(byte[] input, int index, int count)
        {
            var res = Decrypt(input.Skip(index).Take(count).ToArray());
            Array.Copy(res, input, res.Length);
            return res.Length;
        }

        public int Encrypt(byte[] input, int index, int count)
        {
            var res = Encrypt(input.Skip(index).Take(count).ToArray());
            Array.Copy(res, input, res.Length);
            return res.Length;
        }

        public Stream Encrypt(Stream input)
        {
            TryLoadKey();
            return new CryptoStream(input, this, EncryptBlockSize, true);
        }

        public byte[] Encrypt(byte[] input)
        {
            TryLoadKey();
            return _encrytor!.Encrypt(input, RSAEncryptionPadding.Pkcs1);
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
            _encrytor?.Dispose();
            _decrytor?.Dispose();
        }
    }
}
