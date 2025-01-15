using System.Numerics;

namespace EtherSharp.Types;
public record FeeHistory(
    ulong OldestBlockHeight,
    BigInteger[] BaseFeePerGas,
    double[] GasUsedRatio,
    BigInteger[][] Reward
);