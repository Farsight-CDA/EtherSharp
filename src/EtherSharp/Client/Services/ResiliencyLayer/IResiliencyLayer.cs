namespace EtherSharp.Client.Services.ResiliencyLayer;

/// <summary>
/// Persists submitted transaction metadata so pending flows can be recovered after process restarts.
/// </summary>
public interface IResiliencyLayer
{
    /// <summary>
    /// Stores a submission attempt, identified by its chain ID, sender, transaction ID, and sequence.
    /// </summary>
    /// <param name="txSubmission">The submission record to persist.</param>
    /// <param name="cancellationToken">A token used to cancel the persistence operation.</param>
    /// <returns>A task that completes when the submission has been stored.</returns>
    public Task StoreTxSubmissionAsync(TxSubmissionStorage txSubmission, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes all stored submission attempts for the specified sender and transaction ID.
    /// </summary>
    /// <param name="senderKey">The chain and sender to delete submissions for.</param>
    /// <param name="id">The transaction ID whose submissions should be removed.</param>
    /// <param name="cancellationToken">A token used to cancel the delete operation.</param>
    /// <returns>A task that completes when matching submissions have been deleted.</returns>
    public Task DeleteTxSubmissionsAsync(TxSenderKey senderKey, Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the highest persisted nonce for the specified sender, for sequential nonce allocation on restart.
    /// </summary>
    /// <param name="senderKey">The chain and sender to look up.</param>
    /// <param name="cancellationToken">A token used to cancel the read operation.</param>
    /// <returns>The highest persisted nonce, or <see langword="null"/> if none exists.</returns>
    public Task<ulong?> GetLastSubmittedNonceAsync(TxSenderKey senderKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all persisted submission attempts for the specified sender and transaction ID.
    /// </summary>
    /// <param name="senderKey">The chain and sender to load submissions for.</param>
    /// <param name="id">The transaction ID whose submissions should be loaded.</param>
    /// <param name="cancellationToken">A token used to cancel the read operation.</param>
    /// <returns>The submissions previously stored for the transaction, or an empty list if none exist.</returns>
    public Task<IReadOnlyList<TxSubmissionStorage>> FetchTxSubmissionsAsync(
        TxSenderKey senderKey, Guid id, CancellationToken cancellationToken = default
    );
}
