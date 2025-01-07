using EtherSharp.Tx.Types;

namespace EtherSharp.Tx.EIP1559;
public record EIP1559TxParams(
    StateAccess[] AccessList
) : ITxParams<EIP1559TxParams>
{
    public static EIP1559TxParams Default { get; } = new EIP1559TxParams([]);
}
