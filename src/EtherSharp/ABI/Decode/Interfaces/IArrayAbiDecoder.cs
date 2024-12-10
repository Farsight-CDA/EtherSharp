namespace EtherSharp.ABI.Decode.Interfaces;
public partial interface IArrayAbiDecoder
{
    public T[] Array<T>(Func<IArrayAbiDecoder, T> func);
    public TNumber[] NumberArray<TNumber>(bool isUnsigned, uint bitLength);
}
