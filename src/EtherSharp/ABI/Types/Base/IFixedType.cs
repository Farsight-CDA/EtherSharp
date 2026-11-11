namespace EtherSharp.ABI.Types.Base;

internal interface IFixedType
{
    public int Size { get; }

    public void Encode(Span<byte> buffer);
}
