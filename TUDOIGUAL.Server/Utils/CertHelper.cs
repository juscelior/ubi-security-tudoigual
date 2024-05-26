using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using TUDOIGUAL.Lib.Models;

namespace TUDOIGUAL.Server.Utils
{
    public class CertHelper
    {
        /// <summary>
        /// Certificado do servidor
        /// </summary>
        public static X509Certificate2 ServerCertificate = new X509Certificate2($"{GlobalConfiguration.Username}.pfx", string.Empty);

        /// <summary>
        /// Realiza validação do certificado TLS/SSL do cliente
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="certificate">Certificate</param>
        /// <param name="chain">X509Chain</param>
        /// <param name="sslPolicyErrors">SSL policy errs</param>
        /// <returns>True(Validate OK)/False(Validate NG)</returns>
        public static bool ValidateServerCertificate(
            object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // Validate thumbprint
            NodeInfo thumbprintDB = GlobalConfiguration.Clients[((X509Certificate2)certificate).Thumbprint];

            if (((X509Certificate2)certificate).Thumbprint == thumbprintDB.Thumbprint && ((X509Certificate2)certificate).Issuer.Equals($"CN={thumbprintDB.Username}"))
            {
                return true;
            }

            return false;
        }
    }

}
