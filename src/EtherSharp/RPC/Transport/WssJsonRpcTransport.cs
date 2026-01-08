using EtherSharp.Common;
using EtherSharp.Common.Exceptions;
using EtherSharp.Common.Extensions;
using EtherSharp.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics.Metrics;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.RPC.Transport;
/// <summary>
/// Transport for EVM Websocket RPC
/// </summary>
public class WssJsonRpcTransport : IRPCTransport, IDisposable
{
    /// <inheritdoc />
    public bool SupportsFilters => true;
    /// <inheritdoc />
    public bool SupportsSubscriptions => true;

    /// <inheritdoc />
    public event Action? OnConnectionEstablished;
    /// <inheritdoc />
    public event Action<string, ReadOnlySpan<byte>>? OnSubscriptionMessage;

    private readonly ILogger? _logger;

    private readonly Lock _statusLock = new Lock();
    private bool _isInitialized;
    private bool _isDisposed;

    private readonly Uri _uri;
    private readonly TimeSpan _requestTimeout;
    private ClientWebSocket? _socket = null!;

    private readonly CancellationTokenSource _connectionHandlerCts = new CancellationTokenSource();

    private int _requestIdCounter;
    private readonly ConcurrentDictionary<int, (Type RpcResponseType, TaskCompletionSource<object> Tcs)> _pendingRequests = [];

    private readonly ObservableGauge<int>? _websocketConnectedGauge;

    public WssJsonRpcTransport(Uri uri, TimeSpan requestTimeout, IServiceProvider provider)
    {
        _uri = uri;
        _requestTimeout = requestTimeout;
        _logger = provider.GetService<ILoggerFactory>()?.CreateLogger<WssJsonRpcTransport>();

        _websocketConnectedGauge = provider.CreateOTELObservableGauge("wss_connection_up",
            () => (_socket is not null && _socket.State == WebSocketState.Open) ? 1 : 0
        );
    }

    /// <inheritdoc />
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
                _logger?.LogWarning("Websocket connection lost, killing {RequestCount} pending requests...", _pendingRequests.Count);

