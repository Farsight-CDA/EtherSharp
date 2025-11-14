using EtherSharp.Common.Converter;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Common;

public static class ParsingUtils
{
    public static readonly JsonSerializerOptions EvmSerializerOptions = new JsonSerializerOptions()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = {
            new DateTimeOffsetHexConverter(),
            new ByteArrayHexConverter(),
            new IntHexConverter(),
            new UIntHexConverter(),
            new LongHexConverter(),
            new ULongHexConverter(),
            new UnsignedBigIntHexConverter(),
            new TargetBlockNumberConverter()
        }
    };
}
