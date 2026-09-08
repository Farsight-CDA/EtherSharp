using EtherSharp.Types;

namespace EtherSharp.Interpreter.Runtime.Tracing;

/// <summary>Reads the live operand stack without changing it.</summary>
public interface IInterpreterStack
{
    /// <summary>Gets the number of stack words.</summary>
    int Count { get; }

    /// <summary>Gets a stack word by its zero-based index from the top of the stack.</summary>
    /// <param name="index">The zero-based index from the top of the stack.</param>
    /// <returns>The stack word at <paramref name="index"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="index"/> is negative or greater than or equal to <see cref="Count"/>.
    /// </exception>
    Bytes32 Peek(int index);

    /// <summary>Attempts to get a stack word by its zero-based index from the top of the stack.</summary>
    /// <param name="index">The zero-based index from the top of the stack.</param>
    /// <param name="value">The stack word, or its default value when the index is invalid.</param>
    /// <returns><see langword="true"/> when the index is valid; otherwise, <see langword="false"/>.</returns>
    bool TryPeek(int index, out Bytes32 value);
}
