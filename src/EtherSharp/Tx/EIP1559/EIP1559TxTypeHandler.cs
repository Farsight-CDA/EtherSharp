using EtherSharp.Client.Services;
using EtherSharp.Client.Services.TxTypeHandler;
using EtherSharp.RPC.Transport;
using EtherSharp.Wallet;

namespace EtherSharp.Tx.EIP1559;

/// <summary>
/// Encodes and signs EIP-1559 transactions.
/// </summary>
/// <param name="signer">Signer used to produce recoverable transaction signatures.</param>
public sealed class EIP1559TxTypeHandler(IEtherSigner signer)
    : IInitializableService, ITxTypeHandler<EIP1559Transaction, EIP1559TxParams, EIP1559GasParams>
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
        EIP1559TxParams txParams,
        EIP1559GasParams txGasParams,
        ulong nonce,
        CancellationToken cancellationToken = default
    ) => !_isInitialized
        ? throw new InvalidOperationException("Not initialized")
        : new EIP1559Transaction(
            _chainId,
            txGasParams.GasLimit,
            nonce,
            txInput,
            txGasParams.MaxFeePerGas,
            txGasParams.MaxPriorityFeePerGas,
            txParams.AccessList
        ).SignAsync(_signer, cancellationToken);
}
