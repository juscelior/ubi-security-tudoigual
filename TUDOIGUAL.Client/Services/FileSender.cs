using TUDOIGUAL.Client.Utils;

namespace TUDOIGUAL.Client.Services
{
    public class FileSender : SslSocketClient
    {
        public async Task SendFileAsync(string filePath, byte[] encryptedSimetricKey)
        {
            var byteData = UploadFileConverter.ToFormattedBytes(filePath, encryptedSimetricKey);

            await base.SendAsync(filePath + ".sig", byteData);
        }

        public async Task GetKeyAsync(string filePath)
        {
            string keyFilePath = filePath + ".encKey";

            var byteData = UploadFileConverter.ToKeyFormattedBytes(keyFilePath);

            await base.SendAsync(keyFilePath, byteData);
        }

        public async Task GetSyncAsync()
        {
            string filePath = "ALL.ALL";

            var byteData = UploadFileConverter.ToListFormattedBytes(filePath);

            string files = await base.SendAsync(filePath, byteData);

            var listFiles = files.Replace("<EOF>","").Split(";");

            foreach ( var file in listFiles) {
                string localFile = Path.Combine(GlobalConfiguration.SyncFolder, file);
                if (!File.Exists(localFile))
                {
                    var fName = "##COPY##" + file;
                    var fData = UploadFileConverter.ToGetCopyFormattedBytes(fName);
                    await base.SendAsync(localFile, fData);
                }

                
            }
        }
    }

}
