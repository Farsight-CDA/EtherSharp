using EtherSharp.Numerics;

namespace EtherSharp.Types;

/// <summary>
/// Snapshot returned by <c>eth_feeHistory</c> for recent block fee dynamics.
/// </summary>
/// <param name="OldestBlockHeight">Height of the earliest block included in this window.</param>
/// <param name="BaseFeePerGas">Base fee values for the window, including the next block's projected base fee.</param>
/// <param name="GasUsedRatio">Per-block gas usage ratios normalized to each block gas limit.</param>
/// <param name="Reward">Per-block priority fee samples ordered by the requested reward percentiles.</param>
public record FeeHistory(
    ulong OldestBlockHeight,
    UInt256[] BaseFeePerGas,
    double[] GasUsedRatio,
    UInt256[][] Reward
);
