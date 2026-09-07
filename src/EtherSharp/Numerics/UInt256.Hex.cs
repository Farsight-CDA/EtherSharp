namespace EtherSharp.Numerics;

public readonly partial struct UInt256
{
    /// <summary>
    /// Parses one to 64 hexadecimal digits with an optional <c>0x</c> or <c>0X</c> prefix.
    /// </summary>
    /// <param name="value">
    /// Case-insensitive hexadecimal digits. Odd digit counts and
    /// leading zeros are accepted; whitespace, signs, and empty input are rejected.
    /// </param>
    /// <param name="result">The parsed value, or zero if parsing fails.</param>
    /// <returns>Whether the input is a valid hexadecimal 256-bit value.</returns>
    public static bool TryParseFromHex(ReadOnlySpan<char> value, out UInt256 result)
        => HexParser.TryParse(value, out result);
}
