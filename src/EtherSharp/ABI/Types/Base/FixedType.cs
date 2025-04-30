using EtherSharp.ABI.Types.Interfaces;

namespace EtherSharp.ABI.Types.Base;

internal abstract class FixedType<T>(T value) : IFixedType
{
    public T Value { get; } = value;

    public virtual uint Size => 32;

    public abstract void Encode(Span<byte> buffer);
}
