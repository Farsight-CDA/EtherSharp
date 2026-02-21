using EtherSharp.ABI;
using EtherSharp.Numerics;
using System.Numerics;

namespace EtherSharp.Tests.ABI.Decoder;

public class AbiNumberEncodingTests
{
    public static TheoryData<int> BitSizes
        => [.. Enumerable.Range(1, 32).Select(x => x * 8)];
    public static TheoryData<int> NonNativeBitSizes
        => [.. Enumerable.Range(1, 32)
            .Select(x => x * 8)
            .Where(x => x != 8 && x != 16 && x != 32 && x != 64)];

    [Theory]
    [MemberData(nameof(BitSizes))]
    public void Should_Match_Int_MinValue_Output(int bitSize)
    {
        byte[] input = new byte[32];
        input.AsSpan()[0..(32 - (bitSize / 8))].Fill(255);
        input[^(bitSize / 8)] = 128;

        switch(bitSize)
        {
            case 8:
            {
                sbyte value = new AbiDecoder(input).Number<sbyte>(false, bitSize);
                Assert.Equal(SByte.MinValue, value);
                break;
            }
            case 16:
            {
                short value = new AbiDecoder(input).Number<short>(false, bitSize);
                Assert.Equal(Int16.MinValue, value);
                break;
            }
            case > 16 and <= 32:
            {
                int value = new AbiDecoder(input).Number<int>(false, bitSize);
                Assert.Equal(Int32.MinValue >> (32 - bitSize), value);
                break;
            }
            case > 32 and <= 64:
            {
                long value = new AbiDecoder(input).Number<long>(false, bitSize);
                Assert.Equal(Int64.MinValue >> (64 - bitSize), value);
                break;
            }
            case > 64 and <= 256:
            {
                var value = new AbiDecoder(input).Number<Int256>(false, bitSize);
                Assert.Equal(-Int256.Pow(2, bitSize - 1), value);
                break;
            }
            default:
                throw new NotSupportedException();
        }
    }
    [Theory]
    [MemberData(nameof(BitSizes))]
    public void Should_Match_Int_MaxValue_Output(int bitSize)
    {
        byte[] input = new byte[32];
        _ = ((BigInteger.One << (bitSize - 1)) - 1)
            .TryWriteBytes(input.AsSpan()[(32 - (bitSize / 8))..], out _, false, true);

        switch(bitSize)
        {
            case 8:
            {
                sbyte value = new AbiDecoder(input).Number<sbyte>(false, bitSize);
                Assert.Equal(SByte.MaxValue, value);
                break;
            }
            case 16:
            {
                short value = new AbiDecoder(input).Number<short>(false, bitSize);
                Assert.Equal(Int16.MaxValue, value);
                break;
            }
            case > 16 and <= 32:
            {
                int value = new AbiDecoder(input).Number<int>(false, bitSize);
                Assert.Equal(Int32.MaxValue >> (32 - bitSize), value);
                break;
            }
            case > 32 and <= 64:
            {
                long value = new AbiDecoder(input).Number<long>(false, bitSize);
                Assert.Equal(Int64.MaxValue >> (64 - bitSize), value);
                break;
            }
            case > 64 and <= 256:
            {
                var value = new AbiDecoder(input).Number<Int256>(false, bitSize);
                Assert.Equal(Int256.Pow(2, bitSize - 1) - 1, value);
                break;
            }
            default:
                throw new NotSupportedException();
        }
    }

    [Theory]
    [MemberData(nameof(BitSizes))]
    public void Should_Match_UInt_MinValue_Output(int bitSize)
    {
        byte[] input = new byte[32];

        switch(bitSize)
        {
            case 8:
            {
                byte value = new AbiDecoder(input).Number<byte>(true, bitSize);
                Assert.Equal(0, value);
                break;
            }
            case 16:
            {
                ushort value = new AbiDecoder(input).Number<ushort>(true, bitSize);
                Assert.Equal(0, value);
                break;
            }
            case > 16 and <= 32:
            {
                uint value = new AbiDecoder(input).Number<uint>(true, bitSize);
                Assert.Equal((uint) 0, value);
                break;
            }
            case > 32 and <= 64:
            {
                ulong value = new AbiDecoder(input).Number<ulong>(true, bitSize);
                Assert.Equal((ulong) 0, value);
                break;
            }
            case > 64 and <= 256:
            {
                var value = new AbiDecoder(input).Number<UInt256>(true, bitSize);
                Assert.Equal(UInt256.Zero, value);
                break;
            }
            default:
                throw new NotSupportedException();
        }
    }

    [Theory]
    [MemberData(nameof(BitSizes))]
    public void Should_Match_UInt_MaxValue_Output(int bitSize)
    {
        byte[] input = new byte[32];
        input.AsSpan(32 - (bitSize / 8)).Fill(255);

        switch(bitSize)
        {
            case 8:
            {
                byte value = new AbiDecoder(input).Number<byte>(true, bitSize);
                Assert.Equal(Byte.MaxValue, value);
                break;
            }
            case 16:
            {
                ushort value = new AbiDecoder(input).Number<ushort>(true, bitSize);
                Assert.Equal(UInt16.MaxValue, value);
                break;
            }
            case > 16 and <= 32:
            {
                uint value = new AbiDecoder(input).Number<uint>(true, bitSize);
                Assert.Equal(UInt32.MaxValue >> (32 - bitSize), value);
                break;
            }
            case > 32 and <= 64:
            {
                ulong value = new AbiDecoder(input).Number<ulong>(true, bitSize);
                Assert.Equal(UInt64.MaxValue >> (64 - bitSize), value);
                break;
            }
            case > 64 and <= 256:
            {
                var value = new AbiDecoder(input).Number<UInt256>(true, bitSize);
                Assert.Equal(UInt256.Pow(2, (uint) bitSize) - 1, value);
                break;
            }
            default:
                throw new NotSupportedException();
        }
    }
}
