using EtherSharp.Types;

namespace EtherSharp.Interpreter.Precompiles;

/// <summary>
/// Executes a native contract implementation selected by an EVM execution specification.
/// </summary>
public interface IPrecompile
{
    /// <summary>
    /// Executes the precompile for the supplied message call.
    /// </summary>
    /// <param name="call">The message-call context and input.</param>
    /// <returns>The raw EVM call result.</returns>
    public ValueTask<TxCallResult> ExecuteAsync(PrecompileCall call);
}
