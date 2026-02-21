using EtherSharp.Common.Converter;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Common;

/// <summary>
/// Central JSON serializer configuration for Ethereum JSON-RPC payloads.
/// </summary>
public static class ParsingUtils
{
    /// <summary>
    /// Serializer options used for RPC request/response and subscription payloads.
    /// </summary>
    /// <remarks>
    /// Includes EtherSharp converters for hex-encoded numeric, byte, and block-selector fields used by EVM nodes,
    /// writes camelCase request payloads, and keeps response parsing case-insensitive across providers.
    /// </remarks>
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
            new TargetBlockNumberConverter(),
            new UInt256HexConverter(),
            new Int256HexConverter()
        }
    };
}
