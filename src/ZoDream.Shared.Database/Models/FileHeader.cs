using System.Diagnostics;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Xml.Linq;

namespace ZoDream.Shared.Database
{
    public class FileHeader: IFileFormatter
    {
        const string Signature = "ZRDB";
        public DatabaseVersion Version { get; set; } = DatabaseVersion.V1;
        public byte[] ValidityCode { get; set; } = new byte[16];
        /// <summary>
        /// group 开始的位置
        /// </summary>
        public long GroupOffset { get; set; }
        public int GroupCount { get; set; }
        /// <summary>
        /// 需要加上 GroupOffset 才是实际位置
        /// </summary>
        public long EntryOffset { get; set; }
        public int EntryCount { get; set; }

        private bool _isLoaded = false;

        /// <summary>
        /// entry 实际开始的位置
        /// </summary>
        public long EntryRealOffset => GroupOffset + EntryOffset;


        public void Read(BinaryReader reader)
        {
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            var buffer = reader.ReadBytes(4);
            Debug.Assert(Encoding.ASCII.GetString(buffer) == Signature);
            Version = (DatabaseVersion)reader.ReadByte();
            reader.Read(ValidityCode);
            GroupOffset = reader.ReadUInt32();
            GroupCount = reader.ReadByte();
            EntryOffset = reader.ReadUInt32();
            EntryCount = reader.ReadUInt16();
            _isLoaded = true;
        }

        public void Write(BinaryWriter writer)
        {
            if (_isLoaded)
            {
                WritePart(writer);
                return;
            }
            writer.BaseStream.Seek(0, SeekOrigin.Begin);
            writer.Write(Encoding.ASCII.GetBytes(Signature));
            writer.Write((byte)Version);
            writer.Write(ValidityCode);
            writer.Write((uint)GroupOffset);
            writer.Write((byte)GroupCount);
            writer.Write((uint)EntryOffset);
            writer.Write((ushort)EntryCount);
            _isLoaded = true;
        }

        public void WritePart(BinaryWriter writer)
        {
            writer.BaseStream.Seek(21, SeekOrigin.Begin);
            writer.Write((uint)GroupOffset);
            writer.Write((byte)GroupCount);
            writer.Write((uint)EntryOffset);
            writer.Write((ushort)EntryCount);
        }
    }
}
