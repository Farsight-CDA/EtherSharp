namespace EtherSharp.ABI.Decode.Interfaces;
public partial interface IDynamicTupleDecoder
{
    public bool Bool();

    public TNumber Number<TNumber>(bool isUnsigned, int bitLength);
    public ReadOnlySpan<byte> Bytes();
    public string String();

    public T FixedTuple<T>(Func<IFixedTupleDecoder, T> func);
    public T DynamicTuple<T>(Func<IDynamicTupleDecoder, T> func);

    public T[] Array<T>(out T[] value, Func<IArrayAbiDecoder, T> func);
    public TNumber[] NumberArray<TNumber>(bool isUnsigned, uint bitLength);
}
