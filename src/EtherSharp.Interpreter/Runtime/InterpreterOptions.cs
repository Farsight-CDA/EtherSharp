using EtherSharp.Types;

namespace EtherSharp.Interpreter.Runtime;

/// <summary>
/// Configures resource limits for interpreter execution.
/// </summary>
public sealed class InterpreterOptions
{
    private static readonly int _maxMemorySize = Array.MaxLength - (Array.MaxLength % Bytes32.BYTE_LENGTH);

    /// <summary>
    /// Gets the maximum number of bytes of active memory permitted in each call frame.
    /// </summary>
    public int MaxMemorySize { get; init; } = 64 * 1024 * 1024;

    /// <summary>
    /// Creates and validates an independent copy of these options.
    /// </summary>
    /// <returns>The validated options copy.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <see cref="MaxMemorySize"/> is invalid.
    /// </exception>
    internal InterpreterOptions CloneAndValidate()
    {
        ArgumentOutOfRangeException.ThrowIfNegative(MaxMemorySize);

        return (MaxMemorySize & (Bytes32.BYTE_LENGTH - 1)) != 0
            || MaxMemorySize > _maxMemorySize
            ? throw new ArgumentOutOfRangeException(
                nameof(MaxMemorySize),
                $"Maximum memory size must be a multiple of {Bytes32.BYTE_LENGTH} bytes and no greater than {_maxMemorySize}."
            )
            : new InterpreterOptions
            {
                MaxMemorySize = MaxMemorySize
            };
    }
}
