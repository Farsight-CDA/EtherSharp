using EtherSharp.Client.Services.RPC;
using EtherSharp.Common;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Transport;
public class WssJsonRpcTransport(Uri uri, TimeSpan requestTimeout, ILogger? logger = null) : IRPCTransport, IDisposable
{
    public bool SupportsFilters => true;
    public bool SupportsSubscriptions => true;

    private readonly ILogger? _logger = logger;

    private readonly Lock _statusLock = new Lock();
    private bool _isInitialized;
    private bool _isDisposed;

    private readonly Uri _uri = uri;
    private readonly TimeSpan _requestTimeout = requestTimeout;
    private ClientWebSocket? _socket = null!;

    private readonly CancellationTokenSource _connectionHandlerCts = new CancellationTokenSource();

    private int _requestIdCounter = 0;
    private readonly ConcurrentDictionary<int, (Type RpcResponseType, TaskCompletionSource<object> Tcs)> _pendingRequests = [];

    public event Action? OnConnectionEstablished;
    public event Action<string, ReadOnlySpan<byte>>? OnSubscriptionMessage;

    public async ValueTask InitializeAsync(CancellationToken cancellationToken)
    {
        lock(_statusLock)
        {
            if(_isInitialized)
            {
                return;
            }

            _isInitialized = true;
        }

        await ConnectSocketAsync(cancellationToken);
        _ = ConnectionHandler();
    }

    private async Task ConnectionHandler()
    {
        while(!_connectionHandlerCts.IsCancellationRequested)
        {
            _requestIdCounter = 0;
            _ = Task.Run(() => OnConnectionEstablished?.Invoke());

            try
            {
                await MessageHandler();
                _logger?.LogWarning("Websocket connection lost, killing {requestCount} pending requests...", _pendingRequests.Count);

                var wssClosedException = new Exception("WSS closed");
                foreach(var r in _pendingRequests)
                {
                    r.Value.Tcs.SetException(wssClosedException);
                }
                _pendingRequests.Clear();
            }
            catch(Exception ex)
            {
                _logger?.LogWarning(ex, "Websocket connection lost, killing {requestCount} pending requests...", _pendingRequests.Count);
                foreach(var r in _pendingRequests)
                {
                    r.Value.Tcs.SetException(ex);
                }
                _pendingRequests.Clear();
            }

            try
            {
                _socket?.Dispose();
                await ConnectSocketAsync(_connectionHandlerCts.Token);
            }
            catch(Exception ex)
            {
                _logger?.LogCritical(ex, "ConnectionHandler crashed");
            }
        }
    }

    //Returns on successful connection. Will retry infinitly
    private async Task ConnectSocketAsync(CancellationToken cancellationToken)
    {
        _logger?.LogDebug("Initiating websocket connection...");

        while(_socket is null || _socket.State != WebSocketState.Open)
        {
            if (_socket is not null)
            {
                _socket.Abort();
                _socket.Dispose();
            }

            try
            {
                _socket = new ClientWebSocket();
                await _socket.ConnectAsync(_uri, cancellationToken);
            }
            catch(Exception ex)
            {
                _logger?.LogDebug(ex, "Connection attempt failed, retrying in 3s...");
                await Task.Delay(3000, cancellationToken);
            }
        }

        _logger?.LogInformation("Websocket connection established...");
    }

    private record RpcError(int Code, string Message);
    private record JsonRpcResponse<T>(
        [property: JsonRequired] int Id, T? Result, RpcError? Error, [property: JsonRequired] string Jsonrpc);
    private async Task MessageHandler()
    {
        using var ms = new MemoryStream();
        byte[] buffer = new byte[8192];

        while(_socket is not null && _socket.State == WebSocketState.Open)
        {
            ms.Seek(0, SeekOrigin.Begin);
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

            if(!TryIdentifyPayload(msBuffer, out var payloadType, out int requestId, out string subscriptionId))
            {
                continue;
            }

            switch(payloadType)
            {
                case PayloadType.Response:
                    if(!_pendingRequests.TryRemove(requestId, out var value))
                    {
                        _logger?.LogWarning("Received response to request id that is not pending");
                        break;
                    }

                    var (responseType, tcs) = value;
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
                    _logger?.LogWarning("Received unidentified websocket payload");
                    break;
            }
        }
    }

    private static bool TryIdentifyPayload(ReadOnlySpan<byte> jsonSpan,
        out PayloadType payloadType, out int requestId, out string subscriptionId)
    {
        try
        {
            var reader = new Utf8JsonReader(jsonSpan);

            reader.Read();
            reader.Read();

            while(reader.TokenType != JsonTokenType.None)
            {
                switch(reader.TokenType)
                {
                    case JsonTokenType.PropertyName:
                        if(reader.ValueTextEquals("id"))
                        {
                            reader.Read();
                            requestId = int.Parse(reader.GetString()!.AsSpan()[2..], System.Globalization.NumberStyles.HexNumber);
                            subscriptionId = null!;
                            payloadType = PayloadType.Response;
                            return true;
                        }
                        else if(reader.ValueTextEquals("method"))
                        {
                            reader.Read();
                            string? method = reader.GetString();

                            if(method != "eth_subscription")
                            {
                                requestId = -1;
                                subscriptionId = null!;
                                payloadType = PayloadType.Unknown;
                                return true;
                            }

                            reader.Read();
                            reader.Read();
                            reader.Read();

                            if(reader.TokenType == JsonTokenType.PropertyName && reader.ValueTextEquals("subscriptionId"))
                            {
                                requestId = -1;
                                subscriptionId = null!;
                                payloadType = PayloadType.Unknown;
                                return true;
                            }

                            reader.Read();
                            requestId = -1;
                            subscriptionId = reader.GetString()!;
                            payloadType = PayloadType.Subscription;
                            return true;
                        }

                        reader.Skip();
                        break;

                    case JsonTokenType.StartObject or JsonTokenType.StartArray:
                        reader.Skip();
                        break;
                    default:
                        reader.Read();
                        break;
                }
            }

            payloadType = PayloadType.Unknown;
            requestId = -1;
            subscriptionId = null!;
            return true;
        }
        catch(Exception)
        {
            payloadType = PayloadType.Unknown;
            requestId = -1;
            subscriptionId = null!;
            return false;
        }
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
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        int requestId = Interlocked.Increment(ref _requestIdCounter);

        byte[] payload = JsonSerializer.SerializeToUtf8Bytes(
            new JsonRpcRequest(requestId, method, args),
            options: ParsingUtils.EvmSerializerOptions
        );

        var tcs = new TaskCompletionSource<object>();
        var timeoutTask = Task.Delay(_requestTimeout, cancellationToken);

        _pendingRequests.TryAdd(requestId, (typeof(JsonRpcResponse<TResult>), tcs));

        await _socket!.SendAsync(payload, WebSocketMessageType.Text, true, cancellationToken);

        var resultTask = await Task.WhenAny(tcs.Task, timeoutTask);
        if(resultTask == timeoutTask)
        {
            _pendingRequests.TryRemove(requestId, out _);
            throw new TimeoutException($"No response received from server within {_requestTimeout} timeout");
        }

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

    public void Dispose()
    {
        lock(_statusLock)
        {
            if(_isDisposed)
            {
                return;
            }

            _connectionHandlerCts.Cancel();
            _connectionHandlerCts.Dispose();
            _socket?.Abort();
            _socket?.Dispose();
            _isDisposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
