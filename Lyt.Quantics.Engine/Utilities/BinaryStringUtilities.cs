namespace Lyt.Quantics.Engine.Utilities;

public static class BinaryStringUtilities
{
    public static string ToBinary(this int number, int bitsLength = 32) 
        =>  NumberToBinary(number, bitsLength);

    public static string NumberToBinary(int number, int bitsLength = 32)
        => Convert.ToString(number, 2).PadLeft(bitsLength, '0');

    public static int FromBinaryToInt(this string binary) => BinaryToInt(binary);

    public static int BinaryToInt(string binary) => Convert.ToInt32(binary, 2);

    public static string Reverse(this string s)
    {
        char[] charArray = s.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }
}