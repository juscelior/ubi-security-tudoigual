using System.Net.Security;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using TUDOIGUAL.Client.Utils;
using System.Collections;
using System.Text;
using TUDOIGUAL.Lib.Utils;

namespace TUDOIGUAL.Client.Services
{
    public class SslSocketClient : IDisposable
    {

        public void Dispose()
        {
        }

        protected async Task<string> SendAsync(string filePath, byte[] clientData)
        {
            string result = string.Empty;

            TcpClient client = new TcpClient(GlobalConfiguration.Server.IP, GlobalConfiguration.Server.Port);

            try
            {
                System.Net.ServicePointManager.SecurityProtocol =
                    SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                SslStream sslStream =
                    new SslStream(client.GetStream(),
                    false, new RemoteCertificateValidationCallback(CertHelper.ValidateServerCertificate), null);

                var certs = new X509Certificate2Collection();
                certs.Add(CertHelper.ClientCertificate);

                try
                {
                    await sslStream.AuthenticateAsClientAsync(GlobalConfiguration.Server.IP, certs, System.Security.Authentication.SslProtocols.Tls12, true);

                    #region Send without callback
                    await SendDataInChunksAsync(sslStream, clientData, 1024);
                    #endregion

                    #region Receive response from server

                    // Read with a enough buffer
                    // Note by using TLS 1 by System.Security.Authentication.SslProtocols.Tls, the stream will first get 1 byte followed by the rest of the bytes.
                    // So this way wont work on TLS 1, use recursively-read way instead.
                    // See https://stackoverflow.com/a/48753002/7045253
                    byte[] rtnBytes = new byte[1024]; // Data buffer for incoming data
                    int bytesRec = sslStream.Read(rtnBytes);


                    if (filePath.Equals("ALL.ALL"))
                    {
                        byte[] eofBytes = Encoding.UTF8.GetBytes(GlobalConfiguration.Eof);
                        int? index = ByteHelper.GetEndIndexOfSequence(rtnBytes, eofBytes);

                        rtnBytes = ByteHelper.SplitByteArray(rtnBytes, 0, index.Value);

                        result = System.Text.Encoding.UTF8.GetString(rtnBytes);
                    }
                    else
                    {
                        File.WriteAllBytes(filePath, rtnBytes);
                        Console.WriteLine($"Recebido do servidor: {filePath}");
                    }
                    
                    #endregion
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    sslStream.Close();
                    client.Close();
                }
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }

            return result;
        }

        static async Task SendDataInChunksAsync(SslStream sslStream, byte[] data, int chunkSize)
        {
            int offset = 0;
            while (offset < data.Length)
            {
                int size = Math.Min(chunkSize, data.Length - offset);
                await sslStream.WriteAsync(data, offset, size);
                await sslStream.FlushAsync();
                offset += size;
            }
        }
    }

}
