using EtherSharp.Client.Services.RPC;

namespace EtherSharp.Common.Exceptions;
public class RPCException(int code, string message)
    : Exception($"RPC Error Code {code}: {message}")
{
    /// <summary>
    /// Response status code of the Error.
    /// </summary>
    public int Code { get; } = code;

    /// <summary>
    /// Creates a new instance of RPCException from the given RpcResult Error.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="errorResult"></param>
    /// <returns></returns>
    public static RPCException FromRPCError<TResult>(RpcResult<TResult>.Error errorResult)
    {
        ArgumentNullException.ThrowIfNull(errorResult, nameof(errorResult));
        return new RPCException(errorResult.Code, errorResult.Message);
    }
}
