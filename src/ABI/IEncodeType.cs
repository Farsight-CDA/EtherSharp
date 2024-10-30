namespace EVM.net.ABI;

public interface IEncodeType
{
    public int MetadataSize { get; }

    public int PayloadSize { get; }

    public void Encode(Span<byte> values);

    public void Encode(Span<byte> metadata, Span<byte> payload, int payloadOffset);
}