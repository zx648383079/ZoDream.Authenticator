namespace ZoDream.Shared.Database
{
    public enum EntryInputType
    {
        String,
        Bool,
        Int,
        DateTime,
        Input,
        Ftp,
        TOTP
    }

    public interface IEntryInput
    {
        public string Header { get; set; }
        public string Value { get; set; }
    }

    public interface IFtpInput
    {
        public string Account { get; set; }
        public string Password { get; }
        public string Port { get; }
        public string Type { get; }
        public string AuthMethod { get; }
    }

    public interface ITOTPInput
    {
        public string Account { get; set; }
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
