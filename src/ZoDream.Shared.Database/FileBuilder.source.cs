using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ZoDream.Shared.Database.Models;

namespace ZoDream.Shared.Database
{
    internal partial class FileBuilder 
    {

/**
* [4] signature
* [1] version
* [32] md5_encrypt_md5_options
* [4] groupOffset
* [1] groupCount
* [4] entryOffset
* [2] entryCount
* [4] entryDataOffset
* 
* [?] cipher iv
* 
* group
* * [1] parentIndex
* * [1] nameLength
* * [nameLength] name

* 
* entry
* * [1] entryType
* * [1] groupIndex
* * [1] dataCount
* * [1] titleLength
* * [1]? accountLength
* * * [?] dataLength
* * [titleLength] title
* * [accountLength]? account
* * * [dataLength] data
* * * [1] dataType [?] data
*/


        private readonly string TemporaryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "__temp__");


        private readonly Dictionary<int, IRecordSource> _items = [];

        private int _lastIndex = GroupRecord.BeginIndex;

        private BinaryWriter? _temporaryWriter;
        public BinaryWriter Writer => _temporaryWriter ??= new BinaryWriter(File.Create(TemporaryPath));


        private FileHeader LoadHeader(BinaryReader reader)
        {
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

        private void Load(Stream input)
        {
            _items.Clear();
            var reader = new BinaryReader(input);
            var header = LoadHeader(reader);
            LoadGroup(reader, header);
            LoadEntry(reader, header);
        }

        private void LoadGroup(BinaryReader reader, FileHeader header)
        {
            var pos = header.GroupOffset;
            for (var i = 0; i < header.GroupCount; i++)
            {
                reader.BaseStream.Seek(pos, SeekOrigin.Begin);
                var item = new GroupRecord()
                {
                    Id = _lastIndex++,
                    SourceType = RecordSourceType.Original,
                    EntryOffset = pos,
                    ParentId = reader.ReadByte(),
                    NameLength = reader.ReadByte()
                };
                _items.Add(item.Id, item);
                pos = item.EntryOffset + item.EntryLength;
            }
        }
        private void LoadEntry(BinaryReader reader, FileHeader header)
        {
            var pos = header.EntryRealOffset;
            for (var i = 0; i < header.EntryCount; i++)
            {
                reader.BaseStream.Seek(pos, SeekOrigin.Begin);
                var entry = new EntryRecord()
                {
                    Id = _lastIndex++,
                    EntryOffset = pos,
                    SourceType = RecordSourceType.Original,
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
                pos = entry.EntryOffset + entry.EntryLength;
                _items.Add(entry.Id, entry);
            }
        }

        public int Save(IGroupEntity entity)
        {
            var writer = Writer;
            var buffer = _cipher.Encrypt(Encoding.UTF8.GetBytes(entity.Name));
            if (entity.Id <= 0)
            {
                entity.Id = _lastIndex++;
            }
            writer.BaseStream.Seek(0, SeekOrigin.End);
            var group = new GroupRecord
            {
                Id = entity.Id,
                EntryOffset = writer.BaseStream.Position,
                ParentId = entity.ParentId,
                NameLength = buffer.Length,
                SourceType = RecordSourceType.Temporary,
            };
            writer.Write((byte)group.ParentId);
            writer.Write((byte)buffer.Length);
            writer.Write(buffer);
            Add(group);
            return group.Id;
        }

        public int Save(IEntryEntity entity)
        {
            var writer = Writer;
            writer.BaseStream.Seek(0, SeekOrigin.End);
            if (entity.Id <= 0)
            {
                entity.Id = _lastIndex++;
            }
            var group = new EntryRecord
            {
                Id = entity.Id,
                EntryOffset = writer.BaseStream.Position,
                GroupId = entity.GroupId,
                Type = TypeMapper.Convert(entity),
            };
            var names = TypeMapper.EntryPropertyNames(group.Type);
            var hasAccount = TypeMapper.HasAccountProperty(group.Type);
            var begin = hasAccount ? 2 : 1;
            group.PropertiesLength = new int[names.Length];
            var data = new IStreamFormatter[names.Length];
            for (var i = 0; i < names.Length; i++)
            {
                var value = TypeMapper.GetProperty<string>(entity, names[i], i - begin);
                if (string.IsNullOrWhiteSpace(value))
                {
                    data[i] = new ByteFormatter([]);
                }
                else if (names[i] == "FileName")
                {
                    data[i] = new FileFormatter(value);
                }
                else
                {
                    data[i] = new StringFormatter(value);
                }
                group.PropertiesLength[i] = data[i].Length;
            }
            writer.Write((byte)group.Type);
            writer.Write((byte)group.GroupId);
            writer.Write((byte)(names.Length - begin));
            writer.Write((byte)group.PropertiesLength[0]);
            if (hasAccount)
            {
                writer.Write((byte)group.PropertiesLength[1]);
            }
            if (group.IsLargeLength)
            {
                for (var i = begin; i < group.PropertiesLength.Length; i++)
                {
                    writer.Write((uint)group.PropertiesLength[i]);
                }
            }
            else
            {
                for (var i = begin; i < group.PropertiesLength.Length; i++)
                {
                    writer.Write((ushort)group.PropertiesLength[i]);
                }
            }
            foreach (var item in data)
            {
                item.CopyTo(writer.BaseStream);
                item.Dispose();
            }
            Add(group);
            return entity.Id;
        }
        private void Add(IRecordSource record)
        {
            if (_items.TryAdd(record.Id, record))
            {
                return;
            }
            _items[record.Id] = record;
        }

        public void Delete(IGroupEntity group, int changeToGroup)
        {
            Delete(group.Id);
            foreach (var item in _items)
            {
                if (item.Value is GroupRecord g && g.ParentId == group.Id)
                {
                    g.ParentId = changeToGroup;
                } else if (item.Value is EntryRecord e && e.GroupId == group.Id)
                {
                    e.GroupId = changeToGroup;
                }
            }
        }
        public void Delete(IEntryEntity data)
        {
            _items.Remove(data.Id);
        }

        private void Delete(int id)
        {
            _items.Remove(id);
        }

        private void Flush(Stream output)
        {
            var isOverwrite = output == BaseStream;
            output.Seek(0, SeekOrigin.Begin);
            var writer = new BinaryWriter(output);
            var header = new FileHeader();
            var maps = new Dictionary<int, int>();
            var groups = new List<GroupRecord>();
            var entries = new List<EntryRecord>();
            var maxBuffer = 1024;
            foreach (var item in _items)
            {
                if (item.Value is GroupRecord g && g.Id >= GroupRecord.BeginIndex)
                {
                    header.EntryOffset += item.Value.EntryLength;
                    groups.Add(g);
                }
                else if (item.Value is EntryRecord e)
                {
                    entries.Add(e);
                }
                if (item.Value.EntryLength > maxBuffer)
                {
                    maxBuffer = (int)item.Value.EntryLength;
                }
            }
            header.EntryCount = entries.Count;
            header.GroupCount = groups.Count;
            WriteHeader(writer, header);
            var index = GroupRecord.BeginIndex;
            var buffer = ArrayPool<byte>.Shared.Rent(maxBuffer);
            try
            {
                var pos = writer.BaseStream.Position;
                foreach (var item in groups.Order(this))
                {
                    var id = index++;
                    var parentId = maps.TryGetValue(item.ParentId, out var i) ? i : (item.ParentId > GroupRecord.BeginIndex ? 0 : item.ParentId);
                    maps.Add(item.Id, id);
                    var len = ReadRecord(buffer, item);
                    buffer[0] = (byte)parentId;
                    writer.BaseStream.Seek(pos, SeekOrigin.Begin);
                    writer.Write(buffer, 0, len);
                    if (isOverwrite)
                    {
                        item.SourceType = RecordSourceType.Original;
                        item.EntryOffset = pos;
                    }
                    pos = writer.BaseStream.Position;
                }
                foreach (var item in entries.OrderBy(i => i.Id))
                {
                    var groupId = maps.TryGetValue(item.GroupId, out var i) ? i : (item.GroupId > GroupRecord.BeginIndex ? 0 : item.GroupId);
                    var len = ReadRecord(buffer, item);
                    buffer[1] = (byte)groupId;
                    writer.BaseStream.Seek(pos, SeekOrigin.Begin);
                    writer.Write(buffer, 0, len);
                    if (isOverwrite)
                    {
                        item.SourceType = RecordSourceType.Original;
                        item.EntryOffset = pos;
                    }
                    pos = writer.BaseStream.Position;
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }

        }

        private int ReadRecord(byte[] buffer, IRecordSource record)
        {
            return ReadRecord(buffer, record, 0, (int)record.EntryLength);
        }

        private int ReadRecord(byte[] buffer, IRecordSource record, int offset, int count)
        {
            if (record.SourceType == RecordSourceType.Original)
            {
                BaseStream.Seek(record.EntryOffset + offset, SeekOrigin.Begin);
                return BaseStream.Read(buffer, 0, count);
            }
            else
            {
                Writer.BaseStream.Seek(record.EntryOffset + offset, SeekOrigin.Begin);
                return Writer.BaseStream.Read(buffer, 0, count);
            }
        }

        private Stream AsStream(IRecordSource record)
        {
            if (record.SourceType == RecordSourceType.Original)
            {
                return new PartialStream(BaseStream, record.EntryOffset, record.EntryLength);
            }
            else
            {
                return new PartialStream(Writer.BaseStream, record.EntryOffset, record.EntryLength);
            }
        }
        private void WriteHeader(BinaryWriter writer, FileHeader header)
        {
            header.ValidityCode = _cipher.Signature();
            header.Write(writer);
            if (_cipher is ICipherIV c)
            {
                c.Write(writer.BaseStream);
            }
            header.GroupOffset = writer.BaseStream.Position;
            header.Write(writer);
        }
    }
}
