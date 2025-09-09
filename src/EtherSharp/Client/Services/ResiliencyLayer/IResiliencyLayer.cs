namespace EtherSharp.Client.Services.ResiliencyLayer;

/// <summary>
/// Represents a layer for storing transaction parameters for continuation after a restart.
/// </summary>
public interface IResiliencyLayer
{
    /// <summary>
    /// Stores the given tx parameters.
    /// </summary>
    /// <param name="txSubmission"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task StoreTxSubmissionAsync(TxSubmissionStorage txSubmission, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes all tx parameters for the given nonce.
    /// </summary>
    /// <param name="nonce"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task DeleteTxSubmissionsForNonceAsync(uint nonce, CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches the highest nonce that was stored previously.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<uint> GetLastSubmittedNonceAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all tx parameters with the given nonce.
    /// </summary>
    /// <param name="nonce"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<IReadOnlyList<TxSubmissionStorage>> FetchTxSubmissionsAsync(uint nonce, CancellationToken cancellationToken = default);
}
