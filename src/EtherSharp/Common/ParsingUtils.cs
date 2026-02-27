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
    /// Default serializer options used for RPC request/response and subscription payloads.
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
            new Bytes32Converter(),
            new ByteArrayHexConverter(),
            new IntHexConverter(),
            new UIntHexConverter(),
            new LongHexConverter(),
            new ULongHexConverter(),
            new TargetHeightConverter(),
            new UInt256HexConverter(),
            new Int256HexConverter()
        }
    };

    /// <summary>
    /// Creates a copy of <see cref="EvmSerializerOptions"/> for per-client customization.
    /// </summary>
    /// <returns>A new serializer options instance initialized from the default EVM options.</returns>
    public static JsonSerializerOptions CreateDefaultEvmSerializerOptions()
        => new JsonSerializerOptions(EvmSerializerOptions);
}
