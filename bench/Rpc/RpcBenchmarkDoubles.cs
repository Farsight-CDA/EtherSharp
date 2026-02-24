using EtherSharp.RPC;
using EtherSharp.RPC.Transport;
using EtherSharp.Types;

namespace EtherSharp.Bench.Rpc;

internal sealed class BenchTransport : IRPCTransport
{
    private static readonly Task<RpcResult<ulong>> _chainIdResult =
        Task.FromResult<RpcResult<ulong>>(new RpcResult<ulong>.Success(1));

    private static readonly Task<RpcResult<int>> _intResult =
        Task.FromResult<RpcResult<int>>(new RpcResult<int>.Success(1));

    public bool SupportsFilters => true;
    public bool SupportsSubscriptions => false;

    public event Action? OnConnectionEstablished
    {
        add { }
        remove { }
    }

    public event Action<string, ReadOnlySpan<byte>>? OnSubscriptionMessage
    {
        add { }
        remove { }
    }

    public ValueTask InitializeAsync(CancellationToken cancellationToken = default)
        => ValueTask.CompletedTask;

    public Task<RpcResult<TResult>> SendRpcRequestAsync<TResult>(
        string method,
        TargetBlockNumber requiredBlockNumber,
        CancellationToken cancellationToken = default)
        => typeof(TResult) == typeof(ulong)
            ? (Task<RpcResult<TResult>>) (object) _chainIdResult
            : typeof(TResult) == typeof(int)
                ? (Task<RpcResult<TResult>>) (object) _intResult
                : throw new NotSupportedException($"Unsupported TResult: {typeof(TResult).Name}");

    public Task<RpcResult<TResult>> SendRpcRequestAsync<T1, TResult>(
        string method,
        T1 t1,
        TargetBlockNumber requiredBlockNumber,
        CancellationToken cancellationToken = default)
        => throw new NotSupportedException("One-parameter RPC calls are not used by this benchmark.");

    public Task<RpcResult<TResult>> SendRpcRequestAsync<T1, T2, TResult>(
        string method,
        T1 t1,
        T2 t2,
        TargetBlockNumber requiredBlockNumber,
        CancellationToken cancellationToken = default)
        => typeof(TResult) == typeof(int)
            ? (Task<RpcResult<TResult>>) (object) _intResult
            : throw new NotSupportedException($"Unsupported TResult: {typeof(TResult).Name}");

    public Task<RpcResult<TResult>> SendRpcRequestAsync<T1, T2, T3, TResult>(
        string method,
        T1 t1,
        T2 t2,
        T3 t3,
        TargetBlockNumber requiredBlockNumber,
        CancellationToken cancellationToken = default)
        => throw new NotSupportedException("Three-parameter RPC calls are not used by this benchmark.");
}

internal sealed class PassthroughMiddleware : IRpcMiddleware
{
    public Task<RpcResult<TResult>> HandleAsync<TResult>(
        Func<CancellationToken, Task<RpcResult<TResult>>> onNext,
        CancellationToken cancellationToken)
        => onNext(cancellationToken);
}
