using EtherSharp.Client.Services.RPC;
using EtherSharp.Common;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Transport;
public class WssJsonRpcTransport(Uri uri) : IRPCTransport
{
    private int _id = 0;
    private Lock _initLock = new Lock();
    private bool _isInitialized;
    private bool _isDead;

    private readonly Uri _uri = uri;
    private readonly ClientWebSocket _socket = new ClientWebSocket();

    private readonly List<(int RequestId, Type RpcResponseType, TaskCompletionSource<object> Tcs)> _pendingRequests = [];

    public event Action<string, ReadOnlySpan<byte>>? OnSubscriptionMessage;

    public bool SupportsFilters => true;
    public bool SupportsSubscriptions => true;

    public Task<RpcResult<TResult>> SendRpcRequest<TResult>(
        string method, CancellationToken cancellationToken)
        => InnerSendAsync<TResult>(method, [], cancellationToken);
    public Task<RpcResult<TResult>> SendRpcRequest<T1, TResult>(
        string method, T1 t1, CancellationToken cancellationToken)
        => InnerSendAsync<TResult>(method, [t1], cancellationToken);
    public Task<RpcResult<TResult>> SendRpcRequest<T1, T2, TResult>(
        string method, T1 t1, T2 t2, CancellationToken cancellationToken)
        => InnerSendAsync<TResult>(method, [t1, t2], cancellationToken);
    public Task<RpcResult<TResult>> SendRpcRequest<T1, T2, T3, TResult>(
        string method, T1 t1, T2 t2, T3 t3, CancellationToken cancellationToken)
        => InnerSendAsync<TResult>(method, [t1, t2, t3], cancellationToken);

    public async ValueTask InitializeAsync(CancellationToken cancellationToken)
    {
        if (_isInitialized)
        {
            return;
        }

        lock (_initLock)
        {
            if(_isInitialized)
            {
                return;
            }

            _isInitialized = true;
        }

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

        try
        {
            while(true)
            {
                while(_socket.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult receiveResult;
                    int totalLength = 0;
                    do
                    {
                        receiveResult = await _socket.ReceiveAsync(buffer, default);

                        if(receiveResult.Count != 0)
                        {
                            ms.Write(buffer.AsSpan()[0..receiveResult.Count]);
                        }

                        totalLength += receiveResult.Count;
                    }
                    while(!receiveResult.EndOfMessage);

                    var msBuffer = ms.GetBuffer().AsSpan()[0..(int) ms.Position];

                    switch(IdentifyPayload(msBuffer, out int requestId, out string subscriptionId))
                    {
                        case PayloadType.Response:
                            var (_, responseType, tcs) = _pendingRequests.First(x => x.RequestId == requestId);
                            object? response = JsonSerializer.Deserialize(
                                msBuffer, responseType,
                                options: ParsingUtils.EvmSerializerOptions
                            )!;
                            tcs.SetResult(response);
                            break;
                        case PayloadType.Subscription:
                            OnSubscriptionMessage?.Invoke(subscriptionId, msBuffer);
                            break;
                        default:
                            break;
                    }

                    ms.Seek(0, SeekOrigin.Begin);
                    ms.Position = 0;
                }

                await _socket.ConnectAsync(_uri, default);
            }
        }
        catch(Exception ex)
        {
            _isDead = true;
            foreach(var (_, _, tcs) in _pendingRequests)
            {
                tcs.SetException(ex);
            }
        }
    }
    private static PayloadType IdentifyPayload(ReadOnlySpan<byte> jsonSpan,
        out int requestId, out string subscriptionId)
    {
        var reader = new Utf8JsonReader(jsonSpan);

        reader.Read();
        reader.Read();
        reader.Read();
        reader.Read();

        if(reader.TokenType == JsonTokenType.PropertyName && reader.ValueTextEquals("id"))
        {
            reader.Read();
            requestId = int.Parse(reader.GetString()!.AsSpan()[2..], System.Globalization.NumberStyles.HexNumber);
            subscriptionId = null!;
            return PayloadType.Response;
        }

        if(reader.TokenType == JsonTokenType.PropertyName && reader.ValueTextEquals("method"))
        {
            reader.Read();
            string? method = reader.GetString();

            if(method != "eth_subscription")
            {
                goto unknown_payload_type;
            }

            reader.Read();
            reader.Read();
            reader.Read();

            if(reader.TokenType == JsonTokenType.PropertyName && reader.ValueTextEquals("subscriptionId"))
            {
                goto unknown_payload_type;
            }

            reader.Read();
            requestId = -1;
            subscriptionId = reader.GetString()!;
            return PayloadType.Subscription;
        }

    unknown_payload_type:
        requestId = -1;
        subscriptionId = null!;
        return PayloadType.Unknown;
    }

    private enum PayloadType
    {
        Response,
        Subscription,
        Unknown
    }

    private record JsonRpcRequest(int Id, string Method, object?[] Params, string Jsonrpc = "2.0");
    private async Task<RpcResult<TResult>> InnerSendAsync<TResult>(
        string method, object?[] args, CancellationToken cancellationToken = default)
    {
        if(_isDead)
        {
            throw new InvalidOperationException("Transport is dead");
        }

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
