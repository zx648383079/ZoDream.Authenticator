namespace ZoDream.Shared.Database
{
    public class FileHeader
    {
        internal const string Signature = "ZRDB";
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

        /// <summary>
        /// entry 实际开始的位置
        /// </summary>
        public long EntryRealOffset => GroupOffset + EntryOffset;


    }
}
