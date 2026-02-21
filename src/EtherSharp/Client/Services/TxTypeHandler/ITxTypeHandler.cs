using EtherSharp.Tx;
using EtherSharp.Tx.Types;

namespace EtherSharp.Client.Services.TxTypeHandler;

/// <summary>
/// Encodes transaction inputs into signed raw transaction payloads for a specific transaction type.
/// </summary>
/// <typeparam name="TTransaction">The transaction model type handled by this encoder.</typeparam>
/// <typeparam name="TTxParams">The transaction parameter type required by <typeparamref name="TTransaction"/>.</typeparam>
/// <typeparam name="TTxGasParams">The gas parameter type required by <typeparamref name="TTransaction"/>.</typeparam>
public interface ITxTypeHandler<TTransaction, TTxParams, TTxGasParams>
    where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
    where TTxParams : ITxParams<TTxParams>
    where TTxGasParams : ITxGasParams
{
    /// <summary>
    /// Encodes a transaction with nonce and parameters into the raw signed transaction byte payload.
    /// </summary>
    /// <param name="txInput">The transaction input payload to encode.</param>
    /// <param name="txParams">The transaction parameters used for signing and serialization.</param>
    /// <param name="txGasParams">The gas parameters used for fee-related transaction fields.</param>
    /// <param name="nonce">The nonce to embed in the encoded transaction.</param>
    /// <param name="txHash">When this method returns, contains the computed transaction hash.</param>
    /// <returns>The encoded signed transaction represented as a hexadecimal string.</returns>
    public string EncodeTxToBytes(ITxInput txInput, TTxParams txParams, TTxGasParams txGasParams, uint nonce, out string txHash);
}
