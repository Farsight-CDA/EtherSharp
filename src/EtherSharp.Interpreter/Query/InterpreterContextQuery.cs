using EtherSharp.Contract;
using EtherSharp.Interpreter.Runtime;
using EtherSharp.Numerics;
using EtherSharp.Query;
using EtherSharp.Tx;
using EtherSharp.Types;
using System.Buffers.Binary;

namespace EtherSharp.Interpreter.Query;

/// <summary>
/// Fetches a full <see cref="InterpreterContext"/> through batched queries.
/// </summary>
internal sealed class InterpreterContextQuery : IQuery<InterpreterContext>
{
    private const int MAX_RECENT_BLOCK_HASHES = 256;

    public static InterpreterContextQuery Instance { get; } = new();

    private static readonly IQuery<ulong> _chainId = IQuery.GetChainId();
    private static readonly IQuery<ulong> _blockNumber = IQuery.GetBlockNumber();
    private static readonly IQuery<DateTimeOffset> _blockTimestamp = IQuery.GetBlockTimestamp();
    private static readonly IQuery<ulong> _blockGasLimit = IQuery.GetBlockGasLimit();
    private static readonly IQuery<UInt256> _gasPrice = IQuery.GetBlockGasPrice();
    private static readonly IQuery<Address> _coinbase = IQuery.FlashCall(
        IFlashCode.FromRuntimeCode(new EVMByteCode(Convert.FromHexString("416000526014600cf3"))),
        IFlashCall.ForRawFlashCall(0, ReadOnlyMemory<byte>.Empty, static data => Address.FromBytes(data.Span)));
    private static readonly IQuery<UInt256> _prevRandao = IQuery.FlashCall(
        IFlashCode.FromRuntimeCode(new EVMByteCode(Convert.FromHexString("4460005260206000f3"))),
        IFlashCall.ForRawFlashCall(0, ReadOnlyMemory<byte>.Empty, static data => BinaryPrimitives.ReadUInt256BigEndian(data.Span)));
    private static readonly IQuery<CallResult<UInt256>> _baseFee = IQuery.SafeFlashCall(
        IFlashCode.FromRuntimeCode(new EVMByteCode(Convert.FromHexString("4860005260206000f3"))),
        IFlashCall.ForRawFlashCall(0, ReadOnlyMemory<byte>.Empty, static data => BinaryPrimitives.ReadUInt256BigEndian(data.Span)));
    private static readonly IQuery<CallResult<UInt256>> _blobBaseFee = IQuery.SafeFlashCall(
        IFlashCode.FromRuntimeCode(new EVMByteCode(Convert.FromHexString("4a60005260206000f3"))),
        IFlashCall.ForRawFlashCall(0, ReadOnlyMemory<byte>.Empty, static data => BinaryPrimitives.ReadUInt256BigEndian(data.Span)));
    private static readonly IQuery<Bytes32[]> _recentBlockHashes = IQuery.FlashCall(
        IFlashCode.FromRuntimeCode(new EVMByteCode(Convert.FromHexString(
            "6001804301905b61010181106015576120006000f35b6001906000838210602f575b600019820160051b52016006565b5080430340602156"))),
        IFlashCall.ForRawFlashCall(0, ReadOnlyMemory<byte>.Empty, static data =>
        {
            var span = data.Span;
            var hashes = new Bytes32[MAX_RECENT_BLOCK_HASHES];
            for(int i = 0; i < hashes.Length; i++)
            {
                hashes[i] = Bytes32.FromBytes(span.Slice(i * Bytes32.BYTE_LENGTH, Bytes32.BYTE_LENGTH));
            }
            return hashes;
        }));

    private InterpreterContextQuery()
    {
    }

    int IQuery<InterpreterContext>.OperationCount
        => _chainId.OperationCount
            + _blockNumber.OperationCount
            + _blockTimestamp.OperationCount
            + _blockGasLimit.OperationCount
            + _gasPrice.OperationCount
            + _coinbase.OperationCount
            + _prevRandao.OperationCount
            + _baseFee.OperationCount
            + _blobBaseFee.OperationCount
            + _recentBlockHashes.OperationCount;

    void IQuery<InterpreterContext>.AddTo(IQueryPlan plan)
    {
        plan.Add(_chainId);
        plan.Add(_blockNumber);
        plan.Add(_blockTimestamp);
        plan.Add(_blockGasLimit);
        plan.Add(_gasPrice);
        plan.Add(_coinbase);
        plan.Add(_prevRandao);
        plan.Add(_baseFee);
        plan.Add(_blobBaseFee);
        plan.Add(_recentBlockHashes);
    }

    InterpreterContext IQuery<InterpreterContext>.ReadResultFrom(params scoped ReadOnlySpan<ReadOnlyMemory<byte>> queryResults)
    {
        int offset = 0;

        ulong chainId = _chainId.ReadResultFrom(queryResults[offset..(offset += _chainId.OperationCount)]);
        ulong blockNumber = _blockNumber.ReadResultFrom(queryResults[offset..(offset += _blockNumber.OperationCount)]);
        var blockTimestamp = _blockTimestamp.ReadResultFrom(queryResults[offset..(offset += _blockTimestamp.OperationCount)]);
        ulong blockGasLimit = _blockGasLimit.ReadResultFrom(queryResults[offset..(offset += _blockGasLimit.OperationCount)]);
        var gasPrice = _gasPrice.ReadResultFrom(queryResults[offset..(offset += _gasPrice.OperationCount)]);
        var coinbase = _coinbase.ReadResultFrom(queryResults[offset..(offset += _coinbase.OperationCount)]);
        var prevRandao = _prevRandao.ReadResultFrom(queryResults[offset..(offset += _prevRandao.OperationCount)]);
        var baseFeeResult = _baseFee.ReadResultFrom(queryResults[offset..(offset += _baseFee.OperationCount)]);
        var blobBaseFeeResult = _blobBaseFee.ReadResultFrom(queryResults[offset..(offset += _blobBaseFee.OperationCount)]);
        var allRecentBlockHashes = _recentBlockHashes.ReadResultFrom(queryResults[offset..(offset += _recentBlockHashes.OperationCount)]);

        int recentHashCount = (int) Math.Min(blockNumber, (ulong) allRecentBlockHashes.Length);
        var recentBlockHashes = allRecentBlockHashes.AsSpan(0, recentHashCount).ToArray();

        return new InterpreterContext(
            chainId,
            blockNumber,
            blockTimestamp,
            recentBlockHashes,
            gasPrice,
            baseFeeResult switch
            {
                CallResult<UInt256>.Success success => success.Value,
                CallResult<UInt256>.Reverted => null,
                _ => baseFeeResult.Unwrap(),
            },
            blobBaseFeeResult switch
            {
                CallResult<UInt256>.Success success => success.Value,
                CallResult<UInt256>.Reverted => null,
                _ => blobBaseFeeResult.Unwrap(),
            },
            coinbase,
            prevRandao,
            (UInt256) blockGasLimit
        );
    }
}
