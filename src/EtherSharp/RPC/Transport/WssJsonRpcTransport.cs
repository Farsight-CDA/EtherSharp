using EtherSharp.Common;
using EtherSharp.Common.Exceptions;
using EtherSharp.Common.Extensions;
using EtherSharp.Common.Instrumentation;
using EtherSharp.RPC.Transport.Json;
using EtherSharp.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.RPC.Transport;
/// <summary>
/// Transport for EVM Websocket RPC
/// </summary>
public sealed class WssJsonRpcTransport : IRPCTransport, IAsyncDisposable
{
    /// <inheritdoc />
    public bool SupportsSubscriptions => true;

    /// <inheritdoc />
    public event Action? OnConnectionEstablished;
    /// <inheritdoc />
    public event Action<Exception>? OnConnectionLost;
    /// <inheritdoc />
    public event Action<string, ReadOnlySpan<byte>>? OnSubscriptionMessage;

    private readonly ILogger? _logger;

    private readonly Lock _statusLock = new Lock();
    private readonly SemaphoreSlim _lifecycleSemaphore = new SemaphoreSlim(1, 1);
    private bool _isInitialized;
    private bool _isDisposed;

    private readonly Uri _uri;
    private readonly TimeSpan _requestTimeout;
    private readonly Action<ClientWebSocketOptions>? _configureWebSocketOptions;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private ClientWebSocket? _socket = null!;

    private readonly CancellationTokenSource _connectionHandlerCts = new CancellationTokenSource();
    private Task? _connectionHandlerTask;

    private int _requestIdCounter;
    private readonly ConcurrentDictionary<int, IJsonRpcResponseHandler> _pendingRequests = [];
    private readonly SemaphoreSlim _sendSemaphore = new SemaphoreSlim(1, 1);

    private readonly ObservableGauge<int>? _websocketConnectedGauge;
    private readonly RpcRequestMetrics _rpcRequestMetrics;
    private readonly OTELCounter<long>? _subscriptionMessageCounter;

    /// <summary>
    /// Creates a websocket JSON-RPC transport bound to the provided endpoint URI.
    /// </summary>
    /// <param name="uri">Websocket RPC endpoint URI.</param>
    /// <param name="requestTimeout">Timeout used for each connection attempt and pending RPC calls.</param>
    /// <param name="provider">Service provider used for logging, instrumentation, and serializer configuration.</param>
    /// <param name="additionalTags">Additional OpenTelemetry tags.</param>
    /// <param name="configureWebSocketOptions">Optional callback invoked before every connection attempt, including reconnects. Compression is disabled unless configured through the socket options.</param>
    public WssJsonRpcTransport(
        Uri uri, TimeSpan requestTimeout, IServiceProvider provider, TagList additionalTags = default,
        Action<ClientWebSocketOptions>? configureWebSocketOptions = null
    )
    {
        _uri = uri;
        _requestTimeout = requestTimeout;
        _configureWebSocketOptions = configureWebSocketOptions;
        _jsonSerializerOptions = provider.GetService<JsonSerializerOptions>()
            ?? ParsingUtils.EvmSerializerOptions;
        _logger = provider.GetService<ILoggerFactory>()?.CreateLogger<WssJsonRpcTransport>();

        _websocketConnectedGauge = provider.CreateOTELObservableGauge("wss_connection_up",
            () => (_socket is not null && _socket.State == WebSocketState.Open) ? 1 : 0,
            tags: additionalTags
        );

        _rpcRequestMetrics = new RpcRequestMetrics(provider, additionalTags);

        _subscriptionMessageCounter = provider.CreateOTELCounter<long>("subscription_messages_received", tags: additionalTags);
        _subscriptionMessageCounter?.Add(0);
    }

