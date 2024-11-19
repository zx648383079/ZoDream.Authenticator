using System;
using System.IO;
using System.Text;
using ZoDream.Shared.Database.Models;

namespace ZoDream.Shared.Database
{
    internal partial class FileBuilder
    {

        private BinaryWriter Writer => new(BaseStream);

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="header"></param>
        public void WriteHeader(FileHeader header)
        {
            var writer = Writer;
            header.ValidityCode = _cipher.Signature();
            header.EntryOffset = header.GroupOffset = 0;
            header.EntryCount = header.GroupCount = 0;
            header.Write(writer);
            if (_cipher is ICipherIV c)
            {
                c.Write(writer.BaseStream);
            }
            header.GroupOffset = writer.BaseStream.Position;
            header.Write(writer);
        }

        public GroupRecord Write(FileHeader header, IGroupEntity entity, long lockedLength = 0)
        {
            var writer = Writer;
            var buffer = _cipher.Encrypt(Encoding.UTF8.GetBytes(entity.Name));
            var pos = writer.BaseStream.Position;
            var group = new GroupRecord
            {
                Id = entity.Id,
                EntryOffset = pos - header.GroupOffset,
                ParentId = entity.ParentId,
                NameLength = buffer.Length
            };
            AddSpace(pos, group.EntryLength - lockedLength);
            writer.Write((byte)group.ParentId);
            writer.Write((byte)buffer.Length);
            writer.Write(buffer);
            return group;
        }

        public EntryRecord Write(FileHeader header, IEntryEntity entity, long lockedLength = 0)
        {
            var writer = Writer;
            var pos = writer.BaseStream.Position;
            var group = new EntryRecord
            {
                Id = entity.Id,
                EntryOffset = pos - header.GroupOffset,
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
                else if(names[i] == "FileName")
                {
                    data[i] = new FileFormatter(value);
                } else
                {
                    data[i] = new StringFormatter(value);
                }
                group.PropertiesLength[i] = data[i].Length;
            }
            AddSpace(pos, group.EntryLength - lockedLength);
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
            } else
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
            return group;
        }

        public EntryRecord Write(FileHeader header, object entity, long lockedLength = 0)
        {
            var writer = Writer;
            var pos = writer.BaseStream.Position;
            var group = new EntryRecord
            {
                Id = TypeMapper.GetProperty<int>(entity, "Id"),
                EntryOffset = pos - header.GroupOffset,
                GroupId = TypeMapper.GetProperty<int>(entity, "GroupId"),
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
            AddSpace(pos, group.EntryLength - lockedLength);
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
            return group;
        }

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
        public void AddSpace(long position, long length)
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
            var chunkLength = Math.Min(end - begin, 1024 * 100);
            if (chunkLength > 0)
            {
                var buffer = new byte[chunkLength];
                for (var i = end; i > begin; i -= buffer.Length)
                {
                    var len = (int)Math.Min(i - begin, buffer.Length);
                    stream.Seek(i - len - length, SeekOrigin.Begin);
                    stream.Read(buffer, 0, len);
                    stream.Seek(i - len, SeekOrigin.Begin);
                    stream.Write(buffer, 0, len);
                }
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
