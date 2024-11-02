using System.IO;
using System.Text;

namespace ZoDream.Shared.Database
{
    public class EntryItem : IFileFormatter
    {
        public int Id { get; set; }

        public EntryType Type { get; set; }

        public string Title { get; set; } = string.Empty;

        public int GroupId { get; set; }

        public string Account { get; set; } = string.Empty;

        public long Offset { get; set; }

        public long PropertyOffset { get; set; }

        public int PropertyCount { get; set; }

        public bool HasAccount => Type != EntryType.File && Type != EntryType.Note;

        public void Read(BinaryReader reader)
        {
            Type = (EntryType)reader.ReadByte();
            PropertyCount = reader.ReadByte();
            var length = reader.ReadByte();
            Title = Encoding.UTF8.GetString(reader.ReadBytes(length));
            GroupId = reader.ReadByte();
            if (HasAccount)
            {
                length = reader.ReadByte();
                Account = Encoding.UTF8.GetString(reader.ReadBytes(length));
            }
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write((byte)Type);
            writer.Write((byte)PropertyCount);
            var buffer = Encoding.UTF8.GetBytes(Title);
            writer.Write((byte)buffer.Length);
            writer.Write(buffer);
            writer.Write((byte)GroupId);
            if (HasAccount)
            {
                buffer = Encoding.UTF8.GetBytes(Account);
                writer.Write((byte)buffer.Length);
                writer.Write(buffer);
            }
            
        }
    }
}
