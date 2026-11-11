using System.Text.Json.Serialization;

namespace EtherSharp.RPC.Transport.Json;

[JsonConverter(typeof(JsonRpcResponseConverter<>))]
internal readonly struct JsonRpcResponse<TResult>(int? id, TResult? result, bool resultIsNull, RpcError? error)
{
    public int? Id { get; } = id;
    public TResult? Result { get; } = result;
    public bool ResultIsNull { get; } = resultIsNull;
    public RpcError? Error { get; } = error;
}
