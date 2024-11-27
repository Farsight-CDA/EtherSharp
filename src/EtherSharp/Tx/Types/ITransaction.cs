namespace EtherSharp.Tx.Types;
public interface ITransaction<TSelf, TTxParams>
    where TTxParams : ITxParams
    where TSelf : ITransaction<TSelf, TTxParams>
{
    public static abstract int NestedListCount { get; }
    public static abstract byte PrefixByte { get; }

    public static abstract TSelf Create(ulong chainId, TTxParams txParams, ITxInput txInput, uint nonce);

    public int GetEncodedSize(ReadOnlySpan<byte> data, Span<int> listLengths);
    public void Encode(ReadOnlySpan<int> listLengths, ReadOnlySpan<byte> data, Span<byte> destination);
}
