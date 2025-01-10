using System.Numerics;

namespace EtherSharp.Client.Services.RPC;
public record FeeHistory(
    ulong OldestBlockHeight,
    BigInteger[] BaseFeePerGas,
    double[] GasUsedRatio,
    BigInteger[][] Reward
);