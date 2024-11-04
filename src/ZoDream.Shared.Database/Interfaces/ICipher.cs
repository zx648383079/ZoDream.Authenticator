using System;

namespace ZoDream.Shared.Database
{
    public interface ICipher: IDisposable
    {
        public byte[] Decrypt(byte[] input);
        public byte[] Encrypt(byte[] input);

        public byte[] Signature();
    }
}
