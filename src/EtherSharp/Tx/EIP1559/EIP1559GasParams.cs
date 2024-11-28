using EtherSharp.Tx.Types;
using System.Numerics;

namespace EtherSharp.Tx.EIP1559;
public record EIP1559GasParams(
    BigInteger MaxFeePerGas,
    BigInteger MaxPriorityFeePerGas
) : ITxGasParams;
