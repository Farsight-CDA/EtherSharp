using EtherSharp.Types;

namespace EtherSharp.Transport;
public interface IRPCTransport
{
    public bool SupportsFilters { get; }
    public bool SupportsSubscriptions { get; }

    public Task<RpcResult<TResult>> SendRpcRequest<TResult>(string method);
    public Task<RpcResult<TResult>> SendRpcRequest<T1, TResult>(string method, T1 t1);
    public Task<RpcResult<TResult>> SendRpcRequest<T1, T2, TResult>(string method, T1 t1, T2 t2);
    public Task<RpcResult<TResult>> SendRpcRequest<T1, T2, T3, TResult>(string method, T1 t1, T2 t2, T3 t3);
}
