using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Database
{
    public class GroupItem : IFileFormatter
    {
        public int Id { get; set; }

        public string Name { get; private set; } = string.Empty;

        public int ParentId { get; private set; }

        public void Read(BinaryReader reader)
        {
            var length = reader.ReadByte();
            Name = Encoding.UTF8.GetString(reader.ReadBytes(length));
            ParentId = reader.ReadByte();
        }

        public void Write(BinaryWriter writer)
        {
            var buffer = Encoding.UTF8.GetBytes(Name);
            writer.Write((byte)buffer.Length);
            writer.Write(buffer);
            writer.Write((byte)ParentId);
        }
    }
}
