namespace EtherSharp.ABI;

internal interface IDynamicEncodeType : IEncodeType
{
    public uint PayloadSize { get; }

    public void Encode(Span<byte> metadata, Span<byte> payload, uint payloadOffset);
}

internal abstract class DynamicEncodeType<T>(T value) : IDynamicEncodeType
{
    public abstract uint MetadataSize { get; }
    public abstract uint PayloadSize { get; }

    public readonly T Value = value;

    public abstract void Encode(Span<byte> metadata, Span<byte> payload, uint payloadOffset);
}