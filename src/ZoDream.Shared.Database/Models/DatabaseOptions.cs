using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Database
{
    public class DatabaseOptions(string fileName): IDatabaseOptions
    {

        public string FileName { get; private set; } = fileName;
        public string Password { get; private set; } = string.Empty;
        public string KeyFileName { get; private set; } = string.Empty;

        public DatabaseOptions(string fileName, string password)
            : this (fileName)
        {
            Password = password;
        }

        public DatabaseOptions(string fileName, string password, string keyFile)
            : this(fileName, password)
        {
            KeyFileName = keyFile;
        }
    }
}
