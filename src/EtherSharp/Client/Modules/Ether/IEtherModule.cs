using EtherSharp.Contract;
using EtherSharp.Types;
using System.Numerics;

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
    public Task<BigInteger> GetBalanceAsync(Address address, TargetBlockNumber targetHeight = default, CancellationToken cancellationToken = default);
    /// <summary>
    /// Fetches the native balance of the given contract.
    /// </summary>
    /// <param name="contract"></param>
    /// <param name="targetHeight"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<BigInteger> GetBalanceAsync(IEVMContract contract, TargetBlockNumber targetHeight = default, CancellationToken cancellationToken = default);
}
