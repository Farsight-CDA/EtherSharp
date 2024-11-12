namespace EtherSharp.ABI.Decode.Interfaces;
public partial interface IArrayAbiDecoder
{
    public T Struct<T>(Func<IStructAbiDecoder, T> func);
    public T[] Array<T>(Func<IArrayAbiDecoder, T[]> func);
    public TNumber[] NumberArray<TNumber>(bool isUnsigned, uint bitLength);
}
