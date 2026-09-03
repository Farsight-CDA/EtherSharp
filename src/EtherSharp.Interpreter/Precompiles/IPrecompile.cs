using EtherSharp.Interpreter.Runtime;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Precompiles;

/// <summary>
/// Executes a native contract implementation selected by an EVM execution specification.
/// </summary>
public interface IPrecompile
{
    /// <summary>
    /// Gets the address at which this precompile executes.
    /// </summary>
    public Address Address { get; }

    /// <summary>
    /// Executes the precompile for the supplied message call.
    /// </summary>
    /// <param name="host">The upstream host available to the precompile.</param>
    /// <param name="call">The message-call context and input.</param>
    /// <returns>The raw EVM call result.</returns>
    public ValueTask<TxCallResult> ExecuteAsync(IInterpreterHost host, PrecompileCall call);
}
