using System.Diagnostics;
using System.IO;
using System.Text;

namespace ZoDream.Shared.Database
{
    public class FileHeader: IFileFormatter
    {
        const string Signature = "ZRDB";
        public DatabaseVersion Version { get; set; } = DatabaseVersion.V1;
        public byte[] ValidityCode { get; set; } = new byte[16];
        public long GroupOffset { get; set; }
        public int GroupCount { get; set; }

        public long EntryOffset { get; set; }
        public int EntryCount { get; set; }

        public void Read(BinaryReader reader)
        {
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            var buffer = reader.ReadBytes(4);
            Debug.Assert(Encoding.ASCII.GetString(buffer) == Signature);
            Version = (DatabaseVersion)reader.ReadByte();
            reader.Read(ValidityCode);
            GroupOffset = reader.ReadUInt32();
            GroupCount = reader.ReadByte();
            EntryOffset = reader.ReadUInt32() + GroupOffset;
            EntryCount = reader.ReadUInt16();
            reader.BaseStream.Seek(40, SeekOrigin.Begin);
        }

        public void Write(BinaryWriter writer)
        {
            writer.BaseStream.Seek(0, SeekOrigin.Begin);
            writer.Write(Encoding.ASCII.GetBytes(Signature));
            writer.Write((byte)Version);
            writer.Write(ValidityCode);
            if (GroupOffset == 0)
            {
                GroupOffset = writer.BaseStream.Position + 20;
                EntryOffset += GroupOffset;
            }
            writer.Write((uint)GroupOffset);
            writer.Write((byte)GroupCount);
            writer.Write((uint)(EntryOffset - GroupOffset));
            writer.Write((ushort)EntryCount);
            if (writer.BaseStream.Length < 40)
            {
                writer.BaseStream.SetLength(40);
            }
            writer.BaseStream.Seek(40, SeekOrigin.Begin);
        }
    }
}
