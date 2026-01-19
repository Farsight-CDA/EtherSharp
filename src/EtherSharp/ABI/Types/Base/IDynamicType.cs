namespace EtherSharp.ABI.Types.Base;

internal interface IDynamicType : IEncodeType
{
    public int PayloadSize { get; }

    public void Encode(Span<byte> metadata, Span<byte> payload, int payloadOffset);
}

