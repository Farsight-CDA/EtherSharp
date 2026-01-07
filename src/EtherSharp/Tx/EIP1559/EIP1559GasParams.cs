using EtherSharp.Numerics;
using EtherSharp.Tx.Types;
using System.Buffers.Binary;

namespace EtherSharp.Tx.EIP1559;

public record EIP1559GasParams(
    ulong GasLimit,
    UInt256 MaxFeePerGas,
    UInt256 MaxPriorityFeePerGas
) : ITxGasParams<EIP1559GasParams>
{
    static EIP1559GasParams ITxGasParams<EIP1559GasParams>.Decode(ReadOnlySpan<byte> data)
    {
        ulong gasLimit = BinaryPrimitives.ReadUInt64BigEndian(data[0..8]);
        var maxFeePerGas = BinaryPrimitives.ReadUInt256BigEndian(data[8..40]);
        var maxPriorityFeePerGas = BinaryPrimitives.ReadUInt256BigEndian(data[40..72]);

        return new EIP1559GasParams(gasLimit, maxFeePerGas, maxPriorityFeePerGas);
    }

    byte[] ITxGasParams<EIP1559GasParams>.Encode()
    {
        int size = 8 + 32 + 32;

        byte[] arr = new byte[size];
        var buffer = arr.AsSpan();

        BinaryPrimitives.WriteUInt64BigEndian(buffer[0..8], GasLimit);
        BinaryPrimitives.WriteUInt256BigEndian(buffer[8..40], MaxFeePerGas);
        BinaryPrimitives.WriteUInt256BigEndian(buffer[40..72], MaxPriorityFeePerGas);

        return arr;
    }

    EIP1559GasParams ITxGasParams<EIP1559GasParams>.IncrementByFactor(UInt256 multiplier, UInt256 divider, UInt256 minimumIncrement)
    {
        if(multiplier < divider)
        {
            throw new ArgumentException("Multiplier must be larger than divider");
        }
        //
        return new EIP1559GasParams(
            GasLimit,
            UInt256.Max(MaxFeePerGas + minimumIncrement, MaxFeePerGas * multiplier / divider),
            UInt256.Max(MaxPriorityFeePerGas + minimumIncrement, MaxPriorityFeePerGas * multiplier / divider)
        );
    }
}
