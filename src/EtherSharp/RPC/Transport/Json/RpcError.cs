using EtherSharp.Common.Json.Converters;
using System.Text.Json.Serialization;

namespace EtherSharp.RPC.Transport.Json;

internal sealed record RpcError(
    int Code,
    string Message,
    [property: JsonConverter(typeof(RpcErrorDataConverter))] string? Data
);
