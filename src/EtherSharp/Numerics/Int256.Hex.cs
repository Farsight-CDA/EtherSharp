namespace EtherSharp.Numerics;

public readonly partial struct Int256
{
    /// <summary>
    /// Parses one to 64 hexadecimal digits as a 256-bit two's-complement bit pattern,
    /// with an optional <c>0x</c> or <c>0X</c> prefix.
    /// </summary>
    /// <param name="value">
    /// Case-insensitive hexadecimal digits. Odd digit counts and
    /// leading zeros are accepted; whitespace, signs, and empty input are rejected.
    /// Inputs shorter than 64 digits are zero-extended, not sign-extended.
    /// </param>
    /// <param name="result">The parsed value, or zero if parsing fails.</param>
    /// <returns>Whether the input is a valid hexadecimal 256-bit value.</returns>
    public static bool TryParseFromHex(ReadOnlySpan<char> value, out Int256 result)
    {
        bool success = HexParser.TryParse(value, out var parsed);
        result = new Int256(parsed);
        return success;
    }
}
