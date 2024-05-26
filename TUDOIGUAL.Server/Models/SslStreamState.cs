using System.Net.Security;

namespace TUDOIGUAL.Server.Models
{
    public class SslStreamState : IStateObject
    {
        public SslStream SslStream = null;

        private const int FixedBufferSize = 1024;

        public SslStreamState()
        {
            this.Buffer = new byte[this.BufferSize];
            this.ContentBuffer = new List<byte>(this.BufferSize);
        }

        public int BufferSize
        {
            get { return FixedBufferSize; }
        }

        public byte[] Buffer { get; set; }

        public List<byte> ContentBuffer { get; set; }

        public byte[] Content { get; set; }
    }

}
