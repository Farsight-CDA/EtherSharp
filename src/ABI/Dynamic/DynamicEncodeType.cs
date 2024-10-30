namespace EtherSharp.ABI.Dynamic;

internal abstract partial class DynamicEncodeType<T>(T value) : IDynamicEncodeType
{
    public abstract uint PayloadSize { get; }

    public readonly T Value = value;

    public abstract void Encode(Span<byte> metadata, Span<byte> payload, uint payloadOffset);
}