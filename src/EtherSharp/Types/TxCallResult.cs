using EtherSharp.Common.Exceptions;
using EtherSharp.RPC;

namespace EtherSharp.Types;

/// <summary>
/// Represents the raw outcome of an <c>eth_call</c>-style execution.
/// </summary>
/// <param name="success">Whether execution succeeded.</param>
/// <param name="data">The returned bytes for success, or the EVM revert payload for reverts.</param>
public readonly struct TxCallResult(bool success, ReadOnlyMemory<byte> data)
{
    /// <summary>
    /// Gets whether execution succeeded.
    /// </summary>
    public bool Success { get; } = success;

    /// <summary>
    /// Gets the returned bytes for successful execution, or the EVM revert payload when <see cref="Success"/> is <see langword="false"/>.
    /// </summary>
    public ReadOnlyMemory<byte> Data { get; } = data;

    /// <summary>
    /// Converts an RPC <c>eth_call</c> response into a <see cref="TxCallResult"/>.
    /// </summary>
    /// <param name="rpcResult">The low-level RPC response payload.</param>
    /// <returns>A raw result with <see cref="Success"/> set according to the execution outcome.</returns>
    /// <exception cref="RPCException">Thrown when the RPC response is an error unrelated to execution revert.</exception>
    public static TxCallResult ParseFrom(RpcResult<byte[]> rpcResult)
    {
        switch(rpcResult)
        {
            case RpcResult<byte[]>.Success successResult:
            {
                return new TxCallResult(true, successResult.Result);
            }
            case RpcResult<byte[]>.Error errorResult:
            {
                if(!errorResult.Message.StartsWith("execution reverted"))
                {
                    throw RPCException.FromRPCError(errorResult);
                }

                if(errorResult.Data is null || errorResult.Data.Length <= 2)
                {
                    return new TxCallResult(false, Array.Empty<byte>());
                }

                byte[] dataBytes = Convert.FromHexString(errorResult.Data.AsSpan(2));
                return new TxCallResult(false, dataBytes);
            }
            default:
                throw new NotSupportedException();
        }
    }
}
