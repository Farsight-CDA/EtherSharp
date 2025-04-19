namespace EtherSharp.Client.Services.ResiliencyLayer;

/// <summary>
/// Represents a layer for storing transaction parameters for continuation after a restart.
/// </summary>
public interface IResiliencyLayer
{
    /// <summary>
    /// Stores the given tx parameters.
    /// </summary>
    /// <param name="nonce"></param>
    /// <param name="txSubmission"></param>
    /// <returns></returns>
    public Task StoreTxSubmissionAsync(uint nonce, TxSubmission txSubmission);

    /// <summary>
    /// Fetches the highest nonce that was stored previously.
    /// </summary>
    /// <returns></returns>
    public Task<uint> GetLastSubmittedNonceAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all tx parameters with the given nonce.
    /// </summary>
    /// <param name="nonce"></param>
    /// <returns></returns>
    public Task<IReadOnlyList<TxSubmission>> FetchTxSubmissionsAsync(uint nonce);
}
