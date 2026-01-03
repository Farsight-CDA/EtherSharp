using EtherSharp.Tx;
using EtherSharp.Types;

namespace EtherSharp.Client.Modules.FlashCall;

/// <summary>
/// Interface for flash call executor strategies.
/// </summary>
public interface IFlashCallExecutor
{
    /// <summary>
    /// Gets the maximum amount of bytes that can be sent as a payload at a given target height.
    /// </summary>
    public int GetMaxPayloadSize(TargetBlockNumber targetHeight);

    /// <summary>
    /// Executes a flash call with the given payload.
    /// </summary>
    /// <param name="deployment"></param>
    /// <param name="call"></param>
    /// <param name="targetHeight"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<TxCallResult> ExecuteFlashCallAsync(
        IContractDeployment deployment,
        IContractCall call,
        TargetBlockNumber targetHeight,
        CancellationToken cancellationToken
    );
}
