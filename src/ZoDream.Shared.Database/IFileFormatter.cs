using System.IO;

namespace ZoDream.Shared.Database
{
    public interface IFileFormatter
    {
        public void Read(BinaryReader reader);
        public void Write(BinaryWriter writer);
    }
}
