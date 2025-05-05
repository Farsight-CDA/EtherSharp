using EtherSharp.Tx.Types;

namespace EtherSharp.Tx.PendingHandler;
internal class CompletedPendingHandler<TTxParams, TTxGasParams>(
    uint nonce,
    IEnumerable<TxSubmission<TTxParams, TTxGasParams>> txSubmissions,
    TxConfirmationResult result
) : IPendingTxHandler<TTxParams, TTxGasParams>
    where TTxParams : class, ITxParams<TTxParams>
    where TTxGasParams : class, ITxGasParams<TTxGasParams>
{
    public uint Nonce { get; } = nonce;
    public IReadOnlyList<TxSubmission<TTxParams, TTxGasParams>> TxSubmissions { get; } = [.. txSubmissions];

    private readonly TxConfirmationResult _result = result;

    Task<TxConfirmationResult> IPendingTxHandler<TTxParams, TTxGasParams>.PublishAndConfirmAsync(
        Func<
            TxConfirmationError,
            TxConfirmationActionBuilder<TTxParams, TTxGasParams>,
            TxSubmission<TTxParams, TTxGasParams>,
            TxConfirmationAction<TTxParams, TTxGasParams>
        > onError
    ) => Task.FromResult(_result);
}
