using EtherSharp.Tx.Types;
using System.Buffers.Binary;
using System.Numerics;

namespace EtherSharp.Tx.EIP1559;
public record EIP1559GasParams(
    ulong GasLimit,
    BigInteger MaxFeePerGas,
    BigInteger MaxPriorityFeePerGas
) : ITxGasParams<EIP1559GasParams>
{
    static EIP1559GasParams ITxGasParams<EIP1559GasParams>.Decode(ReadOnlySpan<byte> data)
    {
        int maxFeePerGasBytes = data[8];
        int maxPriorityFeePerGasBytes = data[8 + 1 + maxFeePerGasBytes];

        ulong gasLimit = BinaryPrimitives.ReadUInt64BigEndian(data[0..8]);
        var maxFeePerGas = new BigInteger(data.Slice(8 + 1, maxFeePerGasBytes), true);
        var maxPriorityFeePerGas = new BigInteger(data.Slice(8 + 1 + maxFeePerGasBytes + 1, maxPriorityFeePerGasBytes), true);

        return new EIP1559GasParams(gasLimit, maxFeePerGas, maxPriorityFeePerGas);
    }

    byte[] ITxGasParams<EIP1559GasParams>.Encode()
    {
        int maxFeePerGasBytes = MaxFeePerGas.GetByteCount(true);
        int maxPriorityFeePerGasBytes = MaxPriorityFeePerGas.GetByteCount(true);

        int size = 8 + 1 + maxFeePerGasBytes + 1 + maxPriorityFeePerGasBytes;

        byte[] buffer = new byte[size];

        BinaryPrimitives.WriteUInt64BigEndian(buffer.AsSpan(0, 8), GasLimit);
        buffer[8] = (byte) maxFeePerGasBytes;
        MaxFeePerGas.TryWriteBytes(buffer.AsSpan(8 + 1, maxFeePerGasBytes), out _, true);
        buffer[8 + 1 + maxFeePerGasBytes] = (byte) maxPriorityFeePerGasBytes;
        MaxPriorityFeePerGas.TryWriteBytes(buffer.AsSpan(8 + 1 + maxFeePerGasBytes + 1, maxPriorityFeePerGasBytes), out _, true);

        return buffer;
    }

    EIP1559GasParams ITxGasParams<EIP1559GasParams>.IncrementByFactor(BigInteger multiplier, BigInteger divider, BigInteger minimumIncrement)
    {
        if(multiplier < divider)
        {
            throw new ArgumentException("Multiplier must be larger than divider");
        }

        var newMaxFeePerGas = MaxFeePerGas;
        newMaxFeePerGas *= multiplier;
        newMaxFeePerGas /= divider;

        if(newMaxFeePerGas == MaxFeePerGas)
        {
            newMaxFeePerGas += minimumIncrement;
        }

        var newMaxPriorityFeePerGas = MaxPriorityFeePerGas;
        newMaxPriorityFeePerGas *= multiplier;
        newMaxPriorityFeePerGas /= divider;

        if(newMaxPriorityFeePerGas == MaxPriorityFeePerGas)
        {
            newMaxPriorityFeePerGas += minimumIncrement;
        }

        return new EIP1559GasParams(
            GasLimit,
            newMaxFeePerGas,
            newMaxPriorityFeePerGas
        );
    }
}
