namespace EtherSharp.ABI.Decode;
public interface IArrayAbiDecoder
{

    public T Struct<T>(Func<IStructAbiDecoder, T> func);
    public T[] Array<T>(Func<IArrayAbiDecoder, T[]> func);
}
