using EtherSharp.Numerics;

namespace EtherSharp.Interpreter.Runtime.Memory;

/// <summary>
/// Indicates that an EVM memory access exceeds the configured per-frame limit.
/// </summary>
public sealed class MemoryLimitExceededException(
    UInt256 offset,
    UInt256 length,
    int maxMemorySize
) : Exception($"Memory access at offset {offset} with length {length} exceeds the configured maximum of {maxMemorySize} bytes.")
{
    /// <summary>The requested byte offset.</summary>
    public UInt256 Offset { get; } = offset;

    /// <summary>The requested length in bytes.</summary>
    public UInt256 Length { get; } = length;

    /// <summary>The active memory limit in bytes.</summary>
    public int MaxMemorySize { get; } = maxMemorySize;
}
