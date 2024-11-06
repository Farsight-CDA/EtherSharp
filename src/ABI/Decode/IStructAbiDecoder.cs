namespace EtherSharp.ABI.Decode;
public interface IStructAbiDecoder
{
    public ReadOnlyMemory<byte> Bytes();

    public string String();

    public T Struct<T>(Func<IStructAbiDecoder, T> func);

    public T[] Array<T>(out T[] value, Func<IArrayAbiDecoder, T[]> func);

    public TNumber[] NumberArray<TNumber>(bool isUnsigned, uint bitLength);

    public TNumber Number<TNumber>(bool isUnsigned, int bitLength);
}
