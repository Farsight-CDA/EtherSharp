using System.Text.Json;

namespace EtherSharp.RPC.Transport.Json;

internal sealed class JsonRpcResponseHandler<TResult> : TaskCompletionSource<JsonRpcResponse<TResult>>, IJsonRpcResponseHandler
{
    public JsonRpcResponseHandler()
        : base(TaskCreationOptions.RunContinuationsAsynchronously)
    {
    }

    public void SetResponse(ReadOnlySpan<byte> json, JsonSerializerOptions options)
    {
        var response = JsonSerializer.Deserialize<JsonRpcResponse<TResult>>(json, options);
        SetResult(response);
    }
}
