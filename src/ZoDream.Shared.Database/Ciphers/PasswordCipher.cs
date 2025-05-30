using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ZoDream.Shared.Database
{
    public class PasswordCipher(byte[] iv) : ICipher
    {
        public PasswordCipher(string key)
            : this(Hash(key))
        {
        }

        /// <summary>
        /// 用来加密长度
        /// </summary>
        public byte RandomKey => iv[(iv[0] ^ iv.Last()) % iv.Length];

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

        /// <summary>
        /// 生成不可逆的加密密码
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static byte[] Hash(string key)
        {
            var data = Encoding.UTF8.GetBytes(key);
            Debug.Assert(data.Length > 3);
            var length = data[0] + data[1];
            if (length < 20)
            {
                length = 30;
            } 
            else if (length > 128)
            {
                length = Math.Max(length % 128, 44);
            }
            var sha1 = SHA1.HashData(data);
            var buffer = new byte[length];
            for (int i = 0; i < length; i++)
            {
                var j = data[i % data.Length];
                var val = sha1[j % sha1.Length];
                buffer[i] = val == 0 ? (byte)i : val;
            }
            return buffer;
        }
    }
}
