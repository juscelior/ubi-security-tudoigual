using System;
using System.Security.Cryptography;
using TUDOIGUAL.Client.Services;
using TUDOIGUAL.Client.Utils;
using TUDOIGUAL.Lib.Utils;

internal class Program
{

    private static async Task Main(string[] args)
    {
        string filename = null;
        string ip = null;
        string thumbprint = null;
        int port = 6667;
        string output = null;


        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-dir" && i + 1 < args.Length)
            {
                GlobalConfiguration.SyncFolder = args[i + 1];
                i++;
            }
            else if (args[i] == "-user" && i + 1 < args.Length)
            {
                GlobalConfiguration.Username = args[i + 1];
                i++;
            }
            else if (args[i] == "-filename" && i + 1 < args.Length)
            {
                filename = args[i + 1];
                i++;
            }
            else if (args[i] == "-out" && i + 1 < args.Length)
            {
                output = args[i + 1];
                i++;
            }

            else if (args[i] == "-ip" && i + 1 < args.Length)
            {
                ip = args[i + 1];
                i++;
            }

            else if (args[i] == "-thumbprint" && i + 1 < args.Length)
            {
                thumbprint = args[i + 1];
                i++;
            }
            else if (args[i] == "-port" && i + 1 < args.Length)
            {
                port = int.Parse(args[i + 1]);
                i++;
            }
        }

        GlobalConfiguration.Eof = "<EOF>";
        GlobalConfiguration.Key = "<KEY>";

#if DEBUG
        GlobalConfiguration.SyncFolder = "C:\\git\\ubi2\\TUDOIGUAL\\TUDOIGUAL.Client\\bin\\Debug\\net8.0\\Files";
        GlobalConfiguration.Username = "juscelio";
        thumbprint = "B32311464613ECD03A0A0A95140B37189F61A929";
        ip = "localhost";
        port = 6667;
        //filename = "C:\\git\\ubi2\\TUDOIGUAL\\TUDOIGUAL.Client\\bin\\Debug\\net8.0\\Files\\Demo - Copia.txt.enc";
        output = "C:\\git\\ubi2\\TUDOIGUAL\\TUDOIGUAL.Client\\bin\\Debug\\net8.0\\";
        //newUser = true;
#endif
        GlobalConfiguration.LoadServer(thumbprint, ip, port);

        try
        {
            Console.WriteLine("Iniciando...");

            RsaKeyHelper rsaKeyGeneration = new RsaKeyHelper();
            rsaKeyGeneration.CreateKeyPair(GlobalConfiguration.Username, string.Empty, ip, false);

            Thread timerThread = new Thread(StartTimer);
            timerThread.IsBackground = true; // Definir a thread como thread em segundo plano
            timerThread.Start();


            if (!string.IsNullOrEmpty(filename))
            {
                DecriptLocalFile(filename, output);
            }
            else
            {
                Console.WriteLine("Start Socket client...");

                if (!Directory.Exists(GlobalConfiguration.SyncFolder))
                {
                    Directory.CreateDirectory(GlobalConfiguration.SyncFolder);
                }

                using (FileSystemWatcher watcher = new FileSystemWatcher())
                {
                    watcher.Path = GlobalConfiguration.SyncFolder;

                    watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;

                    watcher.Created += OnCreated;

                    watcher.EnableRaisingEvents = true;

                    Console.WriteLine($"Monitoring folder: {GlobalConfiguration.SyncFolder}");
                    Console.WriteLine("Press 'q' to quit.");

                    // Continue rodando até que o usuário decida parar
                    while (Console.Read() != 'q') ;
                }
            }

            Console.ReadKey();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        Console.ReadKey();
    }

    private static void DecriptLocalFile(string filename, string output)
    {
        using var fileSender = new FileSender();
        fileSender.GetKeyAsync(filename).GetAwaiter().GetResult();

        RSA rsa = RsaKeyHelper.LoadPrivateKey(GlobalConfiguration.Username);

        using (FileStream fsEncryptedKey = new FileStream(filename + ".encKey", FileMode.Open, FileAccess.Read))
        {
            byte[] encryptedSimetricKey = new byte[256];

            fsEncryptedKey.Read(encryptedSimetricKey, 0, encryptedSimetricKey.Length);

            byte[] combined = RsaKeyHelper.DecryptWithPublicKey(rsa, encryptedSimetricKey);

            string name = Path.GetFileName(filename).Replace(".enc", "");

            RsaKeyHelper.DecryptFile(filename, Path.Combine(output, name), combined);
        }
    }

    private static void OnCreated(object sender, FileSystemEventArgs e)
    {
        if (!e.FullPath.EndsWith(".enc") 
            && !e.FullPath.EndsWith(".encKey")
            && !e.FullPath.EndsWith(".2")
            && !e.FullPath.EndsWith(".sig"))
        {

            
            Console.WriteLine($"Novo ficheiro: {e.FullPath}");

            Thread.Sleep(3000);


            (string encryptedFilePath, byte[] simetricKey) =  RsaKeyHelper.EncryptFile(e.FullPath);

            RSA rsa = RsaKeyHelper.LoadPrivateKey(GlobalConfiguration.Username);
            byte[] encryptedSimetricKey = RsaKeyHelper.EncryptWithPrivateKey(rsa, simetricKey);

            using var fileSender = new FileSender();
            fileSender.SendFileAsync(encryptedFilePath, encryptedSimetricKey).GetAwaiter().GetResult();

            File.Delete(e.FullPath);
        }
    }

    static void StartTimer()
    {
        Timer timer = new Timer(SyncCallback, null, 0, 15000);
    }

    private static void SyncCallback(Object state)
    {

        using var fileSender = new FileSender();
        fileSender.GetSyncAsync().GetAwaiter().GetResult();

        // Adicione aqui o código que você deseja executar a cada 5 segundos
    }
}