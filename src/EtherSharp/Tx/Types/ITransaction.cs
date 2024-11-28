namespace EtherSharp.Tx.Types;
public interface ITransaction<TSelf, TTxParams, TTxGasParams>
    where TTxParams : ITxParams
    where TTxGasParams : ITxGasParams
    where TSelf : ITransaction<TSelf, TTxParams, TTxGasParams>
{
    public static abstract int NestedListCount { get; }
    public static abstract byte PrefixByte { get; }

    public static abstract TSelf Create(ulong chainId, TTxParams txParams, TTxGasParams txGasParams, ITxInput txInput, uint nonce, ulong gas);

    public int GetEncodedSize(ReadOnlySpan<byte> data, Span<int> listLengths);
    public void Encode(ReadOnlySpan<int> listLengths, ReadOnlySpan<byte> data, Span<byte> destination);
}
