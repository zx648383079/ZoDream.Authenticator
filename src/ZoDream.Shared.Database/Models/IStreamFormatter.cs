using System;
using System.IO;
using System.Text;

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

    internal class StringFormatter : ByteFormatter, IStreamFormatter
    {
        public StringFormatter(string text)
            : base(Encoding.UTF8.GetBytes(text))
        {
            
        }
        public StringFormatter(string text, ICipher cipher)
            : base(cipher.Encrypt(Encoding.UTF8.GetBytes(text)))
        {
            
        }
    }

    internal class FileFormatter : IStreamFormatter
    {
        public FileFormatter(string fileName)
            :this (File.OpenRead(fileName))
        {
            
        }
        public FileFormatter(string fileName, ICipher cipher)
            : this(File.OpenRead(fileName), cipher)
        {
            
        }

        public FileFormatter(Stream input, ICipher cipher)

        {
            BaseStream = new MemoryStream();
            cipher.Encrypt(input).CopyTo(BaseStream);
            input.Dispose();
        }

        public FileFormatter(Stream input)
        {
            BaseStream = input;
        }

        private readonly Stream BaseStream;

        public int Length => (int)BaseStream.Length;

        public void CopyTo(Stream output)
        {
            BaseStream.CopyTo(output);
        }

        public void Dispose()
        {
            BaseStream.Dispose();
        }
    }
}
