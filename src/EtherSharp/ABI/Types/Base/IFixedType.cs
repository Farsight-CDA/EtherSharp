namespace EtherSharp.ABI.Types.Base;
internal interface IFixedType : IEncodeType
{
    public uint Size { get; }

    public void Encode(Span<byte> buffer);
}
