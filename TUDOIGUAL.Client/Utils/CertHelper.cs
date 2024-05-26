using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace TUDOIGUAL.Client.Utils
{
    public class CertHelper
    {
        public static X509Certificate2 ClientCertificate = new X509Certificate2($"{GlobalConfiguration.Username}.pfx");

        public static bool ValidateServerCertificate(
            object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (((X509Certificate2)certificate).Thumbprint == GlobalConfiguration.Server.Thumbprint && ((X509Certificate2)certificate).Issuer.Equals($"CN={GlobalConfiguration.Server.Username}"))
            {
                return true;
            }

            return false;
        }
    }

}
