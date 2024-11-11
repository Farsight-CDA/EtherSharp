using EtherSharp.ABI.Encode;

namespace EtherSharp.ABI.Dynamic;
internal interface IDynamicType : IEncodeType
{
    public uint PayloadSize { get; }

    public void Encode(Span<byte> metadata, Span<byte> payload, uint payloadOffset);
}

