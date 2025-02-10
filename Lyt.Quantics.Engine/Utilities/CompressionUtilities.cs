using System.IO.Compression;

namespace Lyt.Quantics.Engine.Utilities;

public static class CompressionUtilities
{
    /*
    byte[] dataToCompress = Encoding.UTF8.GetBytes(originalString);
    byte[] compressedData = GZipCompressor.Compress(dataToCompress);
    string compressedString = Encoding.UTF8.GetString(compressedData);
    Console.WriteLine("Length of compressed string: " + compressedString.Length);
byte[] decompressedData = GZipCompressor.Decompress(compressedData);
    string deCompressedString = Encoding.UTF8.GetString(decompressedData);
    Console.WriteLine("Length of decompressed string: " + deCompressedString.Length);
*/
    public static byte[] Compress(string stringData)
    {
        byte[] byteData = Encoding.UTF8.GetBytes(stringData);
        return CompressionUtilities.Compress(byteData);
    }

    public static string DecompressToString (byte[] byteData)
    {
        byte[] decompressedByteData = CompressionUtilities.Decompress(byteData);
        return Encoding.UTF8.GetString(decompressedByteData);
    }

    public static byte[] Compress(byte[] data)
    {
        byte[] compressArray ;
        try
        {
            using MemoryStream memoryStream = new();
            using (DeflateStream deflateStream = new(memoryStream, CompressionMode.Compress))
            {
                deflateStream.Write(data, 0, data.Length);
            }

            compressArray = memoryStream.ToArray();
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
            throw; 
        }

        return compressArray;
    }

    public static byte[] Decompress(byte[] data)
    {
        byte[] decompressedArray;
        try
        {
            using MemoryStream decompressedStream = new();
            using (MemoryStream compressStream = new(data))
            {
                using DeflateStream deflateStream = new(compressStream, CompressionMode.Decompress);
                deflateStream.CopyTo(decompressedStream);
            }

            decompressedArray = decompressedStream.ToArray();
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
            throw;
        }

        return decompressedArray;
    }
}