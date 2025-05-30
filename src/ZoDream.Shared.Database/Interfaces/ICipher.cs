using System;
using System.IO;

namespace ZoDream.Shared.Database
{
    public interface ICipher: IDisposable
    {

        public byte RandomKey { get; }
        public void Seek(long position);
        public byte[] Decrypt(byte[] input);
        public int Decrypt(byte[] input, int index, int count);
        public byte[] Encrypt(byte[] input);
        public int Encrypt(byte[] input, int index, int count);

        public Stream Decrypt(Stream input);
        public Stream Encrypt(Stream input);
        /// <summary>
        /// 返回16位字节
        /// </summary>
        /// <returns></returns>
        public byte[] Signature();
    }
}
