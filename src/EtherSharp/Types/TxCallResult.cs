using EtherSharp.ABI;
using EtherSharp.ABI.Types;
using EtherSharp.Client.Services.RPC;
using EtherSharp.Common.Exceptions;

namespace EtherSharp.Types;
public abstract record TxCallResult
{
    private static ReadOnlySpan<byte> ErrorStringSignature => [0x08, 0xc3, 0x79, 0xa0];

    public record Success(byte[] Data) : TxCallResult;
    public record Reverted() : TxCallResult;
    public record RevertedWithMessage(string Message) : TxCallResult;
    public record RevertedWithCustomError(byte[] Data) : TxCallResult;

    /// <summary>
    /// Unwraps the TxCallResult by throwing if it was not a Success result.
    /// </summary>
    /// <returns></returns>
    public byte[] Unwrap() => this switch
    {
        Success s => s.Data,
        Reverted => throw new CallRevertedException("execution reverted"),
        RevertedWithMessage m => throw new CallRevertedException.CallRevertedWithMessageException(m.Message),
        RevertedWithCustomError c => throw new CallRevertedException.CallRevertedWithCustomErrorException(c.Data),
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

                if(errorResult.Data is null || errorResult.Data.Length == 0)
                {
                    return new Reverted();
                }

                var errorSignature = errorResult.Data.AsSpan(0, 4);

                if(errorSignature.SequenceEqual(ErrorStringSignature))
                {
                    return new RevertedWithMessage(
                        AbiTypes.String.Decode(errorResult.Data.AsMemory(4), 0)
                    );
                }
                //
                return new RevertedWithCustomError(errorResult.Data);
            }
            default:
                throw new NotSupportedException();
        }
    }
}
