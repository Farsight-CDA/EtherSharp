using EtherSharp.Common.Exceptions;
using EtherSharp.Common.Extensions;
using EtherSharp.Common.Instrumentation;
using EtherSharp.Transport;
using EtherSharp.Types;
using Microsoft.Extensions.DependencyInjection;

namespace EtherSharp.RPC;

internal class RpcClient : IRpcClient
{
    private readonly IRPCTransport _transport;
    private readonly IRpcMiddleware[] _middlewares;

    private readonly OTELCounter<long>? _rpcRequestsCounter;

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

        _rpcRequestsCounter = serviceProvider.CreateOTELCounter<long>("evm_rpc_requests");

        _rpcRequestsCounter?.Add(0, new KeyValuePair<string, object?>("status", "success"));
        _rpcRequestsCounter?.Add(0, new KeyValuePair<string, object?>("status", "failure"));

        if(_transport.SupportsSubscriptions)
        {
            _transport.OnConnectionEstablished += () => OnConnectionEstablished?.Invoke();
            _transport.OnSubscriptionMessage += (subscriptionId, payload) => OnSubscriptionMessage?.Invoke(subscriptionId, payload);
        }
    }

    public async Task<RpcResult<TResult>> SendRpcRequestAsync<TResult>(
        string method, object?[] parameters, TargetBlockNumber requiredBlockNumber, CancellationToken cancellationToken)
    {
        Func<CancellationToken, Task<RpcResult<TResult>>> onNext = async (ct) =>
        {
            try
            {
                var result = await _transport.SendRpcRequestAsync<TResult>(method, parameters, requiredBlockNumber, ct);
                _rpcRequestsCounter?.Add(1, new KeyValuePair<string, object?>("status", "success"));
                return result;
            }
            catch(Exception ex)
            {
                _rpcRequestsCounter?.Add(1, new KeyValuePair<string, object?>("status", "failure"));

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

        return await onNext(cancellationToken);
    }
}
