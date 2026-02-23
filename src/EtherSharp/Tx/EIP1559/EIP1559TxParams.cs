using EtherSharp.Tx.Types;
using System.Text;
using System.Text.Json;

namespace EtherSharp.Tx.EIP1559;

/// <summary>
/// Defines additional transaction parameters for EIP-1559 transactions.
/// </summary>
/// <param name="AccessList">Optional access list used to predeclare state slots touched by the transaction.</param>
public record EIP1559TxParams(
    StateAccess[] AccessList
) : ITxParams<EIP1559TxParams>
{
    /// <inheritdoc/>
    public static EIP1559TxParams Default { get; } = new EIP1559TxParams([]);

    /// <inheritdoc/>
    static EIP1559TxParams ITxParams<EIP1559TxParams>.Decode(ReadOnlySpan<byte> data)
    {
        string json = Encoding.UTF8.GetString(data);
        return JsonSerializer.Deserialize<EIP1559TxParams>(json)!;
    }

    /// <inheritdoc/>
    byte[] ITxParams<EIP1559TxParams>.Encode()
    {
        string json = JsonSerializer.Serialize(this);
        return Encoding.UTF8.GetBytes(json);
    }
}
