using EtherSharp.Numerics;
using EtherSharp.Tx;
using EtherSharp.Types;

namespace EtherSharp.Query.Operations;

internal sealed class CallQueryOperation<T>(IContractCall<T> txInput) : IQuery, IQuery<CallResult<T>>
{
    private readonly IContractCall<T> _txInput = txInput;
    private readonly Func<ReadOnlyMemory<byte>, T> _readResultFrom = txInput.ReadResultFrom;

    public int CallDataLength => CallQueryEncoding.GetCallDataLength(_txInput.Data.Length);
    public UInt256 EthValue => _txInput.Value;

    public void Encode(Span<byte> buffer)
        => _txInput.Data.Span.CopyTo(CallQueryEncoding.EncodeHeader(
            _txInput.To,
            EthValue,
            _txInput.Data.Length,
            buffer)
        );

    public int ParseResultLength(ReadOnlySpan<byte> resultData)
        => CallQueryEncoding.ParseResultLength(resultData);

    CallResult<T> IQuery<CallResult<T>>.ReadResultFrom(params ReadOnlySpan<ReadOnlyMemory<byte>> queryResults)
    {
        var (success, returnData) = CallQueryEncoding.ParseResult(queryResults[0]);

        return success switch
        {
            true => CallResult<T>.ParseSuccessFrom(returnData, _txInput.To, _readResultFrom),
            false => new CallResult<T>.Reverted(_txInput.To, returnData)
        };
    }
}
