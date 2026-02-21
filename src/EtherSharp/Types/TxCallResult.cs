using EtherSharp.Common.Exceptions;
using EtherSharp.RPC;

namespace EtherSharp.Types;

/// <summary>
/// Represents the raw outcome of an <c>eth_call</c>-style execution.
/// </summary>
public abstract record TxCallResult
{
    /// <summary>
    /// Indicates execution succeeded and returned raw call data.
    /// </summary>
    /// <param name="Data">The returned bytes from the call.</param>
    public record Success(ReadOnlyMemory<byte> Data) : TxCallResult;

    /// <summary>
    /// Indicates execution reverted and returned raw revert data.
    /// </summary>
    /// <param name="Data">The EVM revert payload, if any.</param>
    public record Reverted(ReadOnlyMemory<byte> Data) : TxCallResult;

    /// <summary>
    /// Returns call data for successful execution, or throws a parsed revert exception.
    /// </summary>
    /// <param name="callToAddress">The target contract address used for exception parsing.</param>
    /// <returns>The successful call return bytes.</returns>
    /// <exception cref="CallRevertedException">Thrown when this instance is a <see cref="Reverted"/> result.</exception>
    public ReadOnlyMemory<byte> Unwrap(Address? callToAddress) => this switch
    {
        Success s => s.Data,
        Reverted r => throw CallRevertedException.Parse(callToAddress, r.Data.Span),
        _ => throw new NotImplementedException()
    };

    /// <summary>
    /// Converts an RPC <c>eth_call</c> response into a <see cref="TxCallResult"/>.
    /// </summary>
    /// <param name="rpcResult">The low-level RPC response payload.</param>
    /// <returns>A <see cref="Success"/> for normal responses, or <see cref="Reverted"/> for execution reverts.</returns>
    /// <exception cref="RPCException">Thrown when the RPC response is an error unrelated to execution revert.</exception>
    public static TxCallResult ParseFrom(RpcResult<byte[]> rpcResult)
    {
        switch(rpcResult)
        {
            case RpcResult<byte[]>.Success successResult:
            {
                return new Success(successResult.Result);
            }
            case RpcResult<byte[]>.Error errorResult:
            {
                if(!errorResult.Message.StartsWith("execution reverted"))
                {
                    throw RPCException.FromRPCError(errorResult);
                }

                if(errorResult.Data is null || errorResult.Data.Length <= 2)
                {
                    return new Reverted(Array.Empty<byte>());
                }

                byte[] dataBytes = Convert.FromHexString(errorResult.Data.AsSpan(2));
                return new Reverted(dataBytes);
            }
            default:
                throw new NotSupportedException();
        }
    }
}
