using EtherSharp.Numerics;

namespace EtherSharp.Interpreter.Memory;

/// <summary>
/// Indicates that an EVM memory access exceeds the configured per-frame limit.
/// </summary>
/// <param name="offset">The requested memory offset.</param>
/// <param name="length">The requested memory length.</param>
/// <param name="maxMemorySize">The configured maximum active memory size.</param>
public sealed class MemoryLimitExceededException(
    UInt256 offset,
    UInt256 length,
    int maxMemorySize
) : Exception($"Memory access at offset {offset} with length {length} exceeds the configured maximum of {maxMemorySize} bytes.")
{
    /// <summary>
    /// Gets the requested memory offset.
    /// </summary>
    public UInt256 Offset { get; } = offset;

    /// <summary>
    /// Gets the requested memory length.
    /// </summary>
    public UInt256 Length { get; } = length;

    /// <summary>
    /// Gets the configured maximum active memory size.
    /// </summary>
    public int MaxMemorySize { get; } = maxMemorySize;
}
