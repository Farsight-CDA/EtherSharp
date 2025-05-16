namespace EtherSharp.ABI.Types.Base;
internal interface IDynamicType : IEncodeType
{
    public uint PayloadSize { get; }

    public void Encode(Span<byte> metadata, Span<byte> payload, uint payloadOffset);
}

