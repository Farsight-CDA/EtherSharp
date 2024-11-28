using EtherSharp.Tx.Types;

namespace EtherSharp.Tx.EIP1559;
public record EIP1559TxParams(
    StateAccess[] AccessList
) : ITxParams;
