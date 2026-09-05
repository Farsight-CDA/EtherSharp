using EtherSharp.Interpreter.Runtime;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Runtime.Precompiles;

/// <summary>
/// Implements the EVM identity precompile.
/// </summary>
public sealed class IdentityPrecompile : IPrecompile
{
    /// <summary>
    /// Gets the shared identity precompile instance.
    /// </summary>
    public static IdentityPrecompile Instance { get; } = new();

    private IdentityPrecompile()
    {
    }

    /// <inheritdoc/>
    public Address Address { get; } = Address.FromString("0x0000000000000000000000000000000000000004");

    /// <inheritdoc/>
    public ValueTask<TxCallResult> ExecuteAsync(IInterpreterHost host, PrecompileCall call)
        => ValueTask.FromResult(new TxCallResult(true, call.Input.ToArray()));
}
