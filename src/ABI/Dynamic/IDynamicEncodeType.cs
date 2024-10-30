namespace EtherSharp.ABI.Dynamic;
internal interface IDynamicEncodeType : IEncodeType
{
    public uint PayloadSize { get; }

    public void Encode(Span<byte> metadata, Span<byte> payload, uint payloadOffset);
}

