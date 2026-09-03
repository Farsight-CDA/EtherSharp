using EtherSharp.Contract;
using EtherSharp.Numerics;
using EtherSharp.Tx;
using EtherSharp.Types;

namespace EtherSharp.Query;

internal sealed class GetNonceQuery : IQuery<NonceSearchResult>
{
    private static IQuery<Address> Probe { get; } = IQuery.FlashCall(
        IFlashCode.FromRuntimeCode(new EVMByteCode(Convert.FromHexString("3060005260206000F3"))),
        IFlashCall.ForRawFlashCall(
            UInt256.Zero,
            ReadOnlyMemory<byte>.Empty,
            static data => Address.FromBytes(data.Span[^Address.BYTES_LENGTH..])
        )
    );

    private readonly Address _account;
    private readonly ulong _startNonce;
    private readonly int _searchCount;
    private readonly IQuery<Address> _probe;

    public GetNonceQuery(in Address account, ulong startNonce, int searchCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(searchCount);

        _account = account;
        _startNonce = startNonce;
        _searchCount = searchCount;
        _probe = IQuery.WithCaller(account, Probe);
    }

    int IQuery<NonceSearchResult>.OperationCount
        => _probe.OperationCount;

    void IQuery<NonceSearchResult>.AddTo(IQueryPlan plan)
        => plan.Add(_probe);

    NonceSearchResult IQuery<NonceSearchResult>.ReadResultFrom(
        params scoped ReadOnlySpan<ReadOnlyMemory<byte>> queryResults)
    {
        var createdAddress = _probe.ReadResultFrom(queryResults);

        for(int i = 0; i < _searchCount; i++)
        {
            ulong nonce = _startNonce + (ulong) i;

            if(Address.DeriveCreate(in _account, nonce) == createdAddress)
            {
                return new NonceSearchResult.Found(nonce);
            }
        }

        return new NonceSearchResult.NotFound();
    }
}
