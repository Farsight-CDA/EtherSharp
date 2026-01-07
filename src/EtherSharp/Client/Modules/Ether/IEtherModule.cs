using EtherSharp.Contract;
using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.Client.Modules.Ether;

/// <summary>
/// Module used to interact with the native currency.
/// </summary>
public interface IEtherModule
{
    /// <summary>
    /// Fetches the native balance of the given address.
    /// </summary>
    /// <param name="address"></param>
    /// <param name="targetHeight"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<UInt256> GetBalanceAsync(Address address, TargetBlockNumber targetHeight = default, CancellationToken cancellationToken = default);
    /// <summary>
    /// Fetches the native balance of the given contract.
    /// </summary>
    /// <param name="contract"></param>
    /// <param name="targetHeight"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<UInt256> GetBalanceAsync(IEVMContract contract, TargetBlockNumber targetHeight = default, CancellationToken cancellationToken = default);
}
