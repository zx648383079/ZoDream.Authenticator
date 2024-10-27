namespace ZoDream.Shared.Database
{
    public interface ICipher
    {
        public byte[] Decrypt(byte[] input);
        public byte[] Encrypt(byte[] input);

        
    }
}
