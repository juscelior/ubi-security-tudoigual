using System.Security.Cryptography;
using System.Text;
using TUDOIGUAL.Lib.Utils;
using TUDOIGUAL.Server.Models;

namespace TUDOIGUAL.Server.Utils.Handler
{
    public class FileRequestHandler : IRequestHandler
    {
        private const int FileNameReservedBytesLen = 4;

        public FileRequestHandler()
        {
            // Cria se não existe
            System.IO.Directory.CreateDirectory(GlobalConfiguration.UploadFolder);
        }

        public void Dispose()
        {
        }

        public async Task<byte[]> HandleAsync(IStateObject state)
        {

            // Recupera o contexto do upload
            var uploadFileInfo = await this.GetUploadFileInfoAsync(state.Content);

            byte[] output = new byte[] { };

            if (uploadFileInfo.FileName.EndsWith(".encKey"))
            {
                output = File.ReadAllBytes(uploadFileInfo.SavedFullPath);
            }
            else if (uploadFileInfo.FileName.Contains("##COPY##"))
            {
                var fName = uploadFileInfo.FileName.Replace("##COPY##", "");

                output = File.ReadAllBytes(Path.Combine(GlobalConfiguration.UploadFolder, fName));
            }
            else if (uploadFileInfo.FileName.EndsWith("ALL.ALL"))
            {
                var encFiles = Directory.GetFiles(GlobalConfiguration.UploadFolder, "*.enc")?.Select(f=>Path.GetFileName(f));

                string listAllFiles = string.Join(";", encFiles);

                output = Encoding.UTF8.GetBytes(listAllFiles + GlobalConfiguration.Eof);
            }
            else
            {
                // Salva o arquivo
                await this.SaveFileAsync(uploadFileInfo);

                output = File.ReadAllBytes(uploadFileInfo.SavedFullPath + ".sig");

            }


            return output;
        }

        private async Task<UploadFileInfo> GetUploadFileInfoAsync(byte[] data)
        {
            var ufInfo = new UploadFileInfo();

            var dataBytes = data;
            var dataBytesLen = dataBytes.Length;

            // File name's length
            var tmp = Encoding.UTF8.GetString(dataBytes, 0, FileNameReservedBytesLen);
            ufInfo.FileNameLen = Convert.ToInt16(Encoding.UTF8.GetString(dataBytes, 0, 4));

            // File name
            ufInfo.FileName = Encoding.UTF8.GetString(dataBytes, FileNameReservedBytesLen, ufInfo.FileNameLen);

            // File content
            var eofBytes = Encoding.UTF8.GetBytes(GlobalConfiguration.Eof);
            var keyBytes = Encoding.UTF8.GetBytes(GlobalConfiguration.Key);


            int? eofIndex = ByteHelper.GetEndIndexOfSequence(dataBytes, eofBytes);
            int? keyIndex = ByteHelper.GetEndIndexOfSequence(dataBytes, keyBytes);

            if (!ufInfo.FileName.EndsWith(".encKey") && !ufInfo.FileName.EndsWith("ALL.ALL") && !ufInfo.FileName.Contains("##COPY##"))
            {

                ufInfo.FileData = ByteHelper.SplitByteArray(dataBytes, FileNameReservedBytesLen + ufInfo.FileNameLen, keyIndex.Value - keyBytes.Length);

                ufInfo.EncryptedSimetricKey = ByteHelper.SplitByteArray(dataBytes, FileNameReservedBytesLen + ufInfo.FileNameLen + ufInfo.FileData.Length + keyBytes.Length, eofIndex.Value - eofBytes.Length);
            }

            ufInfo.SavedFullPath = Path.Combine(GlobalConfiguration.UploadFolder, ufInfo.FileName);

            return await Task.FromResult(ufInfo);
        }

        private async Task SaveFileAsync(UploadFileInfo ufInfo)
        {
            #region Create an empty file if not exist

            _ = await this.CreateFileAsync(ufInfo.SavedFullPath);
            #endregion

            #region Write content to file

            using BinaryWriter bWrite = new BinaryWriter(File.Open(ufInfo.SavedFullPath, FileMode.Create));
            bWrite.Write(ufInfo.FileData, 0, ufInfo.FileData.Length);
            bWrite.Close();
            #endregion

            #region Write key content to file

            using BinaryWriter bkWrite = new BinaryWriter(File.Open(ufInfo.SavedFullPath + ".encKey", FileMode.Create));
            bkWrite.Write(ufInfo.EncryptedSimetricKey, 0, ufInfo.EncryptedSimetricKey.Length);
            bkWrite.Close();
            #endregion


            RSA rsa = RsaKeyHelper.LoadPrivateKey(GlobalConfiguration.Username);
            
            RsaKeyHelper.SignDocument(rsa, ufInfo.SavedFullPath, ufInfo.SavedFullPath + ".sig");
            

            await Task.CompletedTask;
        }

        private async Task<bool> CreateFileAsync(string fileFullPath)
        {
            bool isAreadyExist = false;

            if (File.Exists(fileFullPath))
                isAreadyExist = true;
            else
                File.Create(fileFullPath).Close();

            return await Task.FromResult(!isAreadyExist);
        }
    }

}
