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

        private BinaryReader Reader => new(BaseStream);

        public FileHeader ReadHeader()
        {
            var header = new FileHeader();
            header.Read(Reader);
            return header;
        }

        public IEnumerable<GroupItem> ReadGroup(FileHeader header)
        {
            var index = 20;
            var reader = Reader;
            var count = header.GroupCount;
            var pos = header.GroupOffset;
            for (var i = 0; i < count; i++)
            {
                reader.BaseStream.Seek(pos, SeekOrigin.Begin);
                var group = new GroupItem()
                {
                    Id = index + i
                };
                group.Read(reader);
                pos = reader.BaseStream.Position;
                yield return group;
            }
        }

        public IEnumerable<EntryItem> ReadEntry(FileHeader header)
        {
            var reader = Reader;
            var count = header.EntryCount;
            var pos = header.EntryOffset;
            var offset = 0;
            for (var i = 0; i < count; i++)
            {
                reader.BaseStream.Seek(pos, SeekOrigin.Begin);
                var entry = new EntryItem()
                {
                    Id = i,
                    PropertyOffset = offset,
                };
                entry.Read(reader);
                pos = reader.BaseStream.Position;
                offset += entry.PropertyCount;
                yield return entry;
            }
        }

        public IEnumerable<Stream> ReadEntryData(FileHeader header)
        {
            var reader = Reader;
            var pos = header.EntryDataOffset;
            while(true)
            {
                reader.BaseStream.Seek(pos, SeekOrigin.Begin);
                var length = reader.ReadUInt16();
                var next = pos + length + 2;
                if (next > reader.BaseStream.Length)
                {
                    yield break;
                }
                pos = next;
                yield return new PartialStream(reader.BaseStream, length);
            }
        }

        public IEnumerable<Stream> ReadEntryData(FileHeader header, EntryItem entry)
        {
            var reader = Reader;
            var pos = header.EntryDataOffset;
            reader.BaseStream.Seek(pos, SeekOrigin.Begin);
            for (var i = 0; i < entry.PropertyOffset; i++)
            {
                var length = reader.ReadUInt16();
                reader.BaseStream.Seek(length, SeekOrigin.Current);
            }
            pos = reader.BaseStream.Position;
            for (var i = 0; i < entry.PropertyCount; i++)
            {
                reader.BaseStream.Seek(pos, SeekOrigin.Begin);
                var length = reader.ReadUInt16();
                var next = pos + length + 2;
                pos = next;
                yield return new PartialStream(reader.BaseStream, length);
            }
        }
    }
}
