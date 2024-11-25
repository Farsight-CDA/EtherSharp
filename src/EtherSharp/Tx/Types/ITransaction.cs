namespace EtherSharp.Tx.Types;
public interface ITransaction
{
    public static abstract int NestedListCount { get; }
    internal int GetEncodedSize(ReadOnlySpan<byte> data, Span<int> listLengths);
    internal void Encode(ReadOnlySpan<int> listLengths, ReadOnlySpan<byte> data, Span<byte> destination);
}
