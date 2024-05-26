using Microsoft.Extensions.Hosting;
using TUDOIGUAL.Server.Services;

namespace TUDOIGUAL.Server
{
    public class SslSocketWorker : BackgroundService
    {
        private const string SRV_START_MSG = "Iniciando o servidor...";
        private const string SRV_STOPPED_MSG = "O servidor está parado.";

        public SslSocketWorker()
        {
        }

        protected override async Task ExecuteAsync(CancellationToken cancelToken)
        {
            Console.WriteLine(SRV_START_MSG);

            SslSocketServer.Start();

            while (!cancelToken.IsCancellationRequested)
            {
                SslSocketServer.Listen();
            }

            SslSocketServer.Stop();
            Console.WriteLine(SRV_STOPPED_MSG);

            await Task.CompletedTask;
        }
    }

}
