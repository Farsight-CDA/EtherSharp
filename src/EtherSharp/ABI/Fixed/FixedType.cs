namespace EtherSharp.ABI.Fixed;

internal abstract partial class FixedType<T>(T value) : IFixedType
{
    public T Value { get; } = value;
    public virtual uint MetadataSize => 32;

    public abstract void Encode(Span<byte> buffer);
}
