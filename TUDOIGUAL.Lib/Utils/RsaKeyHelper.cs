using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

namespace TUDOIGUAL.Lib.Utils
{
    public class RsaKeyHelper
    {
        public X509Certificate2 CreateKeyPair(string commonName, string pfxPassword, string ip, bool addToFile)
        {
            string pfxPath = commonName + ".pfx";
            X509Certificate2 cert = null;

            if (!File.Exists(pfxPath))
            {
                using (RSA rsa = RSA.Create(2048))
                {
                    cert = GenerateAndSaveCertificate(rsa, commonName, pfxPath, pfxPassword);

                    Console.WriteLine($"Thumbprint: {cert.Thumbprint}");
                    Console.WriteLine($"Issuer: {cert.Issuer}");

                    if (addToFile)
                    {
                        AddClientIfNotExists("clients.txt", cert.Thumbprint, ip, commonName);
                    }

                }
            }

            return cert;
        }

        static void SavePrivateKey(string path, RSA rsa)
        {
            var privateKeyBytes = rsa.ExportRSAPrivateKey();
            File.WriteAllBytes(path, privateKeyBytes);
            Console.WriteLine($"Chave privada salva em: {path}");
        }

        static void SavePublicKey(string path, string commonName, RSA rsa)
        {
            var certificate = GenerateSelfSignedCertificate(rsa, commonName);
            var publicKey = certificate.Export(X509ContentType.Cert);
            File.WriteAllBytes(path, publicKey);
            Console.WriteLine($"Certificado com chave pública salvo em: {path}");
        }

        static X509Certificate2 GenerateSelfSignedCertificate(RSA rsa, string commonName)
        {
            var distinguishedName = new X500DistinguishedName($"CN={commonName}");
            var request = new CertificateRequest(distinguishedName, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            var certificate = request.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(5));
            return certificate;
        }

        public static RSA LoadPrivateKey(string commonName)
        {
            string pfxPath = commonName + ".pfx";
            var certificate = new X509Certificate2(pfxPath, string.Empty, X509KeyStorageFlags.Exportable);

            RSA privateKey = certificate.GetRSAPrivateKey();

            return privateKey;
        }

        public static void SignDocument(RSA rsa, string documentPath, string signaturePath)
        {
            byte[] documentBytes = File.ReadAllBytes(documentPath);
            byte[] signatureBytes = rsa.SignData(documentBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            File.WriteAllBytes(signaturePath, signatureBytes);
            Console.WriteLine($"Documento assinado salvo em: {signaturePath}");
        }

        static X509Certificate2 GenerateAndSaveCertificate(RSA rsa, string commonName, string pfxPath, string password)
        {

            var distinguishedName = new X500DistinguishedName($"CN={commonName}");
            var request = new CertificateRequest(distinguishedName, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            request.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, true));
            request.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(request.PublicKey, false));
            request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, true));
            request.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(
                new OidCollection { new Oid("1.3.6.1.5.5.7.3.2") }, true));

            X509Certificate2 clientCert = request.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(5));


            byte[] pfxBytes = clientCert.Export(X509ContentType.Pfx);
            File.WriteAllBytes(pfxPath, pfxBytes);
            Console.WriteLine($"Certificado cliente salvo em: {pfxPath}");

            return clientCert;
        }

        static X509Certificate2 LoadCertificate(string pfxPath, string password)
        {
            return new X509Certificate2(pfxPath, password, X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
        }

        public static (string, byte[]) EncryptFile(string filePath)
        {
            string encryptedFilePath = filePath + ".enc";
            byte[] simetricKey;

            using (Aes aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.GenerateKey();
                aes.GenerateIV();

                byte[] key = aes.Key;
                byte[] iv = aes.IV;

                simetricKey = CombineArrays(key, iv);

                using (FileStream fsInput = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                using (FileStream fsEncrypted = new FileStream(encryptedFilePath, FileMode.Create, FileAccess.Write))
                {
                    using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                    using (CryptoStream csEncrypt = new CryptoStream(fsEncrypted, encryptor, CryptoStreamMode.Write))
                    {
                        fsInput.CopyTo(csEncrypt);
                    }
                }
            }

            return (encryptedFilePath, simetricKey);
        }

        public static void DecryptFile(string encryptedFilePath,string decryptedFilePath, byte[] combined)
        {
            using (FileStream fsEncrypted = new FileStream(encryptedFilePath, FileMode.Open, FileAccess.Read))
            {
                byte[] key = new byte[32]; // 256 bits
                byte[] iv = new byte[16];  // 128 bits

                Array.Copy(combined, 0, key, 0, key.Length);
                Array.Copy(combined, key.Length, iv, 0, iv.Length);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = iv;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    using (CryptoStream csDecrypt = new CryptoStream(fsEncrypted, decryptor, CryptoStreamMode.Read))
                    using (FileStream fsDecrypted = new FileStream(decryptedFilePath, FileMode.Create, FileAccess.Write))
                    {
                        csDecrypt.CopyTo(fsDecrypted);
                    }
                }
            }
        }


        public static byte[] EncryptWithPrivateKey(RSA rsa, byte[] data)
        {
            return rsa.Encrypt(data, RSAEncryptionPadding.Pkcs1);
        }

        public static byte[] DecryptWithPublicKey(RSA rsa, byte[] data)
        {
            return rsa.Decrypt(data, RSAEncryptionPadding.Pkcs1);
        }

        static byte[] CombineArrays(byte[] first, byte[] second)
        {
            byte[] result = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, result, 0, first.Length);
            Buffer.BlockCopy(second, 0, result, first.Length, second.Length);
            return result;
        }



        static void AddClientIfNotExists(string filePath, string thumbprint, string ip, string username)
        {
            // Verifica se o arquivo existe
            if (!File.Exists(filePath))
            {
                // Cria o arquivo e adiciona a nova linha
                using (StreamWriter sw = File.CreateText(filePath))
                {
                    sw.WriteLine($"{thumbprint}, {ip}, {username}");
                    Console.WriteLine("Arquivo criado e nova linha adicionada.");
                }
                return;
            }

            // Lê todas as linhas do arquivo
            var lines = File.ReadAllLines(filePath).ToList();

            // Verifica se já existe uma linha com o mesmo thumbprint
            bool thumbprintExists = lines.Any(line => line.Split(',')[0].Trim() == thumbprint);

            if (thumbprintExists)
            {
                Console.WriteLine("O thumbprint já existe no arquivo.");
            }
            else
            {
                // Adiciona a nova linha ao arquivo
                using (StreamWriter sw = File.AppendText(filePath))
                {
                    sw.WriteLine($"{Environment.NewLine}{thumbprint}, {ip}, {username}");
                    Console.WriteLine("Nova linha adicionada ao arquivo.");
                }
            }
        }

        
    }
}
