namespace EtherSharp.Interpreter.Runtime;

/// <summary>Controls top-level sender nonce handling during simulation.</summary>
public enum TopLevelNonceHandling
{
    /// <summary>Skips nonce handling only for call simulations with a destination address.</summary>
    Default = 0,

    /// <summary>Looks up, validates, and increments the sender nonce.</summary>
    Validate,

    /// <summary>Skips sender nonce lookup, validation, and increment. Disallows top-level contract creation.</summary>
    Skip
}
