using System.Collections.Generic;
using System.IO;

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
            var offset = header.EntryDataOffset;
            for (var i = 0; i < count; i++)
            {
                reader.BaseStream.Seek(pos, SeekOrigin.Begin);
                var entry = new EntryItem()
                {
                    Offset = pos - header.EntryOffset,
                    Id = i,
                    PropertyOffset = offset,
                };
                entry.Read(reader);
                pos = reader.BaseStream.Position;
                JumpDatePart(reader, offset, entry.PropertyCount);
                offset = reader.BaseStream.Position;
                yield return entry;
            }
        }

        private void JumpDatePart(BinaryReader reader, long offset, int count)
        {
            if (count == 0)
            {
                return;
            }
            reader.BaseStream.Seek(offset, SeekOrigin.Begin);
            for (var i = 0; i < count; i++)
            {
                var length = reader.ReadUInt16();
                reader.BaseStream.Seek(length, SeekOrigin.Current);
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
            var pos = header.EntryDataOffset + entry.PropertyOffset;
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
