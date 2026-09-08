namespace EtherSharp.Interpreter.Runtime.Tracing;

/// <summary>Provides read-only access to interpreter memory.</summary>
public interface IInterpreterMemory
{
    /// <summary>Gets the active EVM memory size in bytes.</summary>
    int Size { get; }

    /// <summary>Gets a borrowed read-only view of an active memory range without expanding memory.</summary>
    /// <param name="offset">The zero-based byte offset.</param>
    /// <param name="length">The number of bytes to read.</param>
    /// <remarks>
    /// The returned memory is valid only during the awaited hook callback.
    /// An empty range at <see cref="Size"/> is valid.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    /// An argument is negative or the range extends beyond <see cref="Size"/>.
    /// </exception>
    ReadOnlyMemory<byte> Slice(int offset, int length);
}
