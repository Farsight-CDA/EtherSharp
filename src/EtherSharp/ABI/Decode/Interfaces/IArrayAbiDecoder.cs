namespace EtherSharp.ABI.Decode.Interfaces;
public partial interface IArrayAbiDecoder
{
    public T FixedTuple<T>(Func<IFixedTupleDecoder, T> func);
    public T DynamicTuple<T>(Func<IDynamicTupleDecoder, T> func);

    public T[] Array<T>(Func<IArrayAbiDecoder, T> func);
    public TNumber[] NumberArray<TNumber>(bool isUnsigned, uint bitLength);
}
