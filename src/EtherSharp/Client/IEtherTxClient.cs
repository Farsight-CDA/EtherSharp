namespace EtherSharp.Client;

/// <summary>
/// Signer-bound Ethereum client abstraction extending <see cref="IEtherClient"/>.
/// </summary>
/// <remarks>
/// Disposing only releases signer-scoped resources, not the underlying <see cref="IEtherClient"/>.
/// Disposing the underlying client disposes all attached tx clients.
/// </remarks>
public interface IEtherTxClient : IEtherClient;
