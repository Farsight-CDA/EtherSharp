using EtherSharp.Types;

namespace EtherSharp.Client.Services;

internal interface IInitializableService
{
    public ValueTask InitializeAsync(ulong chainId, CompatibilityReport compatibilityReport, CancellationToken cancellationToken = default);
}
