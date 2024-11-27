using EtherSharp.Tx.Types;
using System.Numerics;

namespace EtherSharp.Tx.EIP1559;
public record EIP1559TxParams(
    ulong Gas,
    BigInteger MaxFeePerGas,
    BigInteger MaxPriorityFeePerGas,
    StateAccess[] AccessList
) : ITxParams;
