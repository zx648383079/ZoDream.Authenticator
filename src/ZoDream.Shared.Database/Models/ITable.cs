namespace ZoDream.Shared.Database
{
    public interface IRecordSource
    {
        public int Id { get; }
        /// <summary>
        /// 是否是原始文件，还是临时文件
        /// </summary>
        public RecordSourceType SourceType { get; }
        /// <summary>
        /// 入口位置
        /// </summary>
        public long EntryOffset { get; }
        /// <summary>
        /// 占用长度
        /// </summary>
        public long EntryLength { get; }
    }
    public interface ITableRecord : IRecordSource
    {
        /// <summary>
        /// 数据的开启位置，包含EntryOffset
        /// </summary>
        public long EntryDataOffset { get; }
    }

    public enum RecordSourceType
    {
        Original,
        Temporary
    }
}
