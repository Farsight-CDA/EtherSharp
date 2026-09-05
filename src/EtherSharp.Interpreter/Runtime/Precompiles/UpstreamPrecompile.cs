using EtherSharp.Types;

namespace EtherSharp.Interpreter.Runtime.Precompiles;

/// <summary>
/// Delegates a standard, input-only precompile to the upstream execution environment.
/// </summary>
/// <param name="address">The upstream precompile address.</param>
public sealed class UpstreamPrecompile(Address address) : IPrecompile
{
    /// <inheritdoc/>
    public Address Address { get; } = address;

    /// <inheritdoc/>
    public ValueTask<TxCallResult> ExecuteAsync(IInterpreterHost host, PrecompileCall call)
        => new(host.CallPrecompileAsync(call.Caller, Address, call.Value, call.Input));
}
