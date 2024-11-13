using System;

namespace ZoDream.Shared.Database
{
    public interface ICipher: IDisposable
    {

        public void Seek(long position);
        public byte[] Decrypt(byte[] input);
        public byte[] Encrypt(byte[] input);
        /// <summary>
        /// 返回16位字节
        /// </summary>
        /// <returns></returns>
        public byte[] Signature();
    }
}
