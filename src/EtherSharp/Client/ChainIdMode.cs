namespace EtherSharp.Client;

/// <summary>
/// Controls how a preconfigured chain ID is handled during client initialization.
/// </summary>
public enum ChainIdMode
{
    /// <summary>
    /// Queries the chain ID from the RPC node during initialization and throws if it does not match the configured value.
    /// </summary>
    Verify,
    /// <summary>
    /// Uses the configured chain ID directly without querying the RPC node during initialization.
    /// </summary>
    Assume,
}
