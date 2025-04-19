using EtherSharp.Tx.Types;
using System.Reflection.Emit;
using System.Text;
using System.Text.Json;

namespace EtherSharp.Tx.EIP1559;
public record EIP1559TxParams(
    StateAccess[] AccessList
) : ITxParams<EIP1559TxParams>
{
    public static EIP1559TxParams Default { get; } = new EIP1559TxParams([]);

    static EIP1559TxParams ITxParams<EIP1559TxParams>.Decode(ReadOnlySpan<byte> data)
    {
        string json = Encoding.UTF8.GetString(data);
        return JsonSerializer.Deserialize<EIP1559TxParams>(json)!;
    }

    byte[] ITxParams<EIP1559TxParams>.Encode()
    {
        string json = JsonSerializer.Serialize(this);
        return Encoding.UTF8.GetBytes(json);
    }
}
