namespace EtherSharp.Tx.Types;
public interface ITransaction
{
    public static abstract int NestedListCount { get; }
    public static abstract byte PrefixByte { get; }
    
    public int GetEncodedSize(ReadOnlySpan<byte> data, Span<int> listLengths);
    public void Encode(ReadOnlySpan<int> listLengths, ReadOnlySpan<byte> data, Span<byte> destination);
}
