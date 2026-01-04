using EtherSharp.RLP;
using EtherSharp.Tx.Types;
using System.Numerics;

namespace EtherSharp.Tx.EIP1559;

public record EIP1559Transaction(
    ulong ChainId,
    ulong Gas,
    uint Nonce,
    ITxInput Input,
    BigInteger MaxFeePerGas,
    BigInteger MaxPriorityFeePerGas,
    StateAccess[] AccessList
) : ITransaction<EIP1559Transaction, EIP1559TxParams, EIP1559GasParams>
{
    public static int NestedListCount => 2;
    public static byte PrefixByte => 0x02;

    public static EIP1559Transaction Create(ulong chainId, EIP1559TxParams txParams, EIP1559GasParams gasParams, ITxInput txInput, uint nonce)
        => new EIP1559Transaction(
            chainId,
            gasParams.GasLimit,
            nonce,
            txInput,
            gasParams.MaxFeePerGas,
            gasParams.MaxPriorityFeePerGas,
            txParams.AccessList
        );

    public int GetEncodedSize(Span<int> listLengths)
    {
        listLengths[1] = TxRLPEncoder.GetAccessListLength(AccessList);

        int contentSize =
            RLPEncoder.GetIntSize(ChainId) +
            RLPEncoder.GetIntSize(Nonce) +
            RLPEncoder.GetIntSize(MaxPriorityFeePerGas) +
            RLPEncoder.GetIntSize(MaxFeePerGas) +
            RLPEncoder.GetIntSize(Gas) +
            RLPEncoder.GetStringSize(Input.To is null ? [] : Input.To.Bytes) +
            RLPEncoder.GetIntSize(Input.Value) +
            RLPEncoder.GetStringSize(Input.Data.Span) +
            RLPEncoder.GetListSize(listLengths[1]);

        listLengths[0] = contentSize;

        return RLPEncoder.GetListSize(contentSize);
    }

    public void Encode(ReadOnlySpan<int> listLengths, Span<byte> destination)
        => new RLPEncoder(destination)
            .EncodeList(listLengths[0])
                .EncodeInt(ChainId)
                .EncodeInt(Nonce)
                .EncodeInt(MaxPriorityFeePerGas)
                .EncodeInt(MaxFeePerGas)
                .EncodeInt(Gas)
                .EncodeString(Input.To is null ? [] : Input.To.Bytes)
                .EncodeInt(Input.Value)
                .EncodeString(Input.Data.Span)
                .EncodeList(listLengths[1])
                    .EncodeAccessList(AccessList);
}
