using System.IO;

namespace ZoDream.Shared.Database
{
    public interface ICipherIV
    {
        /// <summary>
        /// 把iv写入文件中
        /// </summary>
        /// <param name="input"></param>
        public void Write(Stream input);
        /// <summary>
        /// 从文件读取iv
        /// </summary>
        /// <param name="input"></param>
        public void Read(Stream input);
    }
}
