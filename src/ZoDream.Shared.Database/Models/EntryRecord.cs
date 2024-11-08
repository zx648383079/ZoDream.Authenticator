using System.Linq;

namespace ZoDream.Shared.Database
{
    internal class EntryRecord : ITableRecord
    {
        public int Id { get; set; }

        public EntryType Type { get; set; }

        public int GroupId { get; set; }

        public int[] PropertiesLength { get; set; } = [];

        public bool HasAccount => TypeMapper.HasAccountProperty(Type);
        public bool IsLargeLength => TypeMapper.IsLargeLength(Type);

        public long EntryOffset { get; set; }

        public int ExtraPropertyCount => PropertiesLength.Length - (HasAccount ? 2 : 1);

        protected long InnerDataOffset => 4 + (HasAccount ? 1 : 0) + ExtraPropertyCount * (IsLargeLength ? 4 : 2);

        public long EntryDataOffset => EntryOffset + InnerDataOffset;
        public long EntryLength => InnerDataOffset + PropertiesLength.Sum();
    }
}
