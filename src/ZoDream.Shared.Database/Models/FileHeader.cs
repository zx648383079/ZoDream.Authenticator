using System.Diagnostics;
using System.IO;
using System.Text;

namespace ZoDream.Shared.Database
{
    public class FileHeader: IFileFormatter
    {
        const string Signature = "ZRDB";
        public DatabaseVersion Version { get; private set; }
        public string ValidityCode { get; private set; } = string.Empty;
        public long GroupOffset { get; private set; }
        public int GroupCount { get; private set; }

        public long EntryOffset { get; private set; }
        public int EntryCount { get; private set; }

        public long EntryDataOffset { get; private set; }

        public void Read(BinaryReader reader)
        {
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            var buffer = reader.ReadBytes(4);
            Debug.Assert(Encoding.ASCII.GetString(buffer) == Signature);
            Version = (DatabaseVersion)reader.ReadByte();
            ValidityCode = Encoding.ASCII.GetString(reader.ReadBytes(32));
            GroupOffset = reader.ReadUInt32();
            GroupCount = reader.ReadByte();
            EntryOffset = reader.ReadUInt32() + GroupOffset;
            EntryCount = reader.ReadUInt16();
            EntryDataOffset = reader.ReadUInt32() + EntryOffset;
        }

        public void Write(BinaryWriter writer)
        {
            writer.BaseStream.Seek(0, SeekOrigin.Begin);
            writer.Write(Encoding.ASCII.GetBytes(Signature));
            writer.Write((byte)Version);
            writer.Write(Encoding.ASCII.GetBytes(ValidityCode));
            writer.Write((uint)GroupOffset);
            writer.Write((byte)GroupCount);
            writer.Write((uint)(EntryOffset - GroupOffset));
            writer.Write((ushort)EntryCount);
            writer.Write((uint)(EntryDataOffset - EntryOffset));
        }
    }
}
