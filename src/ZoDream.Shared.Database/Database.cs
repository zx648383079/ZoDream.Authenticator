using System;
using System.Collections.Generic;
using System.IO;

namespace ZoDream.Shared.Database
{
    public partial class Database : IDatabase
    {
        public Database(IDatabaseOptions options)
        {
            _options = options;
        }

        private readonly IDatabaseOptions _options;
        private FileBuilder? _builder;

        public void Create()
        {
            _builder = new FileBuilder(_options.FileName, Convert(_options), false);
        }

        public void Open()
        {
            _builder = new FileBuilder(_options.FileName, Convert(_options), true);
        }

        /// <summary>
        /// 需要更新对应id
        /// </summary>
        public void Flush()
        {
            _builder!.Flush();
        }

        public void Dispose()
        {
            _builder?.Dispose();
        }

        public static ICipher Convert(IDatabaseOptions options)
        {
            var items = new List<ICipher>();
            if (!string.IsNullOrWhiteSpace(options.Password))
            {
                items.Add(new PasswordCipher(options.Password));
            }
            if (!string.IsNullOrWhiteSpace(options.KeyFileName) && File.Exists(options.KeyFileName))
            {
                items.Add(new FileCipher(options.KeyFileName));
            }
            if (items.Count == 0) 
            {
                throw new ArgumentNullException(nameof(options));
            }
            return new MixCipher([..items]);
        }
    }
}
