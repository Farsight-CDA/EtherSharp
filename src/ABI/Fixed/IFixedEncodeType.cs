namespace EtherSharp.ABI.Fixed;
internal interface IFixedEncodeType : IEncodeType
{
    public void Encode(Span<byte> buffer);
}
