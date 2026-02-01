namespace EtherSharp.Client.Services;

internal interface IInitializableService
{
    public ValueTask InitializeAsync(ulong chainId, CancellationToken cancellationToken = default);
}
