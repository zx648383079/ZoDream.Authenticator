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
    /// <summary>
    /// Account 为 appid
    /// </summary>
    public interface IApiEntryEntity : IAccountEntryEntity
    {
        public string Secret { get; }
        public string Url { get; }
        public string Credential { get; }
    }

    public interface IMailEntryEntity : IAccountEntryEntity
    {
        public string Password { get; }
        public string Server { get; }
        public string Port { get; }
        public string Type { get; }
        public string Security { get; }
        public string AuthMethod { get; }

        public string SMTPServer { get; }
        public string SMTPPort { get; }
        public string SMTPUsername { get; }
        public string SMTPPassword { get; }

    }
    /// <summary>
    /// Account 为版本
    /// </summary>
    public interface ILicenseEntryEntity : IAccountEntryEntity
    {
        public string LicenseKey { get; }
        public string LicenseTo { get; }
        public string Email { get; }
        public string Company { get; }
    }

    public interface IServerEntryEntity : IAccountEntryEntity
    {
        public string Password { get; }
        public string Ip { get; }
        public string Port { get; }
    }

    public interface IFtpEntryEntity : IAccountEntryEntity
    {
        public string Password { get; }
        public string Ip { get; }
        public string Port { get; }
        public string Type { get; }
        public string AuthMethod { get; }
    }

    public interface IDatabaseEntryEntity : IAccountEntryEntity
    {
        public string Password { get; }
        public string Server { get; }
        public string Port { get; }
        public string Type { get; }
        public string AuthMethod { get; }
        public string Database { get; }
        public string SID { get; }
        public string Alias { get; }
        public string ConnectionOptions { get; }
    }

    /// <summary>
    /// Account 为文件名
    /// </summary>
    public interface IFileEntryEntity : IAccountEntryEntity
    {

        public string FileName { get; }
    }

    public interface INoteEntryEntity : IEntryEntity
    {
        public string Content { get; }
    }
    /// <summary>
    /// Account 为 WIFI 名
    /// </summary>
    public interface IWirelessEntryEntity : IAccountEntryEntity
    {
        public string Password { get; }
        /// <summary>
        /// 协议
        /// </summary>
        public string Security { get; }
    }

    /// <summary>
    /// Account 为 钱包地址
    /// </summary>
    public interface IWalletEntryEntity : IAccountEntryEntity
    {

        public string Password { get; }

        public string RecoveryPhrase { get; }

    }

    public interface ITOTPEntryEntity : IAccountEntryEntity
    {
        public string Secret { get; }
        public string Url { get; }
        /// <summary>
        /// sha1 
        /// </summary>
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
