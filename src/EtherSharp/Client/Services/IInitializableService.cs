namespace EtherSharp.Client.Services;

/// <summary>
/// Defines a service lifecycle hook that runs once the client has resolved the active chain ID.
/// </summary>
public interface IInitializableService
{
    /// <summary>
    /// Initializes the service with the chain ID used by the client.
    /// </summary>
    public ValueTask InitializeAsync(ulong chainId, CancellationToken cancellationToken = default);
}
