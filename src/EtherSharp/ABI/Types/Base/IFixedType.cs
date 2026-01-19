namespace EtherSharp.ABI.Types.Base;

internal interface IFixedType : IEncodeType
{
    public int Size { get; }

    public void Encode(Span<byte> buffer);
}
