namespace EtherSharp.ABI.Dynamic;

internal abstract class DynamicType<T>(T value) : IDynamicType
{
    public abstract uint PayloadSize { get; }

    public readonly T Value = value;

    public abstract void Encode(Span<byte> metadata, Span<byte> payload, uint payloadOffset);
}