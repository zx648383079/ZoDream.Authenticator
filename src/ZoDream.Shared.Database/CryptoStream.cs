using System;
using System.IO;

namespace ZoDream.Shared.Database
{
    public class CryptoStream(Stream input, ICipher cipher, int blockSize, bool isEnctypt) : Stream
    {
        /// <summary>
        /// 存储解密后的结果，例如 rsa 编码后可能比 blockSize 大8个子节 
        /// </summary>
        private readonly byte[] _buffer = new byte[blockSize + 256];
        private long _bufferBegin;
        private long _bufferSize;
        private long _bufferOffset;

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => input.Length;

        public override long Position 
        { 
            get => throw new NotImplementedException(); 
            set => throw new NotImplementedException(); 
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var res = 0;
            while (res < count)
            {
                if (_bufferSize == 0 || _bufferOffset >= _bufferSize)
                {
                    _bufferBegin = input.Position;
                    var readLen = input.Read(_buffer, 0, blockSize);
                    if (isEnctypt)
                    {
                        _bufferSize = cipher.Encrypt(_buffer, 0, readLen);
                    } else
                    {
                        _bufferSize = cipher.Decrypt(_buffer, 0, readLen);
                    }
                    _bufferOffset = 0;
                }
                if (_bufferSize == 0)
                {
                    break;
                }
                var i = _bufferOffset;
                var len = (int)Math.Min(_bufferSize - i, count - res);
                if (len <= 0)
                {
                    break;
                }
                Array.Copy(_buffer, i, buffer, offset + res, len);
                res += len;
                _bufferOffset += len;
            }
            return res;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void Flush()
        {
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        protected override void Dispose(bool disposing)
        {
            input?.Dispose();
            base.Dispose(disposing);
        }
    }
}
