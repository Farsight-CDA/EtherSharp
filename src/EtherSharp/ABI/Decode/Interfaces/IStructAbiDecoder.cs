namespace EtherSharp.ABI.Decode.Interfaces;
public partial interface IStructAbiDecoder
{
    public TNumber Number<TNumber>(bool isUnsigned, int bitLength);
    public ReadOnlySpan<byte> Bytes();
    public string String();

    public T Struct<T>(Func<IStructAbiDecoder, T> func);
    public T[] Array<T>(out T[] value, Func<IArrayAbiDecoder, T> func);
    public TNumber[] NumberArray<TNumber>(bool isUnsigned, uint bitLength);
}
