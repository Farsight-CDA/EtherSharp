using EtherSharp.RPC;

namespace EtherSharp.Client;

/// <summary>
/// Internal EtherClient Interface.
/// </summary>
public interface IInternalEtherClient
{
    /// <summary>
    /// The underlying service provider of the EtherClient.
    /// </summary>
    public IServiceProvider Provider { get; }

    /// <summary>
    /// The underlying RPC client of the EtherClient.
    /// </summary>
    public IRpcClient RPC { get; }
}
