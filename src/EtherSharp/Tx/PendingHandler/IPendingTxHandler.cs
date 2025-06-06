﻿using EtherSharp.Tx.Types;

namespace EtherSharp.Tx.PendingHandler;

/// <summary>
/// Represents a handler class to manage a pending transaction with a given nonce.
/// </summary>
/// <typeparam name="TTxParams"></typeparam>
/// <typeparam name="TTxGasParams"></typeparam>
public interface IPendingTxHandler<TTxParams, TTxGasParams>
    where TTxParams : class, ITxParams<TTxParams>
    where TTxGasParams : class, ITxGasParams<TTxGasParams>
{
    /// <summary>
    /// Nonce of the pending transaction managed by this handler.
    /// </summary>
    public uint Nonce { get; }

    /// <summary>
    /// Collection of parameters that the transaction with this nonce was submitted with.
    /// </summary>
    public IReadOnlyList<TxSubmission<TTxParams, TTxGasParams>> TxSubmissions { get; }

    /// <summary>
    /// Publishes the most recent tx submission to the mempool and waits for it to be confirmed on the blockchain.
    /// Provides a callback to handle errors and timeouts.
    /// </summary>
    /// <param name="onError"></param>
    /// <returns></returns>
    public Task<TxConfirmationResult> PublishAndConfirmAsync(
        Func<
            TxConfirmationError,
            TxConfirmationActionBuilder<TTxParams, TTxGasParams>,
            TxSubmission<TTxParams, TTxGasParams>,
            TxConfirmationAction<TTxParams, TTxGasParams>
        > onError
    );
}
