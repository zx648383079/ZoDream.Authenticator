namespace ZoDream.Shared.Database
{
    internal class GroupRecord : ITableRecord
    {
        internal const int BeginIndex = 20;

        public int Id { get; set; }

        public int ParentId { get; set; }

        public int NameLength { get; set; }

        public long EntryOffset { get; set; }

        public long EntryDataOffset => EntryOffset + 2;

        public long EntryLength => 2 + NameLength;
    }
}
