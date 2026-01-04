using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Common.Converter;

internal class UnsignedBigIntHexConverter : JsonConverter<BigInteger>
{
    public override BigInteger Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string hexNumber = reader.GetString() ?? throw new JsonException("Cannot parse null to signed BigInteger");

        int hexChars = hexNumber.Length - 2;

        if(hexChars > 66)
        {
            throw new InvalidOperationException("Unexpected number length");
        }

        Span<char> rawHex = stackalloc char[((hexChars - 1) / 2 * 2) + 2];
        int missingChars = rawHex.Length - hexChars;
        rawHex[..missingChars].Fill('0');
        hexNumber.AsSpan(2).CopyTo(rawHex[missingChars..]);

        if(rawHex.Length > 64)
        {
            throw new InvalidOperationException("Unexpected number length");
        }

        Span<byte> buffer = stackalloc byte[rawHex.Length / 2];

        var res = Convert.FromHexString(rawHex, buffer, out _, out _);
        return res != System.Buffers.OperationStatus.Done
            ? throw new InvalidOperationException("Failed to parse resulting hex")
            : new BigInteger(buffer, true, true);
    }

    public override void Write(Utf8JsonWriter writer, BigInteger value, JsonSerializerOptions options)
    {
        Span<char> buffer = stackalloc char[10];
        buffer[0] = '0';
        buffer[1] = 'x';

        if(value.TryFormat(buffer[2..], out int charsWritten, "X"))
        {
            writer.WriteStringValue(buffer[..(2 + charsWritten)]);
        }
        else
        {
            throw new FormatException("The value could not be formatted as hex.");
        }
    }
}
