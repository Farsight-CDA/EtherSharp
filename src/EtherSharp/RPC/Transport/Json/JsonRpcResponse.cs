using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.RPC.Transport.Json;

[JsonConverter(typeof(JsonRpcResponseConverterFactory))]
internal readonly struct JsonRpcResponse<TResult>(int? id, TResult? result, bool resultIsNull, RpcError? error, string? jsonrpc)
{
    public int? Id { get; } = id;
    public TResult? Result { get; } = result;
    public bool ResultIsNull { get; } = resultIsNull;
    public RpcError? Error { get; } = error;
    public string? Jsonrpc { get; } = jsonrpc;
}
