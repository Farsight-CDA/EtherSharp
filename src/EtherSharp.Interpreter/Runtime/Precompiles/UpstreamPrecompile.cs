using EtherSharp.Types;

namespace EtherSharp.Interpreter.Runtime.Precompiles;

/// <summary>
/// Delegates a standard, input-only precompile to the upstream execution environment.
/// </summary>
/// <remarks>
/// Upstream results do not report gas usage, so this implementation does not charge execution gas.
/// </remarks>
/// <param name="address">The upstream precompile address.</param>
public sealed class UpstreamPrecompile(Address address) : IPrecompile
{
    /// <inheritdoc/>
    public Address Address { get; } = address;

    /// <inheritdoc/>
    public async ValueTask<ExecutionResult> ExecuteAsync(IInterpreterHost host, PrecompileCall call)
    {
        var result = await host.CallPrecompileAsync(call.Caller, Address, call.Value, call.Input);
        return result.Success
            ? ExecutionResult.Success(result.Data)
            : ExecutionResult.PrecompileFailure(PrecompileFailureReason.Unspecified);
    }
}
