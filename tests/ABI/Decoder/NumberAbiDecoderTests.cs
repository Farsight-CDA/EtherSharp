using EtherSharp.ABI;
using System.Numerics;

namespace EtherSharp.Tests.ABI.Decoder;
public class AbiNumberEncodingTests
{
    public static TheoryData<int> BitSizes
        => new TheoryData<int>(Enumerable.Range(1, 32).Select(x => x * 8));
    public static TheoryData<int> NonNativeBitSizes
        => new TheoryData<int>(Enumerable.Range(1, 32)
            .Select(x => x * 8)
            .Where(x => x != 8 && x != 16 && x != 32 && x != 64)
        );

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
                var value = new AbiDecoder(input).Number<sbyte>(false, bitSize);
                Assert.Equal(sbyte.MinValue, value);
                break;
            }
            case 16:
            {
                var value = new AbiDecoder(input).Number<short>(false, bitSize);
                Assert.Equal(short.MinValue, value);
                break;
            }
            case > 16 and <= 32:
            {
                var value = new AbiDecoder(input).Number<int>(false, bitSize);
                Assert.Equal(int.MinValue >> (32 - bitSize), value);
                break;
            }
            case > 32 and <= 64:
            {
                var value = new AbiDecoder(input).Number<long>(false, bitSize);
                Assert.Equal(long.MinValue >> (64 - bitSize), value);
                break;
            }
            case > 64 and <= 256:
            {
                var value = new AbiDecoder(input).Number<BigInteger>(false, bitSize);
                Assert.Equal(-BigInteger.Pow(2, bitSize - 1), value);
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
                var value = new AbiDecoder(input).Number<sbyte>(false, bitSize);
                Assert.Equal(sbyte.MaxValue, value);
                break;
            }
            case 16:
            {
                var value = new AbiDecoder(input).Number<short>(false, bitSize);
                Assert.Equal(short.MaxValue, value);
                break;
            }
            case > 16 and <= 32:
            {
                var value = new AbiDecoder(input).Number<int>(false, bitSize);
                Assert.Equal(int.MaxValue >> (32 - bitSize), value);
                break;
            }
            case > 32 and <= 64:
            {
                var value = new AbiDecoder(input).Number<long>(false, bitSize);
                Assert.Equal(long.MaxValue >> (64 - bitSize), value);
                break;
            }
            case > 64 and <= 256:
            {
                var value = new AbiDecoder(input).Number<BigInteger>(false, bitSize);
                Assert.Equal(BigInteger.Pow(2, bitSize - 1) - 1, value);
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
                var value = new AbiDecoder(input).Number<byte>(true, bitSize);
                Assert.Equal(0, value);
                break;
            }
            case 16:
            {
                var value = new AbiDecoder(input).Number<ushort>(true, bitSize);
                Assert.Equal(0, value);
                break;
            }
            case > 16 and <= 32:
            {
                var value = new AbiDecoder(input).Number<uint>(true, bitSize);
                Assert.Equal((uint) 0, value);
                break;
            }
            case > 32 and <= 64:
            {
                var value = new AbiDecoder(input).Number<ulong>(true, bitSize);
                Assert.Equal((ulong) 0, value);
                break;
            }
            case > 64 and <= 256:
            {
                var value = new AbiDecoder(input).Number<BigInteger>(true, bitSize);
                Assert.Equal(0, value);
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
                var value = new AbiDecoder(input).Number<byte>(true, bitSize);
                Assert.Equal(byte.MaxValue, value);
                break;
            }
            case 16:
            {
                var value = new AbiDecoder(input).Number<ushort>(true, bitSize);
                Assert.Equal(ushort.MaxValue, value);
                break;
            }
            case > 16 and <= 32:
            {
                var value = new AbiDecoder(input).Number<uint>(true, bitSize);
                Assert.Equal(uint.MaxValue >> (32 - bitSize), value);
                break;
            }
            case > 32 and <= 64:
            {
                var value = new AbiDecoder(input).Number<ulong>(true, bitSize);
                Assert.Equal(ulong.MaxValue >> (64 - bitSize), value);
                break;
            }
            case > 64 and <= 256:
            {
                var value = new AbiDecoder(input).Number<BigInteger>(true, bitSize);
                Assert.Equal(BigInteger.Pow(2, bitSize) - 1, value);
                break;
            }
            default:
                throw new NotSupportedException();
        }
    }
}