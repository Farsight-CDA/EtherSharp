using EtherSharp.Common.Converters.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.RPC.Transport.Json;

internal sealed record RpcError(
    int Code,
    string Message,
    [property: JsonConverter(typeof(RpcErrorDataConverter))] string? Data
);
