using EtherSharp.Numerics;
using EtherSharp.Tx.Types;
using System.Buffers.Binary;

namespace EtherSharp.Tx.Legacy;

public record LegacyGasParams(
    ulong GasLimit,
    UInt256 GasPrice
) : ITxGasParams<LegacyGasParams>
{
    public byte[] Encode()
    {
        int size = 8 + 32;
        byte[] arr = new byte[size];
        var buffer = arr.AsSpan();

        BinaryPrimitives.WriteUInt64BigEndian(buffer[0..8], GasLimit);
        BinaryPrimitives.WriteUInt256BigEndian(buffer[8..40], GasPrice);

        return arr;
    }
    public static LegacyGasParams Decode(ReadOnlySpan<byte> data)
        => new LegacyGasParams(
            BinaryPrimitives.ReadUInt64BigEndian(data[0..8]),
            BinaryPrimitives.ReadUInt256BigEndian(data[8..40])
        );

    public LegacyGasParams IncrementByFactor(UInt256 multiplier, UInt256 divider, UInt256 minimumIncrement)
    {
        if(multiplier < divider)
        {
            throw new ArgumentException("Multiplier must be larger than divider");
        }
        //
        return new LegacyGasParams(
            GasLimit,
            UInt256.Max(GasPrice + minimumIncrement, GasPrice * multiplier / divider)
        );
    }
}
