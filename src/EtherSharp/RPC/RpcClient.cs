using EtherSharp.Common.Exceptions;
using EtherSharp.RPC.Transport;
using EtherSharp.Types;
using Microsoft.Extensions.DependencyInjection;

namespace EtherSharp.RPC;

internal class RpcClient : IRpcClient
{
    private readonly IRPCTransport _transport;
    private readonly IRpcMiddleware[] _middlewares;

    public event Action? OnConnectionEstablished;
    public event Action<string, ReadOnlySpan<byte>>? OnSubscriptionMessage;

    public bool SupportsFilters => _transport.SupportsFilters;
    public bool SupportsSubscriptions => _transport.SupportsSubscriptions;

    private record LogParams(LogResponse Params);
    private record LogResponse(Log Result);

    public RpcClient(IRPCTransport transport, IServiceProvider serviceProvider)
    {
        _transport = transport;
        _middlewares = [.. serviceProvider.GetServices<IRpcMiddleware>().Reverse()];

        if(_transport.SupportsSubscriptions)
        {
            _transport.OnConnectionEstablished += () => OnConnectionEstablished?.Invoke();
            _transport.OnSubscriptionMessage += (subscriptionId, payload) => OnSubscriptionMessage?.Invoke(subscriptionId, payload);
        }
    }

    private Task<RpcResult<TResult>> ExecuteWithMiddlewareAsync<TResult>(
        Func<CancellationToken, Task<RpcResult<TResult>>> onNext,
        CancellationToken cancellationToken)
    {
        switch(_middlewares.Length)
        {
            case 0:
                return onNext(cancellationToken);
            case 1:
                return _middlewares[0].HandleAsync(onNext, cancellationToken);
            case 2:
                return _middlewares[1].HandleAsync(
                    (ct) => _middlewares[0].HandleAsync(onNext, ct),
                    cancellationToken
                );
            case 3:
                return _middlewares[2].HandleAsync(
                    (ct) => _middlewares[1].HandleAsync(
                        (innerCt) => _middlewares[0].HandleAsync(onNext, innerCt),
                        ct
                    ),
                    cancellationToken
                );
            case 4:
                return _middlewares[3].HandleAsync(
                    (ct) => _middlewares[2].HandleAsync(
                        (innerCt) => _middlewares[1].HandleAsync(
                            (innerInnerCt) => _middlewares[0].HandleAsync(onNext, innerInnerCt),
                            innerCt
                        ),
                        ct
                    ),
                    cancellationToken
                );
        }

        foreach(var middleware in _middlewares)
        {
            var next = onNext;
            onNext = (ct) => middleware.HandleAsync(next, ct);
        }

        return onNext(cancellationToken);
    }

    public Task<RpcResult<TResult>> SendRpcRequestAsync<TResult>(
        string method, TargetBlockNumber requiredBlockNumber, CancellationToken cancellationToken = default)
        => ExecuteWithMiddlewareAsync(
            (ct) => SendTransportRequestAsync<TResult>(method, requiredBlockNumber, ct),
            cancellationToken
        );

    public Task<RpcResult<TResult>> SendRpcRequestAsync<T1, TResult>(
        string method, T1 t1, TargetBlockNumber requiredBlockNumber, CancellationToken cancellationToken = default)
        => ExecuteWithMiddlewareAsync(
            (ct) => SendTransportRequestAsync<T1, TResult>(method, t1, requiredBlockNumber, ct),
            cancellationToken
        );

    public Task<RpcResult<TResult>> SendRpcRequestAsync<T1, T2, TResult>(
        string method, T1 t1, T2 t2, TargetBlockNumber requiredBlockNumber, CancellationToken cancellationToken = default)
        => ExecuteWithMiddlewareAsync(
            (ct) => SendTransportRequestAsync<T1, T2, TResult>(method, t1, t2, requiredBlockNumber, ct),
            cancellationToken
        );

    public Task<RpcResult<TResult>> SendRpcRequestAsync<T1, T2, T3, TResult>(
        string method, T1 t1, T2 t2, T3 t3, TargetBlockNumber requiredBlockNumber, CancellationToken cancellationToken = default)
        => ExecuteWithMiddlewareAsync(
            (ct) => SendTransportRequestAsync<T1, T2, T3, TResult>(method, t1, t2, t3, requiredBlockNumber, ct),
            cancellationToken
        );

    private async Task<RpcResult<TResult>> SendTransportRequestAsync<TResult>(
        string method, TargetBlockNumber requiredBlockNumber, CancellationToken cancellationToken)
    {
        try
        {
            return await _transport.SendRpcRequestAsync<TResult>(method, requiredBlockNumber, cancellationToken);
        }
        catch(Exception ex)
        {
            throw WrapTransportException(ex, cancellationToken);
        }
    }

    private async Task<RpcResult<TResult>> SendTransportRequestAsync<T1, TResult>(
        string method, T1 t1, TargetBlockNumber requiredBlockNumber, CancellationToken cancellationToken)
    {
        try
        {
            return await _transport.SendRpcRequestAsync<T1, TResult>(method, t1, requiredBlockNumber, cancellationToken);
        }
        catch(Exception ex)
        {
            throw WrapTransportException(ex, cancellationToken);
        }
    }

    private async Task<RpcResult<TResult>> SendTransportRequestAsync<T1, T2, TResult>(
        string method, T1 t1, T2 t2, TargetBlockNumber requiredBlockNumber, CancellationToken cancellationToken)
    {
        try
        {
            return await _transport.SendRpcRequestAsync<T1, T2, TResult>(method, t1, t2, requiredBlockNumber, cancellationToken);
        }
        catch(Exception ex)
        {
            throw WrapTransportException(ex, cancellationToken);
        }
    }

    private async Task<RpcResult<TResult>> SendTransportRequestAsync<T1, T2, T3, TResult>(
        string method, T1 t1, T2 t2, T3 t3, TargetBlockNumber requiredBlockNumber, CancellationToken cancellationToken)
    {
        try
        {
            return await _transport.SendRpcRequestAsync<T1, T2, T3, TResult>(method, t1, t2, t3, requiredBlockNumber, cancellationToken);
        }
        catch(Exception ex)
        {
            throw WrapTransportException(ex, cancellationToken);
        }
    }

    private static Exception WrapTransportException(Exception ex, CancellationToken cancellationToken)
        => ex is RPCTransportException || (ex is OperationCanceledException && cancellationToken.IsCancellationRequested)
            ? ex
            : new RPCTransportException("Exception while calling RPC transport", ex);
}
