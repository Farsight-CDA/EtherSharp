namespace EtherSharp.Numerics;

public readonly partial struct Int256 : ISpanFormattable, IUtf8SpanFormattable
{
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
