using BenchmarkDotNet.Attributes;
using EtherSharp.Query;
using EtherSharp.Types;

namespace EtherSharp.Bench.Query;

[MemoryDiagnoser]
public class GetNonceQueryBenchmarks
{
    private const int SEARCH_COUNT = 16_384;

    private IQuery<NonceSearchResult> _query = null!;
    private ReadOnlyMemory<byte>[] _queryResults = null!;

    [GlobalSetup]
    public void Setup()
    {
        var account = Address.Parse("0x1234567890123456789012345678901234567890");
        _query = IQuery.GetNonce(searchCount: SEARCH_COUNT);

        var createdAddress = Address.DeriveCreate(in account, SEARCH_COUNT - 1);
        byte[] queryResult = new byte[2 * Address.BYTES_LENGTH];
        account.CopyTo(queryResult);
        createdAddress.CopyTo(queryResult.AsSpan(Address.BYTES_LENGTH));
        _queryResults = [queryResult];
    }

    [Benchmark]
    public NonceSearchResult ReadResult_LastNonceInDefaultRange()
        => _query.ReadResultFrom(_queryResults);
}
