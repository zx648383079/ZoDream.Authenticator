using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ZoDream.Shared.Database
{
    internal partial class FileBuilder : IDisposable
    {

        public FileBuilder(string fileName, ICipher cipher)
        {
            _fileName = fileName;
            _cipher = cipher;
            BaseStream = File.Open(_fileName, FileMode.OpenOrCreate);
            Load(BaseStream);
        }

        public FileBuilder(string fileName, ICipher cipher, bool isOpen)
        {
            _fileName = fileName;
            _cipher = cipher;
            BaseStream = File.Open(_fileName, isOpen ? FileMode.Open : FileMode.Create);
            if (isOpen)
            {
                Load(BaseStream);
            }
        }

        private readonly string _fileName;
        private readonly ICipher _cipher;
        public Stream BaseStream { get; private set; }

        public IEnumerable<IRecordSource> Items => _items.Values;

        public void Read(GroupRecord record, IGroupEntity instance)
        {
            if (record.NameLength == 0)
            {
                return;
            }
            Seek(record, true);
            instance.Name = ReadString(record, record.NameLength);
        }

        public void Read(EntryRecord record, IEntryEntity data)
        {
            
            TypeMapper.SetProperty(data, "Id", record.Id);
            TypeMapper.SetProperty(data, "GroupId", record.GroupId);
            Seek(record, true);
            var names = TypeMapper.EntryPropertyNames(record.Type);
            var begin = record.HasAccount ? 2 : 1;
            for (var i = 0; i < names.Length; i++)
            {
                if (names[i] == "FileName")
                {
                    continue;
                }
                TypeMapper.SetProperty(data, names[i], i - begin, ReadString(record, record.PropertiesLength[i]));
            }
        }

        public void Seek(IRecordSource record, bool isDataPosition = true)
        {
            var pos = record.EntryOffset;
            if (isDataPosition && record is ITableRecord t)
            {
                pos = t.EntryDataOffset;
            }
            if (record.SourceType == RecordSourceType.Original)
            {
                BaseStream.Seek(pos, SeekOrigin.Begin);
            }
            else
            {
                Writer.BaseStream.Seek(pos, SeekOrigin.Begin);
            }
        }

        /// <summary>
        /// 请先定位 Seek
        /// </summary>
        /// <param name="record"></param>
        public string ReadString(IRecordSource record, int length)
        {
            if (length == 0)
            {
                return string.Empty;
            }
            var buffer = ArrayPool<byte>.Shared.Rent(length);
            try
            {
                var len = 0;
                if (record.SourceType == RecordSourceType.Original)
                {
                    BaseStream.ReadExactly(buffer, 0, length);
                }
                else
                {
                    Writer.BaseStream.ReadExactly(buffer, 0, length);
                }
                len = _cipher.Decrypt(buffer, 0, len);
                return Encoding.UTF8.GetString(buffer, 0, len);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        public void Flush()
        {
            Flush(BaseStream);
        }
        public void Dispose()
        {
            _cipher.Dispose();
            BaseStream.Dispose();
            _temporaryWriter?.Dispose();
        }
    }
}
