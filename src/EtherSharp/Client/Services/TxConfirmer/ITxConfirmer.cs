namespace EtherSharp.Client.Services.TxConfirmer;
public interface ITxConfirmer
{
    public Task<TxConfirmationResult> WaitForTxConfirmationAsync(string txHash, TimeSpan timeoutDuration, CancellationToken cancellationToken = default);
}
