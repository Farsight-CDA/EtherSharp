using Microsoft.Extensions.DependencyInjection;

namespace EtherSharp.Client;

/// <summary>
/// Internal EtherClientBuilder Interface.
/// </summary>
public interface IInternalEtherClientBuilder
{
    /// <summary>
    /// The underlying service collection of the client builder.
    /// </summary>
    public IServiceCollection Services { get; }
}
