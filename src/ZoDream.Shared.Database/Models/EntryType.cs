namespace ZoDream.Shared.Database
{
    public enum EntryType: byte
    {
        None,
        Password,
        Authentication,
        File,
        Note,
        Wireless,
        ToTp,
        /// <summary>
        /// 自定义，数据结构为 [1: type] [?: data]
        /// </summary>
        Custom = 99,
    }
}
