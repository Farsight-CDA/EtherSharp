using EtherSharp.RLP;
using EtherSharp.Tx.Types;
using EtherSharp.Types;
using System.Numerics;

namespace EtherSharp.Tx.EIP1559;
public record EIP1559Transaction(
    ulong ChainId,
    ulong Gas,
    uint Nonce,
    Address To,
    BigInteger Value,
    BigInteger MaxFeePerGas,
    BigInteger MaxPriorityFeePerGas,
    StateAccess[] AccessList
) : ITransaction<EIP1559Transaction, EIP1559TxParams, EIP1559GasParams>
{
    public static int NestedListCount => 2;
    public static byte PrefixByte => 0x02;

    public static EIP1559Transaction Create(ulong chainId, EIP1559TxParams txParams, EIP1559GasParams txGasParams, 
        Address to, BigInteger value, uint nonce)
        => new EIP1559Transaction(
            chainId,
            txGasParams.GasLimit,
            nonce,
            to,
            value,
            txGasParams.MaxFeePerGas,
            txGasParams.MaxPriorityFeePerGas,
            txParams.AccessList
        );

    public int GetEncodedSize(ReadOnlySpan<byte> data, Span<int> listLengths)
    {
        listLengths[1] = TxRLPEncoder.GetAccessListLength(AccessList);

        int contentSize =
            RLPEncoder.GetIntSize(ChainId) +
            RLPEncoder.GetIntSize(Nonce) +
            RLPEncoder.GetIntSize(MaxPriorityFeePerGas) +
            RLPEncoder.GetIntSize(MaxFeePerGas) +
            RLPEncoder.GetIntSize(Gas) +
            RLPEncoder.GetStringSize(To.Bytes) +
            RLPEncoder.GetIntSize(Value) +
            RLPEncoder.GetStringSize(data) +
            RLPEncoder.GetListSize(listLengths[1]);

        listLengths[0] = contentSize;

        return RLPEncoder.GetListSize(
            contentSize
        );
    }

    public void Encode(ReadOnlySpan<int> listLengths, ReadOnlySpan<byte> data, Span<byte> destination)
        => new RLPEncoder(destination)
            .EncodeList(listLengths[0])
                .EncodeInt(ChainId)
                .EncodeInt(Nonce)
                .EncodeInt(MaxPriorityFeePerGas)
                .EncodeInt(MaxFeePerGas)
                .EncodeInt(Gas)
                .EncodeString(To.Bytes)
                .EncodeInt(Value)
                .EncodeString(data)
                .EncodeList(listLengths[1])
                    .EncodeAccessList(AccessList);
}
