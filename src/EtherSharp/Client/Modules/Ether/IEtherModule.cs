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
    /// <param name="blockNumber"></param>
    /// <returns></returns>
    public Task<BigInteger> GetBalanceAsync(Address address, TargetBlockNumber blockNumber = default);
    /// <summary>
    /// Fetches the native balance of the given contract.
    /// </summary>
    /// <param name="contract"></param>
    /// <param name="blockNumber"></param>
    /// <returns></returns>
    public Task<BigInteger> GetBalanceAsync(IEVMContract contract, TargetBlockNumber blockNumber = default);
}
