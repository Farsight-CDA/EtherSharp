using EtherSharp.Tx;
using EtherSharp.Types;

namespace EtherSharp.Client.Services.FlashCallExecutor;

/// <summary>
/// Executes read-only contract calls by routing them through a flash call strategy.
/// </summary>
public interface IFlashCallExecutor
{
    /// <summary>
    /// Gets the maximum call payload size, in bytes, supported at the specified target block.
    /// </summary>
    /// <param name="targetHeight">The block height context used to evaluate execution constraints.</param>
    /// <returns>The maximum payload size, in bytes, accepted by this executor.</returns>
    public int GetMaxPayloadSize(TargetBlockNumber targetHeight);

    /// <summary>
    /// Gets the maximum call result size, in bytes, that can be returned at the specified target block.
    /// </summary>
    /// <param name="targetHeight">The block height context used to evaluate execution constraints.</param>
    /// <returns>The maximum return data size, in bytes, supported by this executor.</returns>
    public int GetMaxResultSize(TargetBlockNumber targetHeight);

    /// <summary>
    /// Executes a flash call against a deployed helper contract for the provided call payload.
    /// </summary>
    /// <param name="deployment">The deployment descriptor of the helper contract used for flash execution.</param>
    /// <param name="call">The contract call payload and target metadata to execute.</param>
    /// <param name="targetHeight">The block number to execute the call against.</param>
    /// <param name="cancellationToken">A token used to cancel the underlying RPC request.</param>
    /// <returns>The execution result containing call success state and returned bytes.</returns>
    public Task<TxCallResult> ExecuteFlashCallAsync(
        IContractDeployment deployment,
        IContractCall call,
        TargetBlockNumber targetHeight,
        CancellationToken cancellationToken
    );
}
