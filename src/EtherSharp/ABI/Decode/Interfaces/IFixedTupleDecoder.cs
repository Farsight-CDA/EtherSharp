namespace EtherSharp.ABI.Decode.Interfaces;
public partial interface IFixedTupleDecoder
{
    public bool Bool();
    public string Address();

    public ReadOnlySpan<byte> SizedBytes(int bitLength);
    public TNumber Number<TNumber>(bool isUnsigned, int bitLength);

    public T FixedTuple<T>(Func<IFixedTupleDecoder, T> func);
}
