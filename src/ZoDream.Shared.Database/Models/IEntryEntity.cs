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

    public interface ILoginEntryEntity : IAccountEntryEntity
    {
        public string Password { get; }
        public string Url { get; }
    }
    public interface IFileEntryEntity : IEntryEntity
    {
        public string FileName { get; }
    }

    public interface INoteEntryEntity : IEntryEntity
    {
        public string Content { get; }
    }

    public interface ITOTPEntryEntity : IAccountEntryEntity
    {
        public string Secret { get; }
        public string Url { get; }
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
