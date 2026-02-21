using EtherSharp.Contract;
using EtherSharp.Numerics;
using EtherSharp.Tx;
using EtherSharp.Types;

namespace EtherSharp.Client.Modules.Ether;
/// <summary>
/// Extends <see cref="IEtherModule"/> with transaction-oriented native currency helpers.
/// </summary>
public interface IEtherTxModule : IEtherModule
{
    /// <summary>
    /// Gets the native currency balance of the signer configured on the client.
    /// </summary>
    /// <param name="targetHeight">Block to evaluate the balance at. Uses the latest block by default.</param>
    /// <param name="cancellationToken">Token used to cancel the RPC request.</param>
    /// <returns>Balance in wei.</returns>
    public Task<UInt256> GetMyBalanceAsync(TargetBlockNumber targetHeight = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a transaction input that transfers native currency to an address.
    /// </summary>
    /// <param name="receiver">Destination address.</param>
    /// <param name="amount">Transfer amount in wei.</param>
    /// <returns>An unsigned transaction input ready to estimate or send.</returns>
    public ITxInput Transfer(Address receiver, UInt256 amount);

    /// <summary>
    /// Creates a transaction input that transfers native currency to a payable contract.
    /// </summary>
    /// <param name="contract">Destination payable contract.</param>
    /// <param name="amount">Transfer amount in wei.</param>
    /// <returns>An unsigned transaction input ready to estimate or send.</returns>
    public ITxInput Transfer(IPayableContract contract, UInt256 amount);
}
