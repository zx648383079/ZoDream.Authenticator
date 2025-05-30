using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
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
* [?] recoveryCode
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

        private Stream? _temporaryStream;
        public Stream OutputStream => _temporaryStream ??= File.Create(TemporaryPath, 1024, FileOptions.DeleteOnClose);

        private FileHeader LoadHeader(Stream input)
        {
            var header = new FileHeader();
            ReadHeader(input, header);
            if (!header.ValidityCode.SequenceEqual(_cipher.Signature()))
            {
                throw new CryptographicException("cipher is error");
            }
            if (_cipher is ICipherIV c)
            {
                c.Read(input);
            }
            return header;
        }

        private void Load(Stream input)
        {
            _items.Clear();
            var header = LoadHeader(input);
            LoadGroup(input, header);
            LoadEntry(input, header);
        }

        private void LoadGroup(Stream input, FileHeader header)
        {
            var pos = header.GroupOffset;
            for (var i = 0; i < header.GroupCount; i++)
            {
                input.Seek(pos, SeekOrigin.Begin);
                var item = new GroupRecord()
                {
                    Id = _lastIndex++,
                    SourceType = RecordSourceType.Original,
                    EntryOffset = pos,
                    ParentId = ReadByte(input),
                    NameLength = ReadByte(input)
                };
                _items.Add(item.Id, item);
                pos = item.EntryOffset + item.EntryLength;
            }
        }
        private void LoadEntry(Stream input, FileHeader header)
        {
            var pos = header.EntryRealOffset;
            for (var i = 0; i < header.EntryCount; i++)
            {
                input.Seek(pos, SeekOrigin.Begin);
                var entry = new EntryRecord()
                {
                    Id = _lastIndex++,
                    EntryOffset = pos,
                    SourceType = RecordSourceType.Original,
                    Type = (EntryType)ReadByte(input),
                    GroupId = ReadByte(input),
                };
                var n = ReadByte(input);
                var m = 1;
                if (entry.HasAccount)
                {
                    m++;
                }
                entry.PropertiesLength = new int[n + m];
                entry.PropertiesLength[0] = ReadByte(input); // title
                if (entry.HasAccount)
                {
                    entry.PropertiesLength[1] = ReadByte(input);
                }
                for (var j = 0; j < n; j++)
                {
                    entry.PropertiesLength[m + j] = entry.IsLargeLength ? (int)ReadUInt32(input) : (int)ReadUInt16(input);
                }
                pos = entry.EntryOffset + entry.EntryLength;
                _items.Add(entry.Id, entry);
            }
        }

        public int Save(IGroupEntity entity)
        {
            var output = OutputStream;
            if (entity.Name.Length > 20)
            {
                entity.Name = entity.Name[..20];
            }
            _cipher.Seek(0);
            var buffer = _cipher.Encrypt(Encoding.UTF8.GetBytes(entity.Name));
            if (entity.Id <= 0)
            {
                entity.Id = _lastIndex++;
            }
            var group = new GroupRecord
            {
                Id = entity.Id,
                EntryOffset = PrepareWrite(output, false),
                ParentId = entity.ParentId,
                NameLength = buffer.Length,
                SourceType = RecordSourceType.Temporary,
            };
            Write(output, (byte)group.ParentId);
            Write(output, (byte)buffer.Length);
            output.Write(buffer);
            output.Flush();
            Add(group);
            return group.Id;
        }

        /// <summary>
        /// 预写入
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="nextIsEntry">接下来写来的数据是否时 entry</param>
        /// <returns>返回数据开始位置</returns>
        private long PrepareWrite(Stream output, bool nextIsEntry)
        {
            output.Seek(0, SeekOrigin.End);
            Write(output, (byte)(nextIsEntry ? 1 : 0));
            return output.Position;
        }

        public int Save(IEntryEntity entity)
        {
            var output = OutputStream;
            if (entity.Id <= 0)
            {
                entity.Id = _lastIndex++;
            }
            var group = new EntryRecord
            {
                Id = entity.Id,
                EntryOffset = PrepareWrite(output, true),
                GroupId = entity.GroupId,
                Type = TypeMapper.Convert(entity),
                SourceType = RecordSourceType.Temporary,
            };
            var names = TypeMapper.EntryPropertyNames(group.Type);
            var hasAccount = TypeMapper.HasAccountProperty(group.Type);
            var begin = hasAccount ? 2 : 1;
            group.PropertiesLength = new int[names.Length];
            _cipher.Seek(0);
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
                    data[i] = new FileFormatter(value, _cipher);
                }
                else
                {
                    data[i] = new StringFormatter(value, _cipher);
                }
                group.PropertiesLength[i] = data[i].Length;
            }
            Write(output, (byte)group.Type);
            Write(output, (byte)group.GroupId);
            Write(output, (byte)(names.Length - begin));
            Write(output, (byte)group.PropertiesLength[0]);
            if (hasAccount)
            {
                Write(output, (byte)group.PropertiesLength[1]);
            }
            
            for (var i = begin; i < group.PropertiesLength.Length; i++)
            {
                if (group.IsLargeLength)
                {
                    Write(output, (uint)group.PropertiesLength[i]);
                }
                else
                {
                    Write(output, (ushort)group.PropertiesLength[i]);
                }
            }
            foreach (var item in data)
            {
                item.CopyTo(output);
                item.Dispose();
            }
            output.Flush();
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
        public void Delete(IEntryEntity[] items)
        {
            foreach (var item in items)
            {
                _items.Remove(item.Id);
            }
            
        }

        private void Delete(int id)
        {
            _items.Remove(id);
        }

        private void Flush(Stream output)
        {
            var isOverwrite = output == BaseStream;
            output.Seek(0, SeekOrigin.Begin);
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
            WriteHeader(output, header);
            var index = GroupRecord.BeginIndex;
            if (_items.Count == 0)
            {
                output.SetLength(output.Position);
                output.Flush();
                return;
            }
            var buffer = ArrayPool<byte>.Shared.Rent(maxBuffer);
            try
            {
                var pos = output.Position;
                foreach (var item in groups.Order(this))
                {
                    var id = index++;
                    var parentId = maps.TryGetValue(item.ParentId, out var i) ? i : (item.ParentId > GroupRecord.BeginIndex ? 0 : item.ParentId);
                    maps.Add(item.Id, id);
                    var len = ReadRecord(buffer, item);
                    Debug.Assert(len == item.EntryLength);
                    buffer[0] = (byte)parentId;
                    output.Seek(pos, SeekOrigin.Begin);
                    output.Write(buffer, 0, len);
                    if (isOverwrite)
                    {
                        item.SourceType = RecordSourceType.Original;
                        item.EntryOffset = pos;
                    }
                    pos = output.Position;
                }
                foreach (var item in entries.OrderBy(i => i.Id))
                {
                    var groupId = maps.TryGetValue(item.GroupId, out var i) ? i : (item.GroupId > GroupRecord.BeginIndex ? 0 : item.GroupId);
                    var len = ReadRecord(buffer, item);
                    Debug.Assert(len == item.EntryLength);
                    buffer[1] = (byte)groupId;
                    output.Seek(pos, SeekOrigin.Begin);
                    output.Write(buffer, 0, len);
                    if (isOverwrite)
                    {
                        item.SourceType = RecordSourceType.Original;
                        item.EntryOffset = pos;
                    }
                    pos = output.Position;
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
            output.SetLength(output.Position);
            output.Flush();
        }

        private int ReadRecord(byte[] buffer, IRecordSource record)
        {
            return ReadRecord(buffer, record, 0, (int)record.EntryLength);
        }

        private int ReadRecord(byte[] buffer, IRecordSource record, int offset, int count)
        {
            var input = record.SourceType == RecordSourceType.Original ? BaseStream : OutputStream;
            PrepareRead(input, record);
            input.Seek(record.EntryOffset + offset, SeekOrigin.Begin);
            return input.Read(buffer, 0, count);
        }

        /// <summary>
        /// 验证读取的数据是否有误
        /// </summary>
        /// <param name="input"></param>
        /// <param name="record"></param>
        private void PrepareRead(Stream input, IRecordSource record)
        {
            if (record.SourceType == RecordSourceType.Original)
            {
                return;
            }
            input.Seek(record.EntryOffset - 1, SeekOrigin.Begin);
            var type = ReadByte(input);
            Debug.Assert(type <= 1 && (type > 0) == (record is EntryRecord));
        }

        private Stream AsStream(IRecordSource record)
        {
            if (record.SourceType == RecordSourceType.Original)
            {
                return new PartialStream(BaseStream, record.EntryOffset, record.EntryLength);
            }
            else
            {
                return new PartialStream(OutputStream, record.EntryOffset, record.EntryLength);
            }
        }

        private void ReadHeader(Stream input, FileHeader header)
        {
            input.Seek(0, SeekOrigin.Begin);
            var buffer = ReadBytes(input, 4);
            Debug.Assert(Encoding.ASCII.GetString(buffer) == FileHeader.Signature);
            header.Version = (DatabaseVersion)input.ReadByte();
            input.ReadExactly(header.ValidityCode);
            header.GroupOffset = ReadUInt32(input);
            header.GroupCount = ReadByte(input);
            header.EntryOffset = ReadUInt32(input);
            header.EntryCount = ReadUInt16(input);
        }

        private void WriteHeader(Stream output, FileHeader header)
        {
            header.ValidityCode = _cipher.Signature();
            output.Seek(0, SeekOrigin.Begin);
            output.Write(Encoding.ASCII.GetBytes(FileHeader.Signature));
            output.WriteByte((byte)header.Version);
            output.Write(header.ValidityCode);
            Write(output, (uint)header.GroupOffset);
            Write(output, (byte)header.GroupCount);
            Write(output, (uint)header.EntryOffset);
            Write(output, (ushort)header.EntryCount);
            if (_cipher is ICipherIV c)
            {
                c.Write(output);
            }
            header.GroupOffset = output.Position;
            WritePartHeader(output, header);
        }

        private void WritePartHeader(Stream output, FileHeader header)
        {
            output.Seek(21, SeekOrigin.Begin);
            Write(output, (uint)header.GroupOffset);
            Write(output, (byte)header.GroupCount);
            Write(output, (uint)header.EntryOffset);
            Write(output, (ushort)header.EntryCount);
        }
    }
}
