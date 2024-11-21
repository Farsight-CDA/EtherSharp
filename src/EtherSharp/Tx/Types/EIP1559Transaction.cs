using EtherSharp.RLP;
using EtherSharp.Types;
using System.Numerics;

namespace EtherSharp.Tx.Types;
public record EIP1559Transaction(
    ulong ChainId,
    ulong Gas,
    uint Nonce,
    Address To,
    BigInteger Value,
    BigInteger MaxFeePerGas,
    BigInteger MaxPriorityFeePerGas,
    StateAccess[] AccessList
) : ITransaction
{
    public static int NestedListCount => 2;

    void ITransaction.GetEncodedSize(ReadOnlySpan<byte> data, Span<int> listLengths) => GetEncodedSize(data, listLengths);
    internal void GetEncodedSize(ReadOnlySpan<byte> data, Span<int> listLengths)
    {
        listLengths[0] = TxRLPEncoder.GetAccessListLength(AccessList);
        listLengths[1] = RLPEncoder.GetListSize(
            RLPEncoder.GetIntSize(ChainId) +
            RLPEncoder.GetIntSize(Nonce) +
            RLPEncoder.GetIntSize(MaxPriorityFeePerGas) +
            RLPEncoder.GetIntSize(MaxFeePerGas) +
            RLPEncoder.GetIntSize(Gas) +
            RLPEncoder.GetStringSize(To.Bytes) +
            RLPEncoder.GetIntSize(Value) +
            RLPEncoder.GetStringSize(data) +
            RLPEncoder.GetListSize(listLengths[0]) 
        );
    }

    void ITransaction.Encode(ReadOnlySpan<int> listLengths, ReadOnlySpan<byte> data, Span<byte> destination) => Encode(listLengths, data, destination);
    internal void Encode(ReadOnlySpan<int> listLengths, ReadOnlySpan<byte> data, Span<byte> destination) 
        => new RLPEncoder(destination).EncodeList(listLengths[1])
            .EncodeInt(ChainId)
            .EncodeInt(Nonce)
            .EncodeInt(MaxPriorityFeePerGas)
            .EncodeInt(MaxFeePerGas)
            .EncodeInt(Gas)
            .EncodeString(To.Bytes)
            .EncodeInt(Value)
            .EncodeString(data)
            .EncodeList(listLengths[0])
            .EncodeAccessList(AccessList);
}
