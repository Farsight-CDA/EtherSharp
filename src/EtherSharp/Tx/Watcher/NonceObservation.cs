namespace EtherSharp.Tx.Watcher;

/// <summary>
/// Observed nonce of a sender at a block.
/// </summary>
public readonly record struct NonceObservation(ulong Nonce, ulong BlockNumber);
