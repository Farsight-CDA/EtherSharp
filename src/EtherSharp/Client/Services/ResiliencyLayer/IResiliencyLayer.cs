namespace EtherSharp.Client.Services.ResiliencyLayer;

/// <summary>
/// Persists submitted transaction metadata so pending flows can be recovered after process restarts.
/// </summary>
public interface IResiliencyLayer
{
    /// <summary>
    /// Stores a submitted transaction record.
    /// </summary>
    /// <param name="txSubmission">The submission record to persist.</param>
    /// <param name="cancellationToken">A token used to cancel the persistence operation.</param>
    /// <returns>A task that completes when the submission has been stored.</returns>
    public Task StoreTxSubmissionAsync(TxSubmissionStorage txSubmission, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes all stored submissions associated with the specified nonce.
    /// </summary>
    /// <param name="nonce">The nonce whose submissions should be removed.</param>
    /// <param name="cancellationToken">A token used to cancel the delete operation.</param>
    /// <returns>A task that completes when matching submissions have been deleted.</returns>
    public Task DeleteTxSubmissionsForNonceAsync(uint nonce, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the highest nonce that has been persisted.
    /// </summary>
    /// <param name="cancellationToken">A token used to cancel the read operation.</param>
    /// <returns>The highest persisted nonce.</returns>
    public Task<uint> GetLastSubmittedNonceAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all persisted submissions for the specified nonce.
    /// </summary>
    /// <param name="nonce">The nonce whose submissions should be loaded.</param>
    /// <param name="cancellationToken">A token used to cancel the read operation.</param>
    /// <returns>The submissions previously stored for the nonce.</returns>
    public Task<IReadOnlyList<TxSubmissionStorage>> FetchTxSubmissionsAsync(uint nonce, CancellationToken cancellationToken = default);
}
