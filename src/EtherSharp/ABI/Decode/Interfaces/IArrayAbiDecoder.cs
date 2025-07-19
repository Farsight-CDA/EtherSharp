using EtherSharp.Types;

namespace EtherSharp.ABI.Decode.Interfaces;
public partial interface IArrayAbiDecoder
{
    public bool[] BoolArray();
    public Address[] AddressArray();
    public TNumber[] NumberArray<TNumber>(bool isUnsigned, uint bitLength);

    public string[] StringArray();

    public T[] Array<T>(Func<IArrayAbiDecoder, T> func);

    public T FixedTuple<T>(Func<IFixedTupleDecoder, T> func);
    public T DynamicTuple<T>(Func<IDynamicTupleDecoder, T> func);
}
