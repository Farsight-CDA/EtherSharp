using EtherSharp.Numerics;

namespace EtherSharp.Types;

public record FeeHistory(
    ulong OldestBlockHeight,
    UInt256[] BaseFeePerGas,
    double[] GasUsedRatio,
    UInt256[][] Reward
);
