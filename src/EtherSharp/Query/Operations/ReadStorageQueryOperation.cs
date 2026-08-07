using EtherSharp.Common.Exceptions;
using EtherSharp.Contract;
using EtherSharp.Crypto;
using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.Query.Operations;

internal sealed class ReadStorageQueryOperation(in Address contract, in Bytes32 slot, EVMByteCode? originalByteCode)
    : IQuery, IQuery<Bytes32>
{
    private readonly Bytes32 _slot = slot;

    public Address Contract { get; } = contract;

    public int CallDataLength => CallQueryEncoding.GetCallDataLength(Bytes4.BYTE_LENGTH + Bytes32.BYTE_LENGTH);
    public UInt256 EthValue => 0;

    void IQuery<Bytes32>.AddTo(QueryPlan plan)
    {
        if(originalByteCode is not { } byteCode)
        {
            plan.AddStateOverride(Contract, new AccountOverride(code: ReadStorageCode.Simple));
            plan.AddOperation(this);
            return;
        }

        var codeHash = Keccak256.HashData(byteCode.ByteCode.Span);
        var transientAddress = Address.FromBytes(codeHash.DangerousGetReadOnlySpan()[^Address.BYTES_LENGTH..]);

        plan.AddStateOverride(Contract, new AccountOverride(code: ReadStorageCode.CreatePreserving(transientAddress)));
        plan.AddStateOverride(transientAddress, new AccountOverride(code: byteCode.ByteCode));
        plan.AddOperation(this);
    }

    public void Encode(Span<byte> buffer)
    {
        var callData = CallQueryEncoding.EncodeHeader(
            Contract,
            0,
            Bytes4.BYTE_LENGTH + Bytes32.BYTE_LENGTH,
            buffer);
        ReadStorageCode.FunctionSelector.CopyTo(callData);
        _slot.CopyTo(callData[Bytes4.BYTE_LENGTH..]);
    }

    public int ParseResultLength(ReadOnlySpan<byte> resultData)
        => CallQueryEncoding.ParseResultLength(resultData);

    Bytes32 IQuery<Bytes32>.ReadResultFrom(params scoped ReadOnlySpan<ReadOnlyMemory<byte>> queryResults)
    {
        var (success, returnData) = CallQueryEncoding.ParseResult(queryResults[0]);

        return !success
            ? throw CallRevertedException.Parse(Contract, returnData.Span)
            : returnData.Length != Bytes32.BYTE_LENGTH
                ? throw new CallParsingException.MalformedCallDataException(
                    returnData,
                    new ArgumentException($"ReadStorage returned {returnData.Length} bytes, expected {Bytes32.BYTE_LENGTH}.")
                )
                : Bytes32.FromBytes(returnData.Span);
    }
}
