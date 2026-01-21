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

    public Task<RpcResult<TResult>> SendRpcRequestAsync<TResult>(
        string method, object?[] parameters, TargetBlockNumber requiredBlockNumber, CancellationToken cancellationToken)
    {
        Func<CancellationToken, Task<RpcResult<TResult>>> onNext = (ct) =>
        {
            try
            {
                return _transport.SendRpcRequestAsync<TResult>(method, parameters, requiredBlockNumber, ct);
            }
            catch(Exception ex)
            {
                if(ex is RPCTransportException || (ex is OperationCanceledException && cancellationToken.IsCancellationRequested))
                {
                    throw;
                }

                throw new RPCTransportException("Exception while calling RPC transport", ex);
            }
        };

        foreach(var middleware in _middlewares)
        {
            var next = onNext;
            onNext = (ct) => middleware.HandleAsync(next, ct);
        }

        return onNext(cancellationToken);
    }
}
