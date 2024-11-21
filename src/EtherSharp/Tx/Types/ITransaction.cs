namespace EtherSharp.Tx.Types;
public interface ITransaction
{
    public abstract static int NestedListCount { get; }
    internal int GetEncodedSize(ReadOnlySpan<byte> data, Span<int> listLengths);
    internal void Encode(int totalLength, ReadOnlySpan<int> listLengths, ReadOnlySpan<byte> data, Span<byte> destination);
}