                var wssClosedException = new RPCTransportException("The WebSocket is not connected.");
                foreach(var r in _pendingRequests)
                {
                    r.Value.Tcs.SetException(wssClosedException);
                }
                _pendingRequests.Clear();
            }
            catch(Exception ex)
            {
                _logger?.LogWarning(ex, "Websocket connection lost, killing {RequestCount} pending requests...", _pendingRequests.Count);
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
            if(_socket is not null)
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

    private record RpcError(int Code, string Message, string? Data);
    private record JsonRpcResponse<T>(
        [property: JsonRequired] int Id, T? Result, RpcError? Error, [property: JsonRequired] string Jsonrpc);
    private async Task MessageHandler()
    {
        using var ms = new MemoryStream();
        byte[] buffer = new byte[32 * 1024];

        while(_socket is not null && _socket.State == WebSocketState.Open)
        {
            ms.Seek(0, SeekOrigin.Begin);
            WebSocketReceiveResult receiveResult;
            int totalLength = 0;

            do
            {
                receiveResult = await _socket.ReceiveAsync(buffer, _connectionHandlerCts.Token);

                if(receiveResult.Count != 0)
                {
                    ms.Write(buffer.AsSpan(0, receiveResult.Count));
                }

                totalLength += receiveResult.Count;
            }
            while(!receiveResult.EndOfMessage);

            var msBuffer = ms.GetBuffer().AsSpan(0, (int) ms.Position);

            if(msBuffer.Length == 0)
            {
                continue;
            }

            IdentifyPayload(msBuffer, out var payloadType, out int requestId, out string subscriptionId);

            switch(payloadType)
            {
                case PayloadType.Response:
                    if(!_pendingRequests.TryRemove(requestId, out var value))
                    {
                        _logger?.LogDebug("Received response to request id that is not pending");
                        break;
                    }

                    var (responseType, tcs) = value;

                    try
                    {
                        object? response = JsonSerializer.Deserialize(
                            msBuffer, responseType,
                            options: ParsingUtils.EvmSerializerOptions
                        )!;
                        tcs.SetResult(response);
                    }
                    catch(Exception ex)
                    {
                        tcs.SetException(ex);
                    }
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

    private void IdentifyPayload(ReadOnlySpan<byte> jsonSpan,
        out PayloadType payloadType, out int requestId, out string subscriptionId)
    {
        requestId = -1;
        subscriptionId = null!;
        payloadType = PayloadType.Unknown;

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
                            requestId = Int32.Parse(
                                reader.GetString()!.AsSpan()[2..],
                                System.Globalization.NumberStyles.HexNumber,
                                System.Globalization.CultureInfo.InvariantCulture
                            );
                            payloadType = PayloadType.Response;
                            return;
                        }
                        else if(reader.ValueTextEquals("method"))
                        {
                            reader.Read();
                            string? method = reader.GetString();

                            if(method != "eth_subscription")
                            {
                                _logger?.LogWarning("Failed to identify payload with method {Method}", method);
                                return;
                            }

                            reader.Read(); //eth_subscription string

                            if(reader.TokenType != JsonTokenType.PropertyName || !reader.ValueTextEquals("params"))
                            {
                                _logger?.LogWarning("Failed to identify payload, eth_subscription not followed by params property");
                                return;
                            }

                            reader.Read(); //params property name

                            if(reader.TokenType != JsonTokenType.StartObject)
                            {
                                _logger?.LogWarning("Failed to identify payload, eth_subscription not followed by params object");
                                return;
                            }

                            reader.Read(); //start params object

                            while(reader.TokenType == JsonTokenType.PropertyName)
                            {
                                if(reader.ValueTextEquals("result"))
                                {
                                    reader.Skip();
                                    reader.Read();
                                }
                                else if(reader.ValueTextEquals("subscription"))
                                {
                                    reader.Read();
                                    subscriptionId = reader.GetString()!;
                                    payloadType = PayloadType.Subscription;
                                    return;
                                }
                            }

                            _logger?.LogWarning("Failed to identify payload, eth_subscription params not containing subscription id");
                            return;
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

            _logger?.LogWarning("Failed to identify payload, no marker found till end of payload");
            return;
        }
        catch(Exception ex)
        {
            _logger?.LogWarning(ex, "Exception while trying to identify payload");
            return;
        }
    }

    private enum PayloadType
    {
        Response,
        Subscription,
        Unknown
    }

    private record JsonRpcRequest(int Id, string Method, object?[] Params, string Jsonrpc = "2.0");
    /// <inheritdoc />
    public async Task<RpcResult<TResult>> SendRpcRequestAsync<TResult>(
        string method, object?[] parameters, TargetBlockNumber requiredBlockNumber, CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        int requestId = Interlocked.Increment(ref _requestIdCounter);

        byte[] payload = JsonSerializer.SerializeToUtf8Bytes(
            new JsonRpcRequest(requestId, method, parameters),
            options: ParsingUtils.EvmSerializerOptions
        );

        var tcs = new TaskCompletionSource<object>();
        var timeoutTask = Task.Delay(_requestTimeout, cancellationToken);

        _pendingRequests.TryAdd(requestId, (typeof(JsonRpcResponse<TResult>), tcs));

        try
        {
            if(_socket is null || _socket.State != WebSocketState.Open)
            {
                throw new RPCTransportException("The WebSocket is not connected.");
            }
        }
        catch(ObjectDisposedException)
        {
            throw new RPCTransportException("The WebSocket is not connected.");
        }

        await _socket.SendAsync(payload, WebSocketMessageType.Text, true, cancellationToken);

        var resultTask = await Task.WhenAny(tcs.Task, timeoutTask);

        if(resultTask == timeoutTask)
        {
            _pendingRequests.TryRemove(requestId, out _);

            if(resultTask.IsCompletedSuccessfully)
            {
                throw new TimeoutException($"No response received from server within {_requestTimeout} timeout");
            }
        }

        if(resultTask.IsCanceled)
        {
            await resultTask;
        }

        var jsonRpcResponse = (JsonRpcResponse<TResult>) await tcs.Task;

        if(jsonRpcResponse is null)
        {
            throw new RPCTransportException("RPC Error: Invalid response");
        }
        else if(jsonRpcResponse.Id != requestId)
        {
            throw new RPCTransportException("RPC Error: Invalid response Id");
        }
        else if(jsonRpcResponse.Error != null)
        {
            return new RpcResult<TResult>.Error(jsonRpcResponse.Error.Code, jsonRpcResponse.Error.Message, jsonRpcResponse.Error.Data);
        }
        else if(jsonRpcResponse.Result is null)
        {
            return new RpcResult<TResult>.Null();
        }
        //
        return new RpcResult<TResult>.Success(jsonRpcResponse.Result);
    }

    /// <inheritdoc/>
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
