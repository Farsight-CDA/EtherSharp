using EtherSharp.Client.Services;
using EtherSharp.Client.Services.TxTypeHandler;
using EtherSharp.RPC.Transport;
using EtherSharp.Wallet;

namespace EtherSharp.Tx.Legacy;

/// <summary>
/// Encodes and signs legacy Ethereum transactions.
/// </summary>
/// <param name="signer">Signer used to produce recoverable transaction signatures.</param>
public sealed class LegacyTxTypeHandler(IEtherSigner signer)
    : IInitializableService, ITxTypeHandler<LegacyTransaction, LegacyTxParams, LegacyGasParams>
{
    private readonly IEtherSigner _signer = signer;

    private bool _isInitialized;
    private ulong _chainId;

    /// <inheritdoc/>
    public ValueTask InitializeAsync(
        ulong chainId, RpcRequestOptions _ = default, CancellationToken cancellationToken = default)
    {
        _chainId = chainId;
        _isInitialized = true;
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    public ValueTask<SignedTransaction> EncodeTxAsync(
        ITxInput txInput,
        LegacyTxParams txParams,
        LegacyGasParams txGasParams,
        ulong nonce,
        CancellationToken cancellationToken = default
    ) => !_isInitialized
        ? throw new InvalidOperationException("Not initialized")
        : new LegacyTransaction(
            _chainId,
            txGasParams.GasLimit,
            nonce,
            txInput,
            txGasParams.GasPrice
        ).SignAsync(_signer, cancellationToken);
}
