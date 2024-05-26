using System.Security.Cryptography.X509Certificates;
using TUDOIGUAL.Lib.Models;

namespace TUDOIGUAL.Server.Utils
{
    public class GlobalConfiguration
    {
        public static string Username { get; set; }

        public static string UploadFolder { get; set; }

        public static string Eof { get; set; }

        public static string Key { get; set; }

        public static int Port { get; set; }

        public static Dictionary<string, NodeInfo> Clients { get; set; }


        public static void LoadClients()
        {
            string filePath = "clients.txt";
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Arquivo não encontrado: {filePath}");
                return;
            }

            GlobalConfiguration.Clients = File.ReadAllLines(filePath)
                              .Select(line => line.Split(','))
                              .Select(parts => new NodeInfo
                              {
                                  Thumbprint = parts[0].Trim(),
                                  IP = parts[1].Trim(),
                                  Username = parts[2].Trim()
                              })
                              .ToList().ToDictionary(k => k.Thumbprint);
        }

        public static void UpdateClientsFileWithPfxCertificates()
        {
            string filePath = "clients.txt";

            FileInfo fileInfo = new FileInfo(filePath);
                
            // Obter todos os arquivos .pfx no diretório especificado
            string[] pfxFiles = Directory.GetFiles(fileInfo.Directory.FullName, "*.pfx");

            foreach (var pfxFile in pfxFiles)
            {
                // Importar o certificado do arquivo PFX
                var certificate = new X509Certificate2(pfxFile, String.Empty, X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);

                // Obter o thumbprint do certificado
                string thumbprint = certificate.Thumbprint;

                // Ler todas as linhas do arquivo clients.txt
                var lines = File.Exists(filePath) ? File.ReadAllLines(filePath).ToList() : new List<string>();

                // Verificar se o thumbprint já existe no arquivo
                bool thumbprintExists = lines.Any(line => line.Split(',')[0].Trim() == thumbprint);

                if (!thumbprintExists)
                {
                    // Adicionar nova linha ao arquivo clients.txt
                    string newLine = $"{thumbprint}, 127.0.0.1, {Path.GetFileNameWithoutExtension(pfxFile)}";
                    lines.Add(newLine);
                    File.WriteAllLines(filePath, lines);
                    Console.WriteLine($"Adicionado: {newLine}");
                }
                else
                {
                    Console.WriteLine($"Thumbprint já existe: {thumbprint}");
                }
            }
        }
    }
}
