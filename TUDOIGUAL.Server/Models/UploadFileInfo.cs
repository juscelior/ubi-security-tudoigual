namespace TUDOIGUAL.Server.Models
{
    public class UploadFileInfo
    {
        public int FileNameLen { get; set; }

        public string FileName { get; set; }

        public string FileContent { get; set; }

        public byte[] FileData { get; set; }

        public byte[] fileContentBytes { get; set; }

        public string SavedFullPath { get; set; }
        public byte[] EncryptedSimetricKey { get; set; }
    }
}
