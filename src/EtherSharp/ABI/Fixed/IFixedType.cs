using EtherSharp.ABI.Encode;

namespace EtherSharp.ABI.Fixed;
internal interface IFixedType : IEncodeType
{
    public uint MetadataSize { get; }

    public void Encode(Span<byte> buffer);
}
