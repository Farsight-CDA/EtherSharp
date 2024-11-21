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
    public static int NestedListCount => 1;

    int ITransaction.GetEncodedSize(ReadOnlySpan<byte> data, Span<int> listLengths) 
        => GetEncodedSize(data, listLengths);
    internal int GetEncodedSize(ReadOnlySpan<byte> data, Span<int> listLengths)
    {
        listLengths[0] = TxRLPEncoder.GetAccessListLength(AccessList);
        return RLPEncoder.GetListSize(
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

    void ITransaction.Encode(int totalLength, ReadOnlySpan<int> listLengths, ReadOnlySpan<byte> data, Span<byte> destination) 
        => Encode(totalLength, listLengths, data, destination);
    internal void Encode(int totalLength, ReadOnlySpan<int> listLengths, ReadOnlySpan<byte> data, Span<byte> destination) 
        => new RLPEncoder(destination).EncodeList(totalLength)
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
