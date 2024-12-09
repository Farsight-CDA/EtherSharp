using EtherSharp.Common;
using EtherSharp.Types;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Transport;
public class WssJsonRpcTransport(Uri uri) : IRPCTransport
{
    private int _id = 0;
    private readonly Uri _uri = uri;
    private readonly ClientWebSocket _socket = new ClientWebSocket();

    private readonly List<(int RequestId, Type RpcResponseType, TaskCompletionSource<object> Tcs)> _pendingRequests = [];

    public bool SupportsFilters => true;
    public bool SupportsSubscriptions => true;

    public Task<RpcResult<TResult>> SendRpcRequest<TResult>(string method)
        => InnerSendAsync<TResult>(method, [], default);
    public Task<RpcResult<TResult>> SendRpcRequest<T1, TResult>(string method, T1 t1)
        => InnerSendAsync<TResult>(method, [t1], default);
    public Task<RpcResult<TResult>> SendRpcRequest<T1, T2, TResult>(string method, T1 t1, T2 t2)
        => InnerSendAsync<TResult>(method, [t1, t2], default); 
    public Task<RpcResult<TResult>> SendRpcRequest<T1, T2, T3, TResult>(string method, T1 t1, T2 t2, T3 t3)
        => InnerSendAsync<TResult>(method, [t1, t2, t3], default);

    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        await _socket.ConnectAsync(_uri, cancellationToken);
        _ = MessageHandler();
    }

    private record RpcError(int Code, string Message);
    private record JsonRpcResponse<T>(
        [property: JsonRequired] int Id, T? Result, RpcError? Error, [property: JsonRequired] string Jsonrpc);
    private async Task MessageHandler()
    {
        using var ms = new MemoryStream();
        byte[] buffer = new byte[8192];

        while(_socket.State == WebSocketState.Open)
        {
            WebSocketReceiveResult receiveResult;

            do
            {
                receiveResult = await _socket.ReceiveAsync(buffer, default);

                if(receiveResult.Count != 0)
                {
                    ms.Write(buffer.AsSpan()[0..receiveResult.Count]);
                }
            }
            while(!receiveResult.EndOfMessage);

            var msBuffer = ms.GetBuffer().AsSpan()[0..(int) ms.Length];
            int requestId = ExtractRequestIdFromJson(msBuffer);

            var (_, responseType, tcs) = _pendingRequests.First(x => x.RequestId == requestId);
            object? response = JsonSerializer.Deserialize(msBuffer, responseType)!;
            tcs.SetResult(response);

            ms.Seek(0, SeekOrigin.Begin);
            ms.Position = 0;
        }
    }
    private static int ExtractRequestIdFromJson(ReadOnlySpan<byte> jsonSpan)
    {
        var reader = new Utf8JsonReader(jsonSpan);
        while(reader.Read())
        {
            if(reader.TokenType == JsonTokenType.PropertyName && reader.ValueTextEquals(nameof(JsonRpcResponse<object>.Id)))
            {
                reader.Read();
                return reader.GetInt32();
            }
        }
        throw new InvalidOperationException("Id property not found");
    }

    private record JsonRpcRequest(int Id, string Method, object?[] Params, string Jsonrpc = "2.0");
    private async Task<RpcResult<TResult>> InnerSendAsync<TResult>(
        string method, object?[] args, CancellationToken cancellationToken = default)
    {
        int requestId = Interlocked.Increment(ref _id);

        byte[] payload = JsonSerializer.SerializeToUtf8Bytes(
            new JsonRpcRequest(requestId, method, args),
            options: ParsingUtils.EvmSerializerOptions
        );

        var tcs = new TaskCompletionSource<object>();
        _pendingRequests.Add((requestId, typeof(JsonRpcResponse<TResult>), tcs));

        await _socket.SendAsync(payload, WebSocketMessageType.Text, true, cancellationToken);

        var jsonRpcResponse = (JsonRpcResponse<TResult>) await tcs.Task;

        if(jsonRpcResponse is null)
        {
            throw new Exception("RPC Error: Invalid response");
        }
        else if(jsonRpcResponse.Id != requestId)
        {
            throw new Exception("RPC Error: Invalid response Id");
        }
        else if(jsonRpcResponse.Error != null)
        {
            return new RpcResult<TResult>.Error(jsonRpcResponse.Error.Code, jsonRpcResponse.Error.Message);
        }
        else if(jsonRpcResponse.Result is null)
        {
            return new RpcResult<TResult>.Null();
        }
        //
        return new RpcResult<TResult>.Success(jsonRpcResponse.Result);
    }
}
