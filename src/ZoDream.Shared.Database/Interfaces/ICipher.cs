using System;
using System.IO;

namespace ZoDream.Shared.Database
{
    public interface ICipher: IDisposable
    {

        public void Seek(long position);
        public byte[] Decrypt(byte[] input);
        public byte[] Encrypt(byte[] input);

        public Stream Decrypt(Stream input);
        public Stream Encrypt(Stream input);
        /// <summary>
        /// 返回16位字节
        /// </summary>
        /// <returns></returns>
        public byte[] Signature();
    }
}
