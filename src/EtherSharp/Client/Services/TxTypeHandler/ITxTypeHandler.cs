using EtherSharp.Tx;
using EtherSharp.Tx.Types;
using EtherSharp.Types;

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
    /// Encodes and signs a transaction with the supplied nonce and parameters.
    /// </summary>
    /// <param name="txInput">The transaction input payload to encode.</param>
    /// <param name="txParams">The transaction parameters used for signing and serialization.</param>
    /// <param name="txGasParams">The gas parameters used for fee-related transaction fields.</param>
    /// <param name="nonce">The nonce to embed in the encoded transaction.</param>
    /// <param name="cancellationToken">Token used to cancel the signing operation.</param>
    /// <returns>The encoded signed transaction and its hash.</returns>
    public ValueTask<SignedTransaction> EncodeTxAsync(
        ITxInput txInput,
        TTxParams txParams,
        TTxGasParams txGasParams,
        uint nonce,
        CancellationToken cancellationToken = default);
}
