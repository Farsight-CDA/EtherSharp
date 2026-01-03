using EtherSharp.RLP;
using EtherSharp.Tx.Types;
using System.Numerics;

namespace EtherSharp.Tx.Legacy;

public record LegacyTransaction(
    ulong ChainId,
    ulong Gas,
    uint Nonce,
    ITxInput Input,
    BigInteger GasPrice
) : ITransaction<LegacyTransaction, LegacyTxParams, LegacyGasParams>
{
    public static int NestedListCount => 1;

    public static LegacyTransaction Create(ulong chainId, LegacyTxParams txParams, LegacyGasParams gasParams, ITxInput txInput, uint nonce)
        => new LegacyTransaction(
            chainId,
            gasParams.GasLimit,
            nonce,
            txInput,
            gasParams.GasPrice
        );

    public int GetSignDataEncodedSize(Span<int> listLengths)
    {
        int contentSize =
            RLPEncoder.GetIntSize(Nonce) +
            RLPEncoder.GetIntSize(GasPrice) +
            RLPEncoder.GetIntSize(Gas) +
            RLPEncoder.GetStringSize(Input.To is null ? [] : Input.To.Bytes) +
            RLPEncoder.GetIntSize(Input.Value) +
            RLPEncoder.GetStringSize(Input.Data) +
            RLPEncoder.GetIntSize(ChainId) +
            (2 * RLPEncoder.GetIntSize(0));

        listLengths[0] = contentSize;

        return RLPEncoder.GetListSize(contentSize);
    }

    public int GetEncodedSize(Span<int> listLengths)
    {
        int contentSize =
            RLPEncoder.GetIntSize(Nonce) +
            RLPEncoder.GetIntSize(GasPrice) +
            RLPEncoder.GetIntSize(Gas) +
            RLPEncoder.GetStringSize(Input.To is null ? [] : Input.To.Bytes) +
            RLPEncoder.GetIntSize(Input.Value) +
            RLPEncoder.GetStringSize(Input.Data);

        listLengths[0] = contentSize;

        return RLPEncoder.GetListSize(contentSize);
    }

    public void EncodeSignData(ReadOnlySpan<int> listLengths, Span<byte> destination)
        => new RLPEncoder(destination)
            .EncodeList(listLengths[0])
                .EncodeInt(Nonce)
                .EncodeInt(GasPrice)
                .EncodeInt(Gas)
                .EncodeString(Input.To is null ? [] : Input.To.Bytes)
                .EncodeInt(Input.Value)
                .EncodeString(Input.Data)
                .EncodeInt(ChainId)
                .EncodeInt(0)
                .EncodeInt(0);

    public void Encode(ReadOnlySpan<int> listLengths, Span<byte> destination, int signatureLength)
        => new RLPEncoder(destination)
            .EncodeList(listLengths[0] + signatureLength)
                .EncodeInt(Nonce)
                .EncodeInt(GasPrice)
                .EncodeInt(Gas)
                .EncodeString(Input.To is null ? [] : Input.To.Bytes)
                .EncodeInt(Input.Value)
                .EncodeString(Input.Data);
}
