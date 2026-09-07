namespace EtherSharp.Numerics;

public readonly partial struct Int256 : ISpanFormattable, IUtf8SpanFormattable
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

    /// <summary>
    /// Formats the value as a decimal integer using the current culture.
    /// </summary>
    public override string ToString()
        => ToString(null, null);

    /// <summary>
    /// Formats the value using the specified numeric format and the current culture.
    /// </summary>
    public string ToString(string? format)
        => ToString(format, null);

    /// <summary>
    /// Formats the value as a decimal integer using the specified culture.
    /// </summary>
    public string ToString(IFormatProvider? provider)
        => ToString(null, provider);

    /// <summary>
    /// Formats the value using the specified numeric format and culture.
    /// </summary>
    /// <remarks>
    /// Supports standard and custom BigInteger numeric formats. Hexadecimal (X/x) and binary (B/b)
    /// formats represent negative values using all 256 two's-complement bits, without a prefix.
    /// Positive values have no extra sign digit. Precision specifies a minimum digit count, padded with zeros.
    /// A null or empty format uses decimal notation; a null provider uses the current culture.
    /// </remarks>
    /// <exception cref="FormatException">The numeric format is invalid.</exception>
    public string ToString(string? format, IFormatProvider? provider)
        => IntegerFormatter.Format(in _value, true, format, provider);

    /// <summary>
    /// Formats the value into a character span using the same formats as <see cref="ToString(String, IFormatProvider)"/>.
    /// </summary>
    /// <returns>False with zero characters written if the destination is too small; otherwise, true.</returns>
    /// <exception cref="FormatException">The numeric format is invalid.</exception>
    public bool TryFormat(
        Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null
    ) => IntegerFormatter.TryFormat(in _value, true, destination, out charsWritten, format, provider);

    /// <summary>
    /// Formats the value into a UTF-8 byte span using the same formats as <see cref="ToString(String, IFormatProvider)"/>.
    /// </summary>
    /// <returns>False with zero bytes written if the destination is too small; otherwise, true.</returns>
    /// <exception cref="FormatException">The numeric format is invalid.</exception>
    public bool TryFormat(
        Span<byte> utf8Destination, out int bytesWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null
    ) => IntegerFormatter.TryFormat(in _value, true, utf8Destination, out bytesWritten, format, provider);
}
