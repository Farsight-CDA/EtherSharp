using EtherSharp.Interpreter.Runtime;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Runtime.Precompiles;

/// <summary>
/// Executes a native contract implementation selected by an EVM execution specification.
/// </summary>
public interface IPrecompile
{
    /// <summary>The precompile's execution address.</summary>
    public Address Address { get; }

    /// <summary>Executes the precompile.</summary>
    /// <param name="host">The upstream host available to the precompile.</param>
    /// <param name="call">The message-call context and input.</param>
    /// <returns>The EVM execution outcome, including any specific precompile failure reason.</returns>
    public ValueTask<ExecutionResult> ExecuteAsync(IInterpreterHost host, PrecompileCall call);
}
