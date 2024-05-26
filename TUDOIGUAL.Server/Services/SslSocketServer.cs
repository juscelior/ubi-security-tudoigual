using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using TUDOIGUAL.Server.Models;
using TUDOIGUAL.Server.Utils;
using TUDOIGUAL.Server.Utils.Handler;

namespace TUDOIGUAL.Server.Services
{
    public static class SslSocketServer
    {
        public static ManualResetEvent eventSignal = null;

        public static TcpListener TcpListener = null;

        private const int MaxQueuedClientNumber = 100; // Maximo de clientes
        private const int StreamReadTimeout = 5000;
        private const int StreamWriteTimeout = 5000;

        internal static bool IsClosed { get; set; }

        #region Constructor

        static SslSocketServer()
        {
            eventSignal = new ManualResetEvent(false);
            TcpListener = new TcpListener(System.Net.IPAddress.Any, GlobalConfiguration.Port);
        }
        #endregion

        #region Start Socket Server

        internal static void Start()
        {
            IsClosed = false;
            TcpListener.Start(MaxQueuedClientNumber);
        }
        #endregion

        #region Stop Socket server

        internal static void Stop()
        {
            TcpListener.Stop();
            IsClosed = true;
        }
        #endregion

        #region Listen

        internal static void Listen()
        {
            eventSignal.Reset();

            Console.WriteLine("[SSL Socket] Waiting for a request...");

            try
            {
                TcpListener.BeginAcceptTcpClient(new AsyncCallback(AcceptCallback), TcpListener);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to listen {ex.Message}");
            }

            eventSignal.WaitOne();
        }
        #endregion

        private static void AcceptCallback(IAsyncResult ar)
        {
            eventSignal.Set();

            TcpListener listener = (TcpListener)ar.AsyncState;
            TcpClient handler = listener.EndAcceptTcpClient(ar);

            var sslStream = new System.Net.Security.SslStream(
                handler.GetStream(), false, new RemoteCertificateValidationCallback(CertHelper.ValidateServerCertificate), null);
            sslStream.ReadTimeout = StreamReadTimeout;
            sslStream.WriteTimeout = StreamWriteTimeout;

            try
            {
                sslStream.AuthenticateAsServer(CertHelper.ServerCertificate, true, SslProtocols.Tls12, false);

                var state = new SslStreamState();
                state.SslStream = sslStream;

                sslStream.BeginRead(state.Buffer, 0, state.BufferSize, new AsyncCallback(ReadCallback), state);
            }
            catch (Exception ex)
            {
                sslStream.Close();
                handler.Close();
                Console.WriteLine(ex.ToString() );
            }
        }

        #region ReadCallback

        private static void ReadCallback(IAsyncResult ar)
        {
            byte[] eofBytes = Encoding.UTF8.GetBytes(GlobalConfiguration.Eof);

            SslStreamState state = (SslStreamState)ar.AsyncState;
            System.Net.Security.SslStream handler = state.SslStream;

            int bytesRead = handler.EndRead(ar);

            if (bytesRead > 0)
            {
                state.ContentBuffer.AddRange(state.Buffer);

                byte[] content = state.ContentBuffer.ToArray();


                int? index = ByteHelper.GetEndIndexOfSequence(content, eofBytes);
                if (index != null)
                {
                    state.Content = ByteHelper.SplitByteArray(content, 0, index.Value);

                    using var requestHandler = new FileRequestHandler();
                    var file = requestHandler.HandleAsync(state).GetAwaiter().GetResult();

                    Send(handler, file);
                }
                else
                {
                    handler.BeginRead(state.Buffer, 0, state.BufferSize, new AsyncCallback(ReadCallback), state);
                }
            }
        }

        

        #endregion

        #region Send

        private static void Send(System.Net.Security.SslStream handler, byte[] byteData)
        {
            // Begin sending the data to the remote device
            handler.BeginWrite(byteData, 0, byteData.Length, new AsyncCallback(SendCallback), handler);
        }
        #endregion

        #region SendCallback

        private static void SendCallback(IAsyncResult ar)
        {
            // Retrieve the socket from the state object
            System.Net.Security.SslStream handler = (System.Net.Security.SslStream)ar.AsyncState;

            // Complete sending the data to the remote device
            handler.EndWrite(ar);

            handler.ShutdownAsync().Wait();
            handler.Close();
        }
        #endregion
    }
}
