using System;
using System.IO;

namespace ZoDream.Shared.Database
{
    internal partial class FileBuilder
    {
        private byte EncodeByte(byte b)
        {
            return (byte)(b ^ _ivKey);
        }
        private void EncodeByte(byte[] buffer)
        {
            for (var i = 0; i < buffer.Length; i++)
            {
                buffer[i] = EncodeByte(buffer[i]);
            }
        }
        public void Write(Stream output, byte[] buffer)
        {
            output.Write(buffer, 0, buffer.Length);
        }
        public void Write(Stream output, byte val)
        {
            output.WriteByte(EncodeByte(val));
        }

        public void Write(Stream output, uint val)
        {
            var buffer = BitConverter.GetBytes(val);
            EncodeByte(buffer);
            output.Write(buffer, 0, buffer.Length);
        }
        public void Write(Stream output, ushort val)
        {
            var buffer = BitConverter.GetBytes(val);
            EncodeByte(buffer);
            output.Write(buffer, 0, buffer.Length);
        }
    }
}
