using System.Text;

namespace TUDOIGUAL.Client.Utils
{
    public class UploadFileConverter
    {
        private static int FileNameReservedBytesLen = 4; // Reserved bytes length of file name

        public static byte[] ToFormattedBytes(string filePath, byte[] encryptedSimetricKey)
        {
            byte[] clientData = null;

            // Get file name
            string fileName = Path.GetFileName(filePath);

            // Get file name as bytes
            byte[] fileNameBytes = Encoding.UTF8.GetBytes(fileName);

            // Get file name's lenght as bytes
            byte[] fileNameLenBytes = Encoding.UTF8.GetBytes(fileNameBytes.Length.ToString());

            // Get file data as bytes

            byte[] fileContentBytes = File.ReadAllBytes(filePath);

            // KEY
            byte[] keyBytes = Encoding.UTF8.GetBytes(GlobalConfiguration.Key);

            // EOF
            byte[] eofBytes = Encoding.UTF8.GetBytes(GlobalConfiguration.Eof);


            // Define clientData capacity: [File name length][File name][File content][EOF]
            clientData = new byte[FileNameReservedBytesLen + fileNameBytes.Length + fileContentBytes.Length + encryptedSimetricKey.Length + keyBytes.Length + eofBytes.Length];

            // Write clientData as
            // [File name length][File name][File content][EOF]
            fileNameLenBytes.CopyTo(clientData, 0);
            fileNameBytes.CopyTo(clientData, FileNameReservedBytesLen);
            fileContentBytes.CopyTo(clientData, FileNameReservedBytesLen + fileNameBytes.Length);

            keyBytes.CopyTo(clientData, FileNameReservedBytesLen + fileNameBytes.Length + fileContentBytes.Length);

            encryptedSimetricKey.CopyTo(clientData, FileNameReservedBytesLen + fileNameBytes.Length + fileContentBytes.Length + keyBytes.Length);

            eofBytes.CopyTo(clientData, FileNameReservedBytesLen + fileNameBytes.Length + fileContentBytes.Length+ keyBytes.Length + encryptedSimetricKey.Length);

            return clientData;
        }

        public static byte[] ToKeyFormattedBytes(string filePath)
        {
            byte[] clientData = null;
            byte[] encryptedSimetricKey = new byte[] { };
            // Get file name
            string fileName = Path.GetFileName(filePath);

            // Get file name as bytes
            byte[] fileNameBytes = Encoding.UTF8.GetBytes(fileName);

            // Get file name's lenght as bytes
            byte[] fileNameLenBytes = Encoding.UTF8.GetBytes(fileNameBytes.Length.ToString());

            // Get file data as bytes

            byte[] fileContentBytes = new byte[] { };

            // KEY
            byte[] keyBytes = Encoding.UTF8.GetBytes(GlobalConfiguration.Key);

            // EOF
            byte[] eofBytes = Encoding.UTF8.GetBytes(GlobalConfiguration.Eof);


            // Define clientData capacity: [File name length][File name][File content][EOF]
            clientData = new byte[FileNameReservedBytesLen + fileNameBytes.Length + fileContentBytes.Length + 0 + keyBytes.Length + eofBytes.Length];

            // Write clientData as
            // [File name length][File name][File content][EOF]
            fileNameLenBytes.CopyTo(clientData, 0);
            fileNameBytes.CopyTo(clientData, FileNameReservedBytesLen);
            fileContentBytes.CopyTo(clientData, FileNameReservedBytesLen + fileNameBytes.Length);

            keyBytes.CopyTo(clientData, FileNameReservedBytesLen + fileNameBytes.Length + fileContentBytes.Length);

            encryptedSimetricKey.CopyTo(clientData, FileNameReservedBytesLen + fileNameBytes.Length + fileContentBytes.Length + keyBytes.Length);

            eofBytes.CopyTo(clientData, FileNameReservedBytesLen + fileNameBytes.Length + fileContentBytes.Length + keyBytes.Length + encryptedSimetricKey.Length);

            return clientData;
        }

