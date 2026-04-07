namespace EtherSharp.Client;

/// <summary>
/// Immutable configuration describing how an Ether client instance should behave.
/// </summary>
public sealed record EtherClientOptions
{
    /// <summary>
    /// Gets whether the client was built with transaction capabilities enabled.
    /// </summary>
    public bool IsTxClient { get; init; }

    /// <summary>
    /// Gets the optional fallback gas limit applied to <c>eth_call</c> requests.
    /// </summary>
    public ulong? EthCallGasLimit { get; init; }

    /// <summary>
    /// Gets the optional fallback gas limit applied to flash-call helper execution.
    /// </summary>
    public ulong? FlashCallGasLimit { get; init; }
}
