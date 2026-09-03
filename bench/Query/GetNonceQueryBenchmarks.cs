using BenchmarkDotNet.Attributes;
using EtherSharp.Query;
using EtherSharp.Types;
using System.Buffers.Binary;

namespace EtherSharp.Bench.Query;

[MemoryDiagnoser]
public class GetNonceQueryBenchmarks
{
    private const int SEARCH_COUNT = 16_384;
    private const int RESULT_HEADER_LENGTH = 4;

    private IQuery<NonceSearchResult> _query = null!;
    private ReadOnlyMemory<byte>[] _queryResults = null!;

    [GlobalSetup]
    public void Setup()
    {
        var account = Address.Parse("0x1234567890123456789012345678901234567890");
        _query = IQuery.GetNonce(in account, searchCount: SEARCH_COUNT);

        var createdAddress = Address.DeriveCreate(in account, SEARCH_COUNT - 1);
        int innerResultLength = RESULT_HEADER_LENGTH + Bytes32.BYTE_LENGTH;
        byte[] queryResult = new byte[RESULT_HEADER_LENGTH + innerResultLength];

        // Query result headers pack success into the first byte and payload length into the remaining bytes.
        BinaryPrimitives.WriteUInt32BigEndian(queryResult, (uint) innerResultLength);
        queryResult[0] = 0x01;
        BinaryPrimitives.WriteUInt32BigEndian(queryResult.AsSpan(RESULT_HEADER_LENGTH), Bytes32.BYTE_LENGTH);
        queryResult[RESULT_HEADER_LENGTH] = 0x01;
        createdAddress.CopyTo(queryResult.AsSpan()[^Address.BYTES_LENGTH..]);
        _queryResults = [queryResult];
    }

    [Benchmark]
    public NonceSearchResult ReadResult_LastNonceInDefaultRange()
        => _query.ReadResultFrom(_queryResults);
}
