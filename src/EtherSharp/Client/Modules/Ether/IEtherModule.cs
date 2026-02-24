using EtherSharp.Contract;
using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.Client.Modules.Ether;

/// <summary>
/// Provides read-only operations for the chain native currency (for example ETH).
/// </summary>
public interface IEtherModule
{
    /// <summary>
    /// Gets the native currency balance for a wallet address.
    /// </summary>
    /// <param name="address">Address to query.</param>
    /// <param name="targetHeight">Block to evaluate the balance at. Uses the latest block by default.</param>
    /// <param name="cancellationToken">Token used to cancel the RPC request.</param>
    /// <returns>Balance in wei.</returns>
    public Task<UInt256> GetBalanceAsync(Address address, TargetHeight targetHeight = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the native currency balance for a contract address.
    /// </summary>
    /// <param name="contract">Contract whose address is queried.</param>
    /// <param name="targetHeight">Block to evaluate the balance at. Uses the latest block by default.</param>
    /// <param name="cancellationToken">Token used to cancel the RPC request.</param>
    /// <returns>Balance in wei.</returns>
    public Task<UInt256> GetBalanceAsync(IEVMContract contract, TargetHeight targetHeight = default, CancellationToken cancellationToken = default);
}
