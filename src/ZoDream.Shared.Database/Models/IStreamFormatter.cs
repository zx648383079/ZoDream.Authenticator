using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Database.Models
{
    internal interface IStreamFormatter: IDisposable
    {

        public int Length { get; }

        public void CopyTo(Stream output);
    }

    internal class ByteFormatter(byte[] buffer) : IStreamFormatter
    {
        public int Length => buffer.Length;

        public void CopyTo(Stream output)
        {
            output.Write(buffer, 0, Length);
        }

        public void Dispose()
        {
        }
    }

    internal class StringFormatter(string text) : ByteFormatter(Encoding.UTF8.GetBytes(text)), IStreamFormatter
    {

    }

    internal class FileFormatter(Stream input) : IStreamFormatter
    {
        public FileFormatter(string fileName)
            :this (File.OpenRead(fileName))
        {
            
        }
        public int Length => (int)input.Length;

        public void CopyTo(Stream output)
        {
            input.CopyTo(output);
        }

        public void Dispose()
        {
            input.Dispose();
        }
    }
}
