using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Database
{
    public partial class FileBuilder : IDisposable
    {

        public FileBuilder(string fileName, ICipher cipher)
        {
            _fileName = fileName;
            _cipher = cipher;
            BaseStream = File.OpenRead(_fileName);
        }

        private readonly string _fileName;
        private readonly ICipher _cipher;
        public Stream BaseStream { get; private set; }


        public void Dispose()
        {
            
        }
    }
}
