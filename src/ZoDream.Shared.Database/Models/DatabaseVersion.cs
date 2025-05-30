namespace ZoDream.Shared.Database
{
    public enum DatabaseVersion: byte
    {
        Unknown,
        V1 = 1,
        WithRecovery = 2, // 带恢复码
    }
}
