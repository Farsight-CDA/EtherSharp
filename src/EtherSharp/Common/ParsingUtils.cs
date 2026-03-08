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
    public static readonly JsonSerializerOptions EvmSerializerOptions = CreateEvmSerializerOptions();

    /// <summary>
    /// Creates a copy of <see cref="EvmSerializerOptions"/> for per-client customization.
    /// </summary>
    /// <returns>A new serializer options instance initialized from the default EVM options.</returns>
    public static JsonSerializerOptions CreateDefaultEvmSerializerOptions()
        => new JsonSerializerOptions(EvmSerializerOptions);

    private static JsonSerializerOptions CreateEvmSerializerOptions()
    {
        var options = new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        options.Converters.Add(new DateTimeOffsetHexConverter());
        FixedBytesJsonConverters.Register(options.Converters);
        options.Converters.Add(new ByteArrayHexConverter());
        options.Converters.Add(new IntHexConverter());
        options.Converters.Add(new UIntHexConverter());
        options.Converters.Add(new LongHexConverter());
        options.Converters.Add(new ULongHexConverter());
        options.Converters.Add(new TargetHeightConverter());
        options.Converters.Add(new UInt256HexConverter());
        options.Converters.Add(new Int256HexConverter());
        return options;
    }
}
