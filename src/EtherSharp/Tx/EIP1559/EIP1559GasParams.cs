using EtherSharp.Tx.Types;
using System.Numerics;
using System.Text.Json;
using System.Text;

namespace EtherSharp.Tx.EIP1559;
public record EIP1559GasParams(
    ulong GasLimit,
    BigInteger MaxFeePerGas,
    BigInteger MaxPriorityFeePerGas
) : ITxGasParams<EIP1559GasParams>
{
    static EIP1559GasParams ITxGasParams<EIP1559GasParams>.Decode(ReadOnlySpan<byte> data)
    {
        string json = Encoding.UTF8.GetString(data);
        return JsonSerializer.Deserialize<EIP1559GasParams>(json)!;
    }

    byte[] ITxGasParams<EIP1559GasParams>.Encode()
    {
        string json = JsonSerializer.Serialize(this);
        return Encoding.UTF8.GetBytes(json);
    }
}
