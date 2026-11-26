using EtherSharp.Types;

namespace EtherSharp.Tx.Watcher;

/// <summary>
/// Reads the current nonces of transaction senders.
/// </summary>
public closed class NonceSource
{
    /// <summary>
    /// A sender whose nonce should be read, with the lowest nonce of interest.
    /// </summary>
    public readonly record struct Request(Address Sender, ulong StartNonce);

    /// <summary>
    /// The nonces of the requested senders at one block, in request order.
    /// </summary>
    public readonly record struct Snapshot(ulong BlockNumber, ulong[] Nonces);

    /// <summary>
    /// Reads the nonces of all requested senders at the latest block.
    /// </summary>
    public abstract Task<Snapshot> ReadAsync(ReadOnlyMemory<Request> requests, CancellationToken cancellationToken);
}
