using EtherSharp.Contract;
using EtherSharp.Crypto;
using EtherSharp.Numerics;
using EtherSharp.RLP;
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
        int encodedAddressLength = RLPEncoder.GetStringSize(_account.DangerousGetReadOnlySpan());

        Span<byte> payload = stackalloc byte[RLPEncoder.GetListSize(encodedAddressLength + 1 + sizeof(ulong))];
        Span<byte> hash = stackalloc byte[Bytes32.BYTE_LENGTH];

        for(int i = 0; i < _searchCount; i++)
        {
            ulong nonce = _startNonce + (ulong) i;
            int encodedNonceLength = RLPEncoder.GetIntSize(nonce);
            int listContentLength = encodedAddressLength + encodedNonceLength;
            int payloadLength = RLPEncoder.GetListSize(listContentLength);
            new RLPEncoder(payload)
                .EncodeList(listContentLength)
                .EncodeString(_account.DangerousGetReadOnlySpan())
                .EncodeInt(nonce);
            Keccak256.TryHashData(payload[..payloadLength], hash);

            if(hash[^Address.BYTES_LENGTH..].SequenceEqual(createdAddress.DangerousGetReadOnlySpan()))
            {
                return new NonceSearchResult.Found(nonce);
            }
        }

        return new NonceSearchResult.NotFound();
    }
}
