using System;
using System.IO;

namespace ZoDream.Shared.Database
{
    internal partial class FileBuilder
    {
        private byte DecodeByte(byte b)
        {
            return (byte)(b ^ _ivKey);
        }
        private void DecodeByte(byte[] buffer)
        {
            for (var i = 0; i < buffer.Length; i++)
            {
                buffer[i] = EncodeByte(buffer[i]);
            }
        }
        public byte[] ReadBytes(Stream input, int length)
        {
            var buffer = new byte[length];
            input.ReadExactly(buffer, 0, length);
            return buffer;
        }
        public byte ReadByte(Stream input)
        {
            return DecodeByte((byte)input.ReadByte());
        }

        public uint ReadUInt32(Stream input)
        {
            var buffer = new byte[4];
            input.ReadExactly(buffer);
            DecodeByte(buffer);
            return BitConverter.ToUInt32(buffer);
        }
        public ushort ReadUInt16(Stream input)
        {
            var buffer = new byte[2];
            input.ReadExactly(buffer);
            DecodeByte(buffer);
            return BitConverter.ToUInt16(buffer);
        }
    }
}
