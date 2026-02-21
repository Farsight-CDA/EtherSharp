using EtherSharp.RPC;

namespace EtherSharp.Common.Exceptions;

/// <summary>
/// Represents a JSON-RPC error returned by the node.
/// </summary>
/// <param name="code">JSON-RPC error code.</param>
/// <param name="message">JSON-RPC error message.</param>
/// <param name="data">Optional JSON-RPC error data payload.</param>
public class RPCException(int code, string message, string? data)
    : Exception($"RPC Error Code {code}: {message}{(data is not null ? $" ({data})" : "")}")
{
    /// <summary>
    /// JSON-RPC error code returned by the node.
    /// </summary>
    public int Code { get; } = code;

    /// <summary>
    /// Optional JSON-RPC error data payload.
    /// </summary>
    public string? ErrorData { get; } = data;

    /// <summary>
    /// Creates an <see cref="RPCException"/> from a <see cref="RpcResult{TResult}.Error"/> payload.
    /// </summary>
    /// <typeparam name="TResult">Result type associated with the failed RPC method.</typeparam>
    /// <param name="errorResult">Error payload returned by the RPC client.</param>
    /// <returns>A populated <see cref="RPCException"/> instance.</returns>
    public static RPCException FromRPCError<TResult>(RpcResult<TResult>.Error errorResult)
    {
        ArgumentNullException.ThrowIfNull(errorResult, nameof(errorResult));
        return new RPCException(errorResult.Code, errorResult.Message, errorResult.Data);
    }
}
