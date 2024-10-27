using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Database
{
    public partial class FileBuilder
    {

        private BinaryWriter Writer => new(BaseStream);

        public void Write(IFileFormatter data)
        {
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
        /// 移除字符
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="length"></param>
        protected void RemoveByte(Stream stream, long length)
        {
            if (length == 0)
            {
                return;
            }
            var end = stream.Length - length;
            var buffer = new byte[Math.Min(end, 1024 * 100)];
            for (var i = stream.Position; i < end; i += buffer.Length)
            {
                var len = (int)Math.Min(end - i, buffer.Length);
                stream.Seek(i + length, SeekOrigin.Begin);
                stream.Read(buffer, 0, len);
                stream.Seek(i, SeekOrigin.Begin);
                stream.Write(buffer, 0, len);
            }
            stream.SetLength(end);
            stream.Flush();
        }

        /// <summary>
        /// 应用写入
        /// </summary>
        public void Flush()
        {

        }
    }
}
