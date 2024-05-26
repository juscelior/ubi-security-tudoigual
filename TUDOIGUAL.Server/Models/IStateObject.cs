namespace TUDOIGUAL.Server.Models
{
    public interface IStateObject
    {
        int BufferSize { get; }

        byte[] Buffer { get; set; }

        public List<byte> ContentBuffer { get; set; }

        public byte[] Content { get; set; }
    }
}
