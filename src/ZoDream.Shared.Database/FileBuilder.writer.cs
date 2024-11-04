using System;
using System.IO;

namespace ZoDream.Shared.Database
{
    public partial class FileBuilder
    {

        private BinaryWriter Writer => new(BaseStream);

        public void Write(IFileFormatter data)
        {
            if (data is FileHeader h)
            {
                h.ValidityCode = _cipher.Signature();
            }
            data.Write(Writer);
        }

        public void Write(byte[] buffer)
        {
            var writer = Writer;
            writer.Write((ushort)buffer.Length);
            writer.Write(buffer);
        }

        public void Write(Stream buffer)
        {
            var writer = Writer;
            writer.Write((ushort)(buffer.Length - buffer.Position));
            buffer.CopyTo(writer.BaseStream);
        }


        /// <summary>
        /// 移除字符或空出位置
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="length"></param>
        protected void AddSpace(long position, long length)
        {
            if (length == 0)
            {
                return;
            }
            var oldPos = BaseStream.Position;
            BaseStream.Seek(position, SeekOrigin.Begin);
            if (length < 0)
            {
                RemoveByte(BaseStream, Math.Abs(length));
            } else
            {
                InsertByte(BaseStream, length);
            }
            BaseStream.Seek(oldPos, SeekOrigin.Begin);
        }

        private void InsertByte(Stream stream, long length)
        {
            if (length == 0)
            {
                return;
            }
            var end = stream.Length + length;
            var begin = stream.Position + length;
            var buffer = new byte[Math.Min(end - begin, 1024 * 100)];
            for (var i = end; i > begin; i -= buffer.Length)
            {
                var len = (int)Math.Min(i - begin, buffer.Length);
                stream.Seek(i - len - length, SeekOrigin.Begin);
                stream.Read(buffer, 0, len);
                stream.Seek(i - len, SeekOrigin.Begin);
                stream.Write(buffer, 0, len);
            }
            stream.SetLength(end);
        }

        private void RemoveByte(Stream stream, long length)
        {
            if (length == 0)
            {
                return;
            }
            var end = stream.Length - length;
            var begin = stream.Position;
            var buffer = new byte[Math.Min(end - begin, 1024 * 100)];
            for (var i = begin; i < end; i += buffer.Length)
            {
                var len = (int)Math.Min(end - i, buffer.Length);
                stream.Seek(i + length, SeekOrigin.Begin);
                stream.Read(buffer, 0, len);
                stream.Seek(i, SeekOrigin.Begin);
                stream.Write(buffer, 0, len);
            }
            stream.SetLength(end);
        }

        /// <summary>
        /// 应用写入
        /// </summary>
        public void Flush()
        {
            BaseStream.Flush();
        }
    }
}
