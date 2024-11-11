using EtherSharp.Converter;
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
            new BigIntHexConverter(),
            new ByteArrayHexConverter(),
            new IntHexConverter(),
            new LongHexConverter(),
            new UIntHexConverter(),
            new ULongHexConverter(),
            new DateTimeOffsetHexConverter()
        }
    };
}
