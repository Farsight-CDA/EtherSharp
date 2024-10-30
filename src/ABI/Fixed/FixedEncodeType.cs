namespace EtherSharp.ABI.Fixed;

internal abstract partial class FixedEncodeType<T>(T value) : IFixedEncodeType
{
    public uint MetadataSize => 32;

    public T Value { get; } = value;

    public abstract void Encode(Span<byte> buffer);
}
