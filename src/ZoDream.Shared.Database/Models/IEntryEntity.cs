namespace ZoDream.Shared.Database
{
    public interface IEntryEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public int GroupId { get; set; }
    }

    public interface IAccountEntryEntity: IEntryEntity
    {
        public string Account { get; set; }
    }

    public interface IPasswordEntryEntity : IAccountEntryEntity
    {
        public string Password { get; }
        public string Url { get; }
    }

    public interface IAuthEntryEntity : IPasswordEntryEntity
    {
        public string Email { get; }
        public string Mobile { get; }
    }

    public interface IFileEntryEntity : IEntryEntity
    {
        public string FileName { get; }
    }

    public interface INoteEntryEntity : IEntryEntity
    {
        public string Content { get; }
    }

    public interface IWirelessEntryEntity : IAccountEntryEntity
    {
        public string Password { get; }
        /// <summary>
        /// 协议
        /// </summary>
        public string Security { get; }
    }
    public interface ITOTPEntryEntity : IAccountEntryEntity
    {
        public string Secret { get; }
        public string Url { get; }

        public string Algorithm { get; }
        /// <summary>
        /// 动态码位数
        /// </summary>
        public int Period { get; }
        /// <summary>
        /// 有效期/s
        /// </summary>
        public int Digits { get; }
    }
}
