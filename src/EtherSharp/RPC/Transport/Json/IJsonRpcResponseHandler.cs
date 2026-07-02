using System.Text.Json;

namespace EtherSharp.RPC.Transport.Json;

internal interface IJsonRpcResponseHandler
{
    public void SetResponse(ReadOnlySpan<byte> json, JsonSerializerOptions options);
    public void SetException(Exception exception);
}
