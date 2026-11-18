using System.Globalization;
using System.Numerics;
using System.Text.Json;

namespace EtherSharp.Common.Converters.Json;

internal static class JsonHexNumberReader
{
    internal static T Read<T>(scoped ref Utf8JsonReader reader, int maxDigits) where T : INumberBase<T>
    {
        if(!reader.HasValueSequence && !reader.ValueIsEscaped)
        {
            var utf8Digits = HexUtils.StripHexPrefix(reader.ValueSpan);
            if(utf8Digits.Length <= maxDigits &&
                T.TryParse(utf8Digits, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }
        }

        return ReadTranscoded<T>(ref reader, maxDigits);
    }

    private static T ReadTranscoded<T>(scoped ref Utf8JsonReader reader, int maxDigits) where T : INumberBase<T>
    {
        long valueLength = reader.HasValueSequence
            ? reader.ValueSequence.Length
            : reader.ValueSpan.Length;

        if(valueLength > maxDigits + 4)
        {
            throw new InvalidOperationException("Unexpected number length");
        }

        Span<char> sourceBuffer = stackalloc char[(int) valueLength];
        int charsWritten = reader.CopyString(sourceBuffer);
        var digits = HexUtils.StripHexPrefix(sourceBuffer[..charsWritten]);

        return digits.Length > maxDigits
            ? throw new InvalidOperationException("Unexpected number length")
            : T.Parse(digits, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
    }
}
