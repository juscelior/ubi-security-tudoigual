using TUDOIGUAL.Server.Models;

namespace TUDOIGUAL.Server.Utils.Handler
{
    public interface IRequestHandler : IDisposable
    {
        Task<byte[]> HandleAsync(IStateObject state);
    }
}
