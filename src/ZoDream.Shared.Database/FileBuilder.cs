using System;
using System.IO;

namespace ZoDream.Shared.Database
{
    public partial class FileBuilder : IDisposable
    {

        public FileBuilder(string fileName, ICipher cipher)
        {
            _fileName = fileName;
            _cipher = cipher;
            BaseStream = File.Open(_fileName, FileMode.OpenOrCreate);
        }

        public FileBuilder(string fileName, ICipher cipher, bool isOpen)
        {
            _fileName = fileName;
            _cipher = cipher;
            BaseStream = File.Open(_fileName, isOpen ? FileMode.Open : FileMode.Create);
        }

        private readonly string _fileName;
        private readonly ICipher _cipher;
        public Stream BaseStream { get; private set; }


        public void Dispose()
        {
            _cipher.Dispose();
            BaseStream.Dispose();
        }
    }
}
