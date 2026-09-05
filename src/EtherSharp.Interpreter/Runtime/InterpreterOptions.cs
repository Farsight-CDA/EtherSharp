using EtherSharp.Types;

namespace EtherSharp.Interpreter.Runtime;

/// <summary>
/// Configures resource limits for interpreter execution.
/// </summary>
public sealed class InterpreterOptions
{
    /// <summary>
    /// Gets the default interpreter resource limits.
    /// </summary>
    public static InterpreterOptions Default { get; } = new();

    /// <summary>
    /// Gets the maximum number of bytes of active memory permitted in each call frame.
    /// </summary>
    public int MaxMemorySize { get; init; } = 64 * 1024 * 1024;

    /// <summary>
    /// Validates these options.
    /// </summary>
    /// <returns>These validated options.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <see cref="MaxMemorySize"/> is invalid.
    /// </exception>
    internal InterpreterOptions Validate()
    {
        ArgumentOutOfRangeException.ThrowIfNegative(MaxMemorySize);

        return (MaxMemorySize & (Bytes32.BYTE_LENGTH - 1)) != 0
            ? throw new ArgumentOutOfRangeException(
                nameof(MaxMemorySize),
                $"Maximum memory size must be a multiple of {Bytes32.BYTE_LENGTH} bytes."
            )
            : this;
    }
}
