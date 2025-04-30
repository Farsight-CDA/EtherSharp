namespace EtherSharp.ABI.Types.Interfaces;
internal interface IFixedType : IEncodeType
{
    public uint Size { get; }

    public void Encode(Span<byte> buffer);
}
