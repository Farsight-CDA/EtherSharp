using EtherSharp.Interpreter.Runtime;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Runtime.Precompiles;

/// <summary>The EVM identity precompile.</summary>
public sealed class IdentityPrecompile : IPrecompile
{
    /// <summary>The shared instance.</summary>
    public static IdentityPrecompile Instance { get; } = new();

    private IdentityPrecompile()
    {
    }

    /// <inheritdoc/>
    public Address Address { get; } = Address.FromString("0x0000000000000000000000000000000000000004");

    /// <inheritdoc/>
    public ValueTask<ExecutionResult> ExecuteAsync(IInterpreterHost host, PrecompileCall call)
        => ValueTask.FromResult(ExecutionResult.Success(call.Input.ToArray()));
}
