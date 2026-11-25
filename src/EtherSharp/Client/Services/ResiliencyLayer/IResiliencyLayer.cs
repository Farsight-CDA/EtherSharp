namespace EtherSharp.Client.Services.ResiliencyLayer;

/// <summary>
/// Persists submitted transaction metadata so pending flows can be recovered after process restarts.
/// </summary>
public interface IResiliencyLayer
{
    /// <summary>
    /// Stores a submission attempt, identified by its transaction ID and sequence.
    /// </summary>
    /// <param name="txSubmission">The submission record to persist.</param>
    /// <param name="cancellationToken">A token used to cancel the persistence operation.</param>
    /// <returns>A task that completes when the submission has been stored.</returns>
    public Task StoreTxSubmissionAsync(TxSubmissionStorage txSubmission, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes all stored submission attempts for the specified transaction ID.
    /// </summary>
    /// <param name="id">The transaction ID whose submissions should be removed.</param>
    /// <param name="cancellationToken">A token used to cancel the delete operation.</param>
    /// <returns>A task that completes when matching submissions have been deleted.</returns>
    public Task DeleteTxSubmissionsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the highest nonce that has been persisted, for sequential nonce allocation on restart.
    /// </summary>
    /// <param name="cancellationToken">A token used to cancel the read operation.</param>
    /// <returns>The highest persisted nonce.</returns>
    public Task<ulong?> GetLastSubmittedNonceAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all persisted submission attempts for the specified transaction ID.
    /// </summary>
    /// <param name="id">The transaction ID whose submissions should be loaded.</param>
    /// <param name="cancellationToken">A token used to cancel the read operation.</param>
    /// <returns>The submissions previously stored for the transaction, or an empty list if none exist.</returns>
    public Task<IReadOnlyList<TxSubmissionStorage>> FetchTxSubmissionsAsync(Guid id, CancellationToken cancellationToken = default);
}
