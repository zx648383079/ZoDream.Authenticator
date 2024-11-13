using System;
using System.IO;
using System.Reflection.PortableExecutable;

namespace ZoDream.Shared.Database
{
    internal partial class FileBuilder : IDisposable
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
        /// <summary>
        /// 跳转到开始的位置
        /// </summary>
        /// <param name="pos"></param>
        public void Seek(long pos)
        {
            BaseStream.Seek(pos, SeekOrigin.Begin);
        }

        /// <summary>
        /// 跳转到记录自定义属性开始的位置
        /// </summary>
        /// <param name="header"></param>
        /// <param name="record"></param>
        public void Seek(FileHeader header, ITableRecord record, bool isData = true)
        {
            Seek(header.GetRecordOffset(record, isData));
        }
        /// <summary>
        /// 跳转到 分组 还是 entry 数据的开始位置
        /// </summary>
        /// <param name="header"></param>
        /// <param name="isEntry"></param>
        public void Seek(FileHeader header, bool isEntry)
        {
            var offset = header.GroupOffset;
            if (isEntry)
            {
                offset += header.EntryOffset;
            }
            Seek(offset);
        }

    }
}
