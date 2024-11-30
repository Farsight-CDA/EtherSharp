namespace EtherSharp.Generator.Util;
public class HexUtils
{
    public static string ToHexString(byte[] bytes)
    {
        if(bytes == null || bytes.Length == 0)
        {
            return string.Empty;
        }

        char[] hexChars = new char[bytes.Length * 2];
        const string hexAlphabet = "0123456789ABCDEF";

        for(int i = 0; i < bytes.Length; i++)
        {
            byte b = bytes[i];
            hexChars[i * 2] = hexAlphabet[b >> 4];
            hexChars[(i * 2) + 1] = hexAlphabet[b & 0x0F];
        }

        return new string(hexChars);
    }
}
