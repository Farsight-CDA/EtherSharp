namespace EtherSharp.Interpreter.Runtime;

/// <summary>
/// Identifies a failed call or creation entry check that preserves the allocated execution gas.
/// </summary>
public enum CallEntryFailureReason
{
    /// <summary>The requested call or creation exceeds the call depth limit.</summary>
    DepthExceeded,
    /// <summary>The funding account cannot cover the supplied value.</summary>
    InsufficientBalance,
    /// <summary>The creator's nonce cannot be incremented.</summary>
    CreatorNonceOverflow
}
