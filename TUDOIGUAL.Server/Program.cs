using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TUDOIGUAL.Lib.Utils;
using TUDOIGUAL.Server;
using TUDOIGUAL.Server.Utils;

internal class Program
{
    private static void Main(string[] args)
    {

        string directoryPath = string.Empty;
        string newUsername = string.Empty;
        string ip = string.Empty;
        int port = 6667;

        // Processar argumentos do console
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-dir" && i + 1 < args.Length)
            {
                directoryPath = args[i + 1];
                i++;
            }
            else if (args[i] == "-new-user" && i + 1 < args.Length)
            {
                newUsername = args[i + 1];
                i++;
            }
            else if (args[i] == "-ip" && i + 1 < args.Length)
            {
                ip = args[i + 1];
                i++;
            }
            else if (args[i] == "-port" && i + 1 < args.Length)
            {
                port = int.Parse(args[i + 1]);
                i++;
            }
        }

#if DEBUG
        directoryPath = "C:\\git\\ubi2\\TUDOIGUAL\\TUDOIGUAL.Server\\bin\\Release\\net8.0\\win-x64\\publish\\Upload";
        //newUsername  = "juscelio";
        ip = "127.0.0.1";
        port = 6667;
#endif

        GlobalConfiguration.UploadFolder = directoryPath;
        GlobalConfiguration.Eof = "<EOF>";
        GlobalConfiguration.Key = "<KEY>";

        GlobalConfiguration.Port = port;
        GlobalConfiguration.Username = "server";
        GlobalConfiguration.UpdateClientsFileWithPfxCertificates();
        GlobalConfiguration.LoadClients();

        try
        {
            Console.WriteLine("Iniciando...");

            RsaKeyHelper rsaKeyGeneration = new RsaKeyHelper();

            if (newUsername != string.Empty)
            {
                rsaKeyGeneration.CreateKeyPair(newUsername, string.Empty, ip, true);
            }
            else
            {

                rsaKeyGeneration.CreateKeyPair(GlobalConfiguration.Username, string.Empty, ip, false);

                CreateHostBuilder(args).Build().Run();
            }



            Console.ReadKey();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        Console.ReadKey();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            services.AddHostedService<SslSocketWorker>();
        });
}