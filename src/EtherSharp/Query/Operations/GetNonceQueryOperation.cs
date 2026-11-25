using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.Query.Operations;

internal sealed class GetNonceQueryOperation : IQueryOperation, IQuery<NonceSearchResult>
{
    private readonly ulong _startNonce;
    private readonly int _searchCount;

    public QueryOperationId OperationId => QueryOperationId.GetNonce;
    public UInt256 EthValue => 0;

    public GetNonceQueryOperation(ulong startNonce, int searchCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(searchCount);
        _startNonce = startNonce;
        _searchCount = searchCount;
    }

    public int GetCallDataLength(QueryEncodingContext context)
        => 1;

    public void Encode(Span<byte> buffer, QueryEncodingContext context)
        => buffer[0] = (byte) QueryOperationId.GetNonce;

    public int ParseResultLength(ReadOnlySpan<byte> resultData)
        => 2 * Address.BYTES_LENGTH;

    NonceSearchResult IQuery<NonceSearchResult>.ReadResultFrom(
        params scoped ReadOnlySpan<ReadOnlyMemory<byte>> queryResults
    )
    {
        var result = queryResults[0].Span;
        var account = Address.FromBytes(result[..Address.BYTES_LENGTH]);
        var createdAddress = Address.FromBytes(result.Slice(Address.BYTES_LENGTH, Address.BYTES_LENGTH));
        return createdAddress != default && Address.TryFindCreateNonce(in account, in createdAddress, _startNonce, _searchCount, out ulong nonce)
            ? new NonceSearchResult.Found(nonce)
            : new NonceSearchResult.NotFound();
    }
}
