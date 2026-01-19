
namespace EtherSharp.ABI.Types.Base;

/// <summary>
/// Represents an ABI type that will be ABI encoded into the head section of the encoding.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="value"></param>
public abstract class FixedType<T>(T value) : IFixedType
{
    public T Value { get; } = value;

    public virtual int Size => 32;

    public abstract void Encode(Span<byte> buffer);
}
