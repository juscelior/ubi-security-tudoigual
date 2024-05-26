namespace TUDOIGUAL.Server.Utils
{
    public class ByteHelper
    {
        public static int? GetEndIndexOfSequence(byte[] array, byte[] sequence)
        {
            if (array == null || sequence == null || array.Length == 0 || sequence.Length == 0)
            {
                return null;
            }

            for (int i = 0; i <= array.Length - sequence.Length; i++)
            {
                bool match = true;
                for (int j = 0; j < sequence.Length; j++)
                {
                    if (array[i + j] != sequence[j])
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                {
                    return i + sequence.Length - 1;
                }
            }

            return null;
        }

        public static byte[] SplitByteArray(byte[] array, int startIndex, int endIndex)
        {
            if (startIndex < 0 || endIndex >= array.Length || startIndex > endIndex)
            {
                throw new ArgumentOutOfRangeException("Os índices fornecidos estão fora dos limites do array.");
            }

            int length = endIndex - startIndex + 1;
            byte[] result = new byte[length];
            Array.Copy(array, startIndex, result, 0, length);
            return result;
        }
    }

}
