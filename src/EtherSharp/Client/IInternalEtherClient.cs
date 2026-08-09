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
}
