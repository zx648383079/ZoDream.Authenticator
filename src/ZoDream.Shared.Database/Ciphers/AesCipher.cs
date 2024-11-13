﻿using System.IO;
using System.Security.Cryptography;

namespace ZoDream.Shared.Database
{
    public class AesCipher(string fileName) : ICipher, ICipherFile, ICipherIV
    {

        private readonly Aes _aes = Aes.Create();
        private bool _isLoadedKey = false;

        private void TryLoadKey()
        {
            if (_isLoadedKey)
            {
                return;
            }
            _isLoadedKey = true;
            using var fs = File.OpenRead(fileName);
            //fs.Read(aes.IV);
            fs.Read(_aes.Key);
        }

        public byte[] Decrypt(byte[] input)
        {
            TryLoadKey();
            var decryptor = _aes.CreateDecryptor();
            return decryptor.TransformFinalBlock(input, 0, input.Length);
        }

     

        public byte[] Encrypt(byte[] input)
        {
            TryLoadKey();
            var encryptor = _aes.CreateEncryptor();
            return encryptor.TransformFinalBlock(input, 0, input.Length);
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
            //aes.GenerateIV();
            _aes.GenerateKey();
            using var fs = File.OpenRead(fileName);
            //fs.Write(aes.IV);
            fs.Write(_aes.Key);
        }

        public void Write(Stream input)
        {
            _aes.GenerateIV();
            input.Write(_aes.IV);
        }

        public void Read(Stream input)
        {
            input.Read(_aes.IV);
        }

        public void Dispose()
        {
            _aes.Dispose();
        }

     
    }
}