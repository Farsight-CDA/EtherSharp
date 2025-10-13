using EtherSharp.Tx.Types;
using System.Buffers.Binary;
using System.Numerics;

namespace EtherSharp.Tx.Legacy;

public record LegacyGasParams(
    ulong GasLimit,
    BigInteger GasPrice
) : ITxGasParams<LegacyGasParams>
{
    public byte[] Encode()
    {
        int gasPriceBytes = GasPrice.GetByteCount(true);
        int size = 8 + gasPriceBytes;

        byte[] buffer = new byte[size];

        BinaryPrimitives.WriteUInt64BigEndian(buffer.AsSpan(0, 8), GasLimit);
        GasPrice.TryWriteBytes(buffer.AsSpan(8, gasPriceBytes), out _, true, true);

        return buffer;
    }
    public static LegacyGasParams Decode(ReadOnlySpan<byte> data)
        => new LegacyGasParams(
            BinaryPrimitives.ReadUInt64BigEndian(data[..8]),
            new BigInteger(data[8..], true, true)
        );

    public LegacyGasParams IncrementByFactor(BigInteger multiplier, BigInteger divider, BigInteger minimumIncrement)
    {
        if(multiplier < divider)
        {
            throw new ArgumentException("Multiplier must be larger than divider");
        }
        //
        return new LegacyGasParams(
            GasLimit,
            BigInteger.Max(GasPrice + minimumIncrement, GasPrice * multiplier / divider)
        );
    }
}
