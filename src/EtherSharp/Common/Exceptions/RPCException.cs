using EtherSharp.Types;

namespace EtherSharp.Common.Exceptions;
public class RPCException(int code, string message) 
    : Exception($"RPC Error Code {code}: {message}")
{
    public int Code { get; } = code;

    public static RPCException FromRPCError<TResult>(RpcResult<TResult>.Error errorResult) 
        => new RPCException(errorResult.Code, errorResult.Message);
}
