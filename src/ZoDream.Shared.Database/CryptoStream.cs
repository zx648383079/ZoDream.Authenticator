using System;
using System.IO;

namespace ZoDream.Shared.Database
{
    public class CryptoStream(Stream input, int blockSize, bool isEnctypt) : Stream
    {

        

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
