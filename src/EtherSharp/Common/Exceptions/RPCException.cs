using EtherSharp.Client.Services.RPC;

namespace EtherSharp.Common.Exceptions;
/// <summary>
/// An exception thrown when RPC returns an error.
/// </summary>
/// <param name="code"></param>
/// <param name="message"></param>
/// <param name="data"></param>
public class RPCException(int code, string message, string? data)
    : Exception($"RPC Error Code {code}: {message}{(data is not null ? $" ({data})" : "")}")
{
    /// <summary>
    /// Response status code of the error.
    /// </summary>
    public int Code { get; } = code;

    /// <summary>
    /// The data field of the error.
    /// </summary>
    public string? ErrorData { get; } = data;

    /// <summary>
    /// Creates a new instance of RPCException from the given RpcResult Error.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="errorResult"></param>
    /// <returns></returns>
    public static RPCException FromRPCError<TResult>(RpcResult<TResult>.Error errorResult)
    {
        ArgumentNullException.ThrowIfNull(errorResult, nameof(errorResult));
        return new RPCException(errorResult.Code, errorResult.Message, errorResult.Data);
    }
}
