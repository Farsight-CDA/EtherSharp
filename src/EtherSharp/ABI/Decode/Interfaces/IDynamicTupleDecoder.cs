using EtherSharp.Types;

namespace EtherSharp.ABI.Decode.Interfaces;
public partial interface IDynamicTupleDecoder
{
    public bool Bool();
    public Address Address();
    public string String();
    public ReadOnlySpan<byte> Bytes();

    public ReadOnlySpan<byte> SizedBytes(int bitLength);
    public TNumber Number<TNumber>(bool isUnsigned, int bitLength);

    public bool[] BoolArray();
    public Address[] AddressArray();
    public TNumber[] NumberArray<TNumber>(bool isUnsigned, uint bitLength);

    public string[] StringArray();
    public byte[][] BytesArray();

    public T[] Array<T>(Func<IArrayAbiDecoder, T> func);

    public T FixedTuple<T>(Func<IFixedTupleDecoder, T> func);
    public T DynamicTuple<T>(Func<IDynamicTupleDecoder, T> func);
}