    /// <inheritdoc />
    public async ValueTask InitializeAsync(CancellationToken cancellationToken)
    {
        await _lifecycleSemaphore.WaitAsync(cancellationToken);

        try
        {
            lock(_statusLock)
            {
                ObjectDisposedException.ThrowIf(_isDisposed, this);

                if(_isInitialized)
                {
                    return;
                }
            }

            using var initializationCts = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                _connectionHandlerCts.Token
            );

            await ConnectSocketAsync(initializationCts.Token);

            lock(_statusLock)
            {
                ObjectDisposedException.ThrowIf(_isDisposed, this);
                _isInitialized = true;
            }

            //Started outside the status lock, raises the initial connection event before initialization returns
            _connectionHandlerTask = ConnectionHandler();
        }
        finally
        {
            _lifecycleSemaphore.Release();
        }
    }

    private async Task ConnectionHandler()
    {
        while(true)
        {
            Exception connectionError;

            try
            {
                //Start the receive loop before notifying handlers so they can send and await RPC requests
                var messageHandlerTask = MessageHandler();
                RaiseConnectionEvent(OnConnectionEstablished, static handler => handler(), nameof(OnConnectionEstablished));
                await messageHandlerTask;

                if(_connectionHandlerCts.IsCancellationRequested)
                {
                    return;
                }

                connectionError = new RpcTransportException("The WebSocket is not connected.");
            }
            catch(Exception ex)
            {
                if(_connectionHandlerCts.IsCancellationRequested)
                {
                    return;
                }

                connectionError = ex;
            }

            _logger?.LogWarning(
                connectionError, "Websocket connection lost, killing {RequestCount} pending requests...", _pendingRequests.Count
            );

            RaiseConnectionEvent(OnConnectionLost, handler => handler(connectionError), nameof(OnConnectionLost));

            foreach(var r in _pendingRequests)
            {
                r.Value.SetException(connectionError);
            }

            _pendingRequests.Clear();

            if(!await ReconnectAsync())
            {
                return;
            }
        }
    }

    //Returns true on successful reconnection, false if the transport is being disposed
    private async Task<bool> ReconnectAsync()
    {
        while(true)
        {
            try
            {
                _socket?.Dispose();
                await ConnectSocketAsync(_connectionHandlerCts.Token);
                return true;
            }
            catch(Exception ex)
            {
                if(_connectionHandlerCts.IsCancellationRequested)
                {
                    return false;
                }

                _logger?.LogCritical(ex, "Websocket reconnection attempt crashed, retrying in 3s...");
            }

            try
            {
                await Task.Delay(3000, _connectionHandlerCts.Token);
            }
            catch(OperationCanceledException)
            {
                return false;
            }
        }
    }

    private void RaiseConnectionEvent<THandler>(THandler? handlers, Action<THandler> invoke, string eventName)
        where THandler : Delegate
    {
        foreach(var handler in Delegate.EnumerateInvocationList(handlers))
        {
            try
            {
                invoke(handler);
            }
            catch(Exception ex)
            {
                _logger?.LogWarning(ex, "{EventName} handler threw", eventName);
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

            _socket = new ClientWebSocket();
            _configureWebSocketOptions?.Invoke(_socket.Options);

            try
            {
                using var connectionCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                connectionCts.CancelAfter(_requestTimeout);

                await _socket.ConnectAsync(_uri, connectionCts.Token);
            }
            catch(Exception ex) when(!cancellationToken.IsCancellationRequested)
            {
                _logger?.LogDebug(ex, "Connection attempt failed, retrying in 3s...");
                await Task.Delay(3000, cancellationToken);
            }
        }

        _logger?.LogInformation("Websocket connection established...");
    }

    private async Task MessageHandler()
    {
        using var ms = new MemoryStream();
        byte[] buffer = new byte[32 * 1024];

        while(_socket is not null && _socket.State == WebSocketState.Open)
        {
            var receiveResult = await _socket.ReceiveAsync(buffer.AsMemory(), _connectionHandlerCts.Token);
            if(receiveResult.EndOfMessage)
            {
                ProcessMessage(buffer.AsSpan(0, receiveResult.Count));
                continue;
            }

            ms.Seek(0, SeekOrigin.Begin);
            if(receiveResult.Count != 0)
            {
                ms.Write(buffer.AsSpan(0, receiveResult.Count));
            }

            do
            {
                receiveResult = await _socket.ReceiveAsync(buffer.AsMemory(), _connectionHandlerCts.Token);

                if(receiveResult.Count != 0)
                {
                    ms.Write(buffer.AsSpan(0, receiveResult.Count));
                }
            }
            while(!receiveResult.EndOfMessage);

            ProcessMessage(ms.GetBuffer().AsSpan(0, (int) ms.Position));
        }
    }

    private void ProcessMessage(ReadOnlySpan<byte> message)
    {
        if(message.IsEmpty)
        {
            return;
        }

        IdentifyPayload(message, out var payloadType, out int requestId, out string subscriptionId);

        switch(payloadType)
        {
            case PayloadType.Response:
                if(!_pendingRequests.TryRemove(requestId, out var value))
                {
                    _logger?.LogDebug("Received response to request id that is not pending");
                    break;
                }

                try
                {
                    value.SetResponse(message, _jsonSerializerOptions);
                }
                catch(Exception ex)
                {
                    value.SetException(ex);
                }
                break;
            case PayloadType.Subscription:
                _subscriptionMessageCounter?.Add(1);
                DispatchSubscriptionMessage(subscriptionId, message);
                break;
            default:
                string payload = System.Text.Encoding.UTF8.GetString(message);
                _logger?.LogWarning("Received unidentified websocket payload: {payload}", payload);
                break;
        }
    }

    private void DispatchSubscriptionMessage(string subscriptionId, ReadOnlySpan<byte> payload)
    {
        var handlers = OnSubscriptionMessage;
        if(handlers is null)
        {
            return;
        }

        if(handlers.HasSingleTarget)
        {
            try
            {
                handlers(subscriptionId, payload);
            }
            catch(Exception ex)
            {
                _logger?.LogWarning(ex, "Subscription handler threw for subscription {SubscriptionId}", subscriptionId);
            }

            return;
        }

        foreach(var handler in Delegate.EnumerateInvocationList(handlers))
        {
            try
            {
                handler(subscriptionId, payload);
            }
            catch(Exception ex)
            {
                _logger?.LogWarning(ex, "Subscription handler threw for subscription {SubscriptionId}", subscriptionId);
            }
        }
    }

    internal void IdentifyPayload(ReadOnlySpan<byte> jsonSpan, out PayloadType payloadType, out int requestId, out string subscriptionId)
    {
        requestId = -1;
        subscriptionId = null!;
        payloadType = PayloadType.Unknown;

        try
        {
            var reader = new Utf8JsonReader(jsonSpan);
            bool isSubscriptionMethod = false;
            bool hasSubscriptionId = false;
            string parsedSubscriptionId = null!;
            reader.Read();

            if(reader.TokenType != JsonTokenType.StartObject)
            {
                _logger?.LogWarning("Failed to identify payload, JSON root is not an object");
                return;
            }

            reader.Read();

            while(reader.TokenType != JsonTokenType.None && reader.CurrentDepth > 0)
            {
                switch(reader.TokenType)
                {
                    case JsonTokenType.PropertyName:
                        if(reader.ValueTextEquals("id"u8))
                        {
                            reader.Read();

                            if(reader.TokenType == JsonTokenType.Number)
                            {
                                requestId = reader.GetInt32();
                            }
                            else
                            {
                                Span<char> buffer = stackalloc char[16];
                                int written = reader.CopyString(buffer);
                                var valueSpan = buffer[..written];

                                requestId = valueSpan.StartsWith("0x", StringComparison.Ordinal)
                                    ? Int32.Parse(
                                        valueSpan[2..],
                                        System.Globalization.NumberStyles.HexNumber,
                                        System.Globalization.CultureInfo.InvariantCulture
                                    )
                                    : Int32.Parse(valueSpan, System.Globalization.CultureInfo.InvariantCulture);
                            }

                            payloadType = PayloadType.Response;
                            return;
                        }
                        else if(reader.ValueTextEquals("method"u8))
                        {
                            reader.Read();

                            if(!reader.ValueTextEquals("eth_subscription"u8))
                            {
                                _logger?.LogWarning("Failed to identify payload with method {Method}", reader.GetString());
                                return;
                            }

                            isSubscriptionMethod = true;
                            if(hasSubscriptionId)
                            {
                                subscriptionId = parsedSubscriptionId;
                                payloadType = PayloadType.Subscription;
                                return;
                            }
                        }
                        else if(reader.ValueTextEquals("params"u8))
                        {
                            reader.Read();

                            if(reader.TokenType != JsonTokenType.StartObject)
                            {
                                reader.Skip();
                                break;
                            }

                            reader.Read();

                            while(reader.TokenType == JsonTokenType.PropertyName)
                            {
                                if(reader.ValueTextEquals("result"u8))
                                {
                                    reader.Skip();
                                    reader.Read();
                                }
                                else if(reader.ValueTextEquals("subscription"u8))
                                {
                                    reader.Read();
                                    parsedSubscriptionId = reader.GetString()!;
                                    hasSubscriptionId = true;

                                    if(isSubscriptionMethod)
                                    {
                                        subscriptionId = parsedSubscriptionId;
                                        payloadType = PayloadType.Subscription;
                                        return;
                                    }

                                    reader.Read();
                                }
                                else
                                {
                                    reader.Skip();
                                    reader.Read();
                                }
                            }
                        }

                        reader.Skip();
                        break;

                    case JsonTokenType.StartObject or JsonTokenType.StartArray:
                        reader.Skip();
                        break;
                    default:
                        if(!reader.Read())
                        {
                            _logger?.LogWarning("Failed to identify payload, failed to proceed reading");
                            return;
                        }
                        break;
                }
            }

            _logger?.LogWarning("Failed to identify payload, no marker found till end of payload");
        }
        catch(Exception ex)
        {
            _logger?.LogWarning(ex, "Exception while trying to identify payload");
        }
    }

    internal enum PayloadType
    {
        Response,
        Subscription,
        Unknown
    }

    /// <inheritdoc/>
    public Task<RpcResult<TResult>> SendRpcRequestAsync<TResult>(
        string method, TargetHeight requiredBlockNumber, RpcRequestOptions requestOptions = default,
        CancellationToken cancellationToken = default)
    {
        int requestId = Interlocked.Increment(ref _requestIdCounter);
        byte[] payload = JsonRpcRequestPayload.SerializeToUtf8Bytes(requestId, method, _jsonSerializerOptions);
        return SendRpcRequestInternalAsync<TResult>(requestId, method, payload, requestOptions, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<RpcResult<TResult>> SendRpcRequestAsync<T1, TResult>(
        string method, T1 t1, TargetHeight requiredBlockNumber, RpcRequestOptions requestOptions = default,
        CancellationToken cancellationToken = default)
    {
        int requestId = Interlocked.Increment(ref _requestIdCounter);
        byte[] payload = JsonRpcRequestPayload.SerializeToUtf8Bytes(requestId, method, t1, _jsonSerializerOptions);
        return SendRpcRequestInternalAsync<TResult>(requestId, method, payload, requestOptions, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<RpcResult<TResult>> SendRpcRequestAsync<T1, T2, TResult>(
        string method, T1 t1, T2 t2, TargetHeight requiredBlockNumber, RpcRequestOptions requestOptions = default,
        CancellationToken cancellationToken = default)
    {
        int requestId = Interlocked.Increment(ref _requestIdCounter);
        byte[] payload = JsonRpcRequestPayload.SerializeToUtf8Bytes(requestId, method, t1, t2, _jsonSerializerOptions);
        return SendRpcRequestInternalAsync<TResult>(requestId, method, payload, requestOptions, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<RpcResult<TResult>> SendRpcRequestAsync<T1, T2, T3, TResult>(
        string method, T1 t1, T2 t2, T3 t3, TargetHeight requiredBlockNumber, RpcRequestOptions requestOptions = default,
        CancellationToken cancellationToken = default)
    {
        int requestId = Interlocked.Increment(ref _requestIdCounter);
        byte[] payload = JsonRpcRequestPayload.SerializeToUtf8Bytes(requestId, method, t1, t2, t3, _jsonSerializerOptions);
        return SendRpcRequestInternalAsync<TResult>(requestId, method, payload, requestOptions, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<RpcResult<TResult>> SendRpcRequestAsync<T1, T2, T3, T4, TResult>(
        string method, T1 t1, T2 t2, T3 t3, T4 t4, TargetHeight requiredBlockNumber,
        RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default)
    {
        int requestId = Interlocked.Increment(ref _requestIdCounter);
        byte[] payload = JsonRpcRequestPayload.SerializeToUtf8Bytes(requestId, method, t1, t2, t3, t4, _jsonSerializerOptions);
        return SendRpcRequestInternalAsync<TResult>(requestId, method, payload, requestOptions, cancellationToken);
    }

    private async Task<RpcResult<TResult>> SendRpcRequestInternalAsync<TResult>(
        int requestId, string method, byte[] payload, RpcRequestOptions requestOptions, CancellationToken cancellationToken
    )
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        var responseHandler = new JsonRpcResponseHandler<TResult>();

        try
        {
            if(_socket is null || _socket.State != WebSocketState.Open)
            {
                throw new RpcTransportException("The WebSocket is not connected.");
            }

            _pendingRequests.TryAdd(requestId, responseHandler);

            try
            {
                await _sendSemaphore.WaitAsync(cancellationToken);
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if(requestOptions.TryGetValue(
                        RpcRequestOptionsKeys.Statistics,
                        out var statistics
                    ))
                    {
                        statistics.RecordRequest(method, payload.Length);
                    }
                    await _socket.SendAsync(payload, WebSocketMessageType.Text, true, cancellationToken);
                }
                finally
                {
                    _sendSemaphore.Release();
                }
            }
            catch
            {
                _pendingRequests.TryRemove(requestId, out _);
                throw;
            }
        }
        catch(ObjectDisposedException)
        {
            throw new RpcTransportException("The WebSocket is not connected.");
        }

        JsonRpcResponse<TResult> jsonRpcResponse;
        try
        {
            jsonRpcResponse = await responseHandler.Task.WaitAsync(_requestTimeout, cancellationToken);
        }
        catch(TimeoutException)
        {
            _pendingRequests.TryRemove(requestId, out _);
            _rpcRequestMetrics.Add(method, RpcRequestStatus.Failure);
            throw new TimeoutException($"No response received from server within {_requestTimeout} timeout");
        }
        catch(OperationCanceledException)
        {
            _pendingRequests.TryRemove(requestId, out _);
            _rpcRequestMetrics.Add(method, RpcRequestStatus.Failure);
            throw;
        }

        if(jsonRpcResponse.Id != requestId)
        {
            _rpcRequestMetrics.Add(method, RpcRequestStatus.Failure);
            throw new RpcTransportException("RPC Error: Invalid response Id");
        }
        else if(jsonRpcResponse.Error != null)
        {
            _rpcRequestMetrics.Add(method, RpcRequestStatus.Success);
            return new RpcResult<TResult>.Error(jsonRpcResponse.Error.Code, jsonRpcResponse.Error.Message, jsonRpcResponse.Error.Data);
        }
        else if(jsonRpcResponse.ResultIsNull)
        {
            _rpcRequestMetrics.Add(method, RpcRequestStatus.Success);
            return RpcResult<TResult>.Null.Instance;
        }
        //
        _rpcRequestMetrics.Add(method, RpcRequestStatus.Success);
        return new RpcResult<TResult>.Success(jsonRpcResponse.Result!);
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        lock(_statusLock)
        {
            if(_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            _connectionHandlerCts.Cancel();
        }

        await _lifecycleSemaphore.WaitAsync();

        try
        {
            if(_socket is not null)
            {
                try
                {
                    if(_socket.State is WebSocketState.Open or WebSocketState.CloseReceived or WebSocketState.CloseSent)
                    {
                        await _socket.CloseAsync(
                            WebSocketCloseStatus.NormalClosure,
                            "Disposing transport",
                            CancellationToken.None
                        );
                    }
                }
                catch(OperationCanceledException)
                {
                }
                catch(WebSocketException)
                {
                }
                catch(ObjectDisposedException)
                {
                }
                finally
                {
                    _socket.Abort();
                    _socket.Dispose();
                }
            }

            if(_connectionHandlerTask is not null)
            {
                try
                {
                    await _connectionHandlerTask;
                }
                catch(OperationCanceledException) when(_connectionHandlerCts.IsCancellationRequested)
                {
                }
            }
        }
        finally
        {
            _lifecycleSemaphore.Release();
            _connectionHandlerCts.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
