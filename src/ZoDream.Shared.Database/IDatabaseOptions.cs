namespace ZoDream.Shared.Database
{
    public interface IDatabaseOptions
    {
        public string FileName { get; }
        public string Password { get; }
        public string KeyFileName { get; }
    }
}
