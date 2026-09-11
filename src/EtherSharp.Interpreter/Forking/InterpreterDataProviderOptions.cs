namespace EtherSharp.Interpreter.Forking;

/// <summary>Configures upstream data provider creation by the pre/post-block client helpers.</summary>
public readonly record struct InterpreterDataProviderOptions
{
    /// <summary>
    /// The number of subsequent storage slots to fetch alongside each requested slot, from zero to 255.
    /// Defaults to zero (disabled).
    /// </summary>
    public byte ForwardPrefetchDistance { get; init; }
}
