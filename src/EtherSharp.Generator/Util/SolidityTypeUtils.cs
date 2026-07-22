namespace EtherSharp.Generator.Util;

public static class SolidityTypeUtils
{
    public static bool TryGetFixedBytesLength(
        string type,
        string prefix,
        out int byteLength)
    {
        byteLength = 0;
        return type.StartsWith(prefix, StringComparison.Ordinal)
            && Int32.TryParse(type.Substring(prefix.Length), out byteLength)
            && byteLength is >= 1 and <= 32;
    }
}
