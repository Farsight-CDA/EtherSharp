namespace EtherSharp.ABI.Types.Base;

/// <summary>
/// Represents an ABI type that will be ABI encoded into the tail section of the encoding.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="value"></param>
public abstract class DynamicType<T>(T value) : IDynamicType
{
    public abstract uint PayloadSize { get; }

    public readonly T Value = value;

    public abstract void Encode(Span<byte> metadata, Span<byte> payload, uint payloadOffset);
}