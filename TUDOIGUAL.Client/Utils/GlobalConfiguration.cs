using TUDOIGUAL.Lib.Models;

namespace TUDOIGUAL.Client.Utils
{
    public class GlobalConfiguration
    {
        public static string Username { get; set; }

        public static string SyncFolder { get; set; }

        public static string Eof { get; set; }
        public static string Key { get; set; }


        public static NodeInfo Server { get; set; }


        public static void LoadServer(string thumbprint, string ip, int port)
        {
            Server = new NodeInfo
            {
                Thumbprint = thumbprint,
                IP = ip,
                Username = "server",
                Port = port
            };
        }
    }
}