        public static byte[] ToListFormattedBytes(string filePath)
        {
            byte[] clientData = null;
            byte[] encryptedSimetricKey = new byte[] { };
            // Get file name
            string fileName = Path.GetFileName(filePath);

            // Get file name as bytes
            byte[] fileNameBytes = Encoding.UTF8.GetBytes(fileName);

            // Get file name's lenght as bytes
            byte[] fileNameLenBytes = Encoding.UTF8.GetBytes(fileNameBytes.Length.ToString());

            // Get file data as bytes

            byte[] fileContentBytes = new byte[] { };

            // KEY
            byte[] keyBytes = Encoding.UTF8.GetBytes(GlobalConfiguration.Key);

            // EOF
            byte[] eofBytes = Encoding.UTF8.GetBytes(GlobalConfiguration.Eof);


            // Define clientData capacity: [File name length][File name][File content][EOF]
            clientData = new byte[FileNameReservedBytesLen + fileNameBytes.Length + fileContentBytes.Length + 0 + keyBytes.Length + eofBytes.Length];

            // Write clientData as
            // [File name length][File name][File content][EOF]
            fileNameLenBytes.CopyTo(clientData, 0);
            fileNameBytes.CopyTo(clientData, FileNameReservedBytesLen);
            fileContentBytes.CopyTo(clientData, FileNameReservedBytesLen + fileNameBytes.Length);

            keyBytes.CopyTo(clientData, FileNameReservedBytesLen + fileNameBytes.Length + fileContentBytes.Length);

            encryptedSimetricKey.CopyTo(clientData, FileNameReservedBytesLen + fileNameBytes.Length + fileContentBytes.Length + keyBytes.Length);

            eofBytes.CopyTo(clientData, FileNameReservedBytesLen + fileNameBytes.Length + fileContentBytes.Length + keyBytes.Length + encryptedSimetricKey.Length);

            return clientData;
        }

        public static byte[] ToGetCopyFormattedBytes(string filePath)
        {
            byte[] clientData = null;
            byte[] encryptedSimetricKey = new byte[] { };
            // Get file name
            string fileName = Path.GetFileName(filePath);

            // Get file name as bytes
            byte[] fileNameBytes = Encoding.UTF8.GetBytes(fileName);

            // Get file name's lenght as bytes
            byte[] fileNameLenBytes = Encoding.UTF8.GetBytes(fileNameBytes.Length.ToString());

            // Get file data as bytes

            byte[] fileContentBytes = new byte[] { };

            // KEY
            byte[] keyBytes = Encoding.UTF8.GetBytes(GlobalConfiguration.Key);

            // EOF
            byte[] eofBytes = Encoding.UTF8.GetBytes(GlobalConfiguration.Eof);


            // Define clientData capacity: [File name length][File name][File content][EOF]
            clientData = new byte[FileNameReservedBytesLen + fileNameBytes.Length + fileContentBytes.Length + 0 + keyBytes.Length + eofBytes.Length];

            // Write clientData as
            // [File name length][File name][File content][EOF]
            fileNameLenBytes.CopyTo(clientData, 0);
            fileNameBytes.CopyTo(clientData, FileNameReservedBytesLen);
            fileContentBytes.CopyTo(clientData, FileNameReservedBytesLen + fileNameBytes.Length);

            keyBytes.CopyTo(clientData, FileNameReservedBytesLen + fileNameBytes.Length + fileContentBytes.Length);

            encryptedSimetricKey.CopyTo(clientData, FileNameReservedBytesLen + fileNameBytes.Length + fileContentBytes.Length + keyBytes.Length);

            eofBytes.CopyTo(clientData, FileNameReservedBytesLen + fileNameBytes.Length + fileContentBytes.Length + keyBytes.Length + encryptedSimetricKey.Length);

            return clientData;
        }
    }

}
