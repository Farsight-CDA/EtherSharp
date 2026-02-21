namespace EtherSharp.Client.Services.TxPublisher;

/// <summary>
/// Publishes signed transaction payloads to an Ethereum-compatible network.
/// </summary>
public interface ITxPublisher
{
    /// <summary>
    /// Submits a signed transaction to the network and returns submission details.
    /// </summary>
    /// <param name="transactionHex">The raw signed transaction encoded as a hexadecimal string.</param>
    /// <param name="cancellationToken">A token used to cancel the publish request.</param>
    /// <returns>The submission result including status and transaction hash information.</returns>
    public Task<TxSubmissionResult> PublishTxAsync(string transactionHex, CancellationToken cancellationToken);
}
