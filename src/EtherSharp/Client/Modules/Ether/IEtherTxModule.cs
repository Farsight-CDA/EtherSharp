using EtherSharp.Contract;
using EtherSharp.Numerics;
using EtherSharp.Tx;
using EtherSharp.Types;

namespace EtherSharp.Client.Modules.Ether;
/// <summary>
/// Module used to interact with the native currency.
/// </summary>
public interface IEtherTxModule : IEtherModule
{
    /// <summary>
    /// Fetches the native balance of the wallet attached to the client.
    /// </summary>
    /// <param name="targetHeight"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<UInt256> GetMyBalanceAsync(TargetBlockNumber targetHeight = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a tx input for transferring native currency to the given receiver.
    /// </summary>
    /// <param name="receiver"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public ITxInput Transfer(Address receiver, UInt256 amount);
    /// <summary>
    /// Creates a tx input for transferring native currency to the given contract.
    /// </summary>
    /// <param name="contract"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public ITxInput Transfer(IPayableContract contract, UInt256 amount);
}
