using EtherSharp.Types;

namespace EtherSharp.Interpreter.Runtime.Precompiles;

/// <summary>
/// Rejects execution of a precompile that is not implemented.
/// </summary>
/// <param name="address">The unsupported precompile address.</param>
public sealed class UnsupportedPrecompile(Address address) : IPrecompile
{
    /// <inheritdoc/>
    public Address Address { get; } = address;

    /// <inheritdoc/>
    /// <exception cref="NotSupportedException">The precompile is not implemented.</exception>
    public ValueTask<TxCallResult> ExecuteAsync(IInterpreterHost host, PrecompileCall call)
        => throw new NotSupportedException($"Precompile at {Address} is not implemented.");
}
