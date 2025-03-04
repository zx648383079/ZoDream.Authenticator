using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ZoDream.Shared.Database
{
    public class PasswordCipher(byte[] iv) : ICipher
    {
        public PasswordCipher(string key)
            : this(Encoding.UTF8.GetBytes(key))
        {
        }

        private int _position;

        public byte ReadByte()
        {
            if (iv.Length == 0)
            {
                return 0;
            }
            if (_position >= iv.Length)
            {
                _position %= iv.Length;
            }
            return iv[_position++];
        }

        public byte[] Decrypt(byte[] input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = (byte)(input[i] ^ ReadByte());
            }
            return input;
        }

        public Stream Decrypt(Stream input)
        {
            return new CryptoStream(input, this, 256, false);
        }

        public int Decrypt(byte[] input, int index, int count)
        {
            for (int i = 0; i < count; i++)
            {
                input[i] = (byte)(input[i + index] ^ ReadByte());
            }
            return count;
        }

        public int Encrypt(byte[] input, int index, int count)
        {
            for (int i = 0; i < count; i++)
            {
                input[i] = (byte)(input[i + index] ^ ReadByte());
            }
            return count;
        }

        public Stream Encrypt(Stream input)
        {
            return new CryptoStream(input, this, 256, true);
        }

        public byte[] Encrypt(byte[] input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = (byte)(input[i] ^ ReadByte());
            }
            return input;
        }

        public void Seek(long position)
        {
            if (iv.Length == 0)
            {
                return;
            }
            _position = (int)(position % iv.Length);
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
