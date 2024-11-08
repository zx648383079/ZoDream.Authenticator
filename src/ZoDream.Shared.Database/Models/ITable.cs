namespace ZoDream.Shared.Database
{
    public interface ITableRecord
    {
        public int Id { get; }
        /// <summary>
        /// 入口位置
        /// </summary>
        public long EntryOffset { get; }
        /// <summary>
        /// 占用长度
        /// </summary>
        public long EntryLength { get; }
        /// <summary>
        /// 数据的开启位置，包含EntryOffset
        /// </summary>
        public long EntryDataOffset { get; }
    }
}
