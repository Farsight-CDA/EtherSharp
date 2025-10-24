using EtherSharp.Common.Exceptions;
using EtherSharp.RPC;

namespace EtherSharp.Types;

public abstract record TxCallResult
{
    public record Success(byte[] Data) : TxCallResult;
    public record Reverted(byte[] Data) : TxCallResult;

    /// <summary>
    /// Unwraps the TxCallResult by throwing if it was not a Success result.
    /// </summary>
    /// <returns></returns>
    public byte[] Unwrap(Address callToAddress) => this switch
    {
        Success s => s.Data,
        Reverted r => throw CallRevertedException.Parse(callToAddress, r.Data),
        _ => throw new NotImplementedException()
    };

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
                    return new Reverted([]);
                }

                byte[] dataBytes = Convert.FromHexString(errorResult.Data.AsSpan(2));
                return new Reverted(dataBytes);
            }
            default:
                throw new NotSupportedException();
        }
    }
}
