using EtherSharp.Contract;
using EtherSharp.Numerics;
using EtherSharp.RPC.Transport;
using EtherSharp.Tx;
using EtherSharp.Types;

namespace EtherSharp.Client.Modules.Ether;

/// <summary>
/// Provides balance queries and unsigned transfer inputs for the chain native currency (for example ETH).
/// </summary>
public interface IEtherModule
{
    /// <summary>
    /// Creates a transaction input that transfers native currency to an address.
    /// </summary>
    /// <param name="receiver">Destination address.</param>
    /// <param name="amount">Transfer amount in wei.</param>
    /// <returns>An unsigned transaction input ready to estimate or send.</returns>
    public ITxInput Transfer(in Address receiver, UInt256 amount);

    /// <summary>
    /// Creates a transaction input that transfers native currency to a payable contract.
    /// </summary>
    /// <param name="contract">Destination payable contract.</param>
    /// <param name="amount">Transfer amount in wei.</param>
    /// <returns>An unsigned transaction input ready to estimate or send.</returns>
    public ITxInput Transfer(IPayableContract contract, UInt256 amount);

    /// <summary>
    /// Gets the native currency balance for a wallet address.
    /// </summary>
    /// <param name="address">Address to query.</param>
    /// <param name="targetHeight">Block to evaluate the balance at. Uses the latest block by default.</param>
    /// <param name="requestOptions">Options controlling the RPC request.</param>
    /// <param name="cancellationToken">Token used to cancel the RPC request.</param>
    /// <returns>Balance in wei.</returns>
    public Task<UInt256> GetBalanceAsync(in Address address, TargetHeight targetHeight = default,
        RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the native currency balance for a contract address.
    /// </summary>
    /// <param name="contract">Contract whose address is queried.</param>
    /// <param name="targetHeight">Block to evaluate the balance at. Uses the latest block by default.</param>
    /// <param name="requestOptions">Options controlling the RPC request.</param>
    /// <param name="cancellationToken">Token used to cancel the RPC request.</param>
    /// <returns>Balance in wei.</returns>
    public Task<UInt256> GetBalanceAsync(IEVMContract contract, TargetHeight targetHeight = default,
        RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default);
}
