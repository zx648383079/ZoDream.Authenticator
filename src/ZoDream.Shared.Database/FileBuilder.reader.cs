using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ZoDream.Shared.Database
{
    internal partial class FileBuilder
    {

        private BinaryReader Reader => new(BaseStream);

        public string ReadString(int length)
        {
            if (length <= 0)
            {
                return string.Empty;
            }
            return Encoding.UTF8.GetString(Reader.ReadBytes(length));
        }

        public FileHeader ReadHeader()
        {
            var reader = Reader;
            var header = new FileHeader();
            header.Read(reader);
            if (!header.ValidityCode.SequenceEqual(_cipher.Signature()))
            {
                throw new CryptographicException("cipher is error");
            }
            if (_cipher is ICipherIV c)
            {
                c.Read(reader.BaseStream);
            }
            return header;
        }

        public IEnumerable<GroupRecord> ReadGroup(FileHeader header)
        {
            var index = GroupRecord.BeginIndex;
            var reader = Reader;
            var count = header.GroupCount;
            var pos = header.GroupOffset;
            for (var i = 0; i < count; i++)
            {
                reader.BaseStream.Seek(pos, SeekOrigin.Begin);
                var group = new GroupRecord
                {
                    Id = index + i,
                    EntryOffset = pos - header.GroupOffset,
                    ParentId = reader.ReadByte(),
                    NameLength = reader.ReadByte()
                };
                pos += group.NameLength;
                yield return group;
            }
        }

        public IEnumerable<EntryRecord> ReadEntry(FileHeader header)
        {
            var reader = Reader;
            var count = header.EntryCount;
            var pos = header.EntryRealOffset;
            for (var i = 0; i < count; i++)
            {
                reader.BaseStream.Seek(pos, SeekOrigin.Begin);
                var entry = new EntryRecord()
                {
                    EntryOffset = pos - header.EntryRealOffset,
                    Id = i,
                    Type = (EntryType)reader.ReadByte(),
                    GroupId = reader.ReadByte(),
                };
                var n = reader.ReadByte();
                var m = 1;
                if (entry.HasAccount)
                {
                    m++;
                }
                entry.PropertiesLength = new int[n + m];
                entry.PropertiesLength[0] = reader.ReadByte(); // title
                if (entry.HasAccount)
                {
                    entry.PropertiesLength[1] = reader.ReadByte();
                }
                for (var j = m; j < n; j++)
                {
                    entry.PropertiesLength[j] = (int)BinaryPrimitives.ReadUInt32LittleEndian(reader.ReadBytes(entry.IsLargeLength ? 4 : 2));
                }
                pos += entry.EntryLength;
                yield return entry;
            }
        }


        public void ReadEntry(FileHeader header, EntryRecord entry, object data)
        {
            var reader = Reader;
            var pos = header.EntryOffset + entry.EntryDataOffset;
            TypeMapper.SetProperty(data, "Id", entry.Id);
            TypeMapper.SetProperty(data, "GroupId", entry.Id);
            Seek(header, entry);
            var names = TypeMapper.EntryPropertyNames(entry.Type);
            var begin = entry.HasAccount ? 2 : 1;
            for (var i = 0; i < names.Length; i++)
            {
                if (names[i] == "FileName")
                {
                    continue;
                }
                TypeMapper.SetProperty(data, names[i], i - begin, ReadString(entry.PropertiesLength[i]));
            }
        }
    }
}
