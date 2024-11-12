using EtherSharp.ABI.Decode;
using System.Numerics;

namespace EtherSharp.Tests.ABI.Decoder;
public class AbiNumberEncodingTests
{
    public static IEnumerable<object[]> BitSizes
        => Enumerable.Range(1, 32)
            .Select(x => new object[] { x * 8 });
    public static IEnumerable<object[]> NonNativeBitSizes
        => Enumerable.Range(1, 32)
            .Select(x => x * 8)
            .Where(x => x != 8 && x != 16 && x != 32 && x != 64)
            .Select(x => new object[] { x }
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
                _ = new AbiDecoder(input).Number(out sbyte value, false, bitSize);
                Assert.Equal(sbyte.MinValue, value);
                break;
            }
            case 16:
            {
                _ = new AbiDecoder(input).Number(out short value, false, bitSize);
                Assert.Equal(short.MinValue, value);
                break;
            }
            case > 16 and <= 32:
            {
                _ = new AbiDecoder(input).Number(out int value, false, bitSize);
                Assert.Equal(int.MinValue >> (32 - bitSize), value);
                break;
            }
            case > 32 and <= 64:
            {
                _ = new AbiDecoder(input).Number(out long value, false, bitSize);
                Assert.Equal(long.MinValue >> (64 - bitSize), value);
                break;
            }
            case > 64 and <= 256:
            {
                _ = new AbiDecoder(input).Number(out BigInteger value, false, bitSize);
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
                _ = new AbiDecoder(input).Number(out sbyte value, false, bitSize);
                Assert.Equal(sbyte.MaxValue, value);
                break;
            }
            case 16:
            {
                _ = new AbiDecoder(input).Number(out short value, false, bitSize);
                Assert.Equal(short.MaxValue, value);
                break;
            }
            case > 16 and <= 32:
            {
                _ = new AbiDecoder(input).Number(out int value, false, bitSize);
                Assert.Equal(int.MaxValue >> (32 - bitSize), value);
                break;
            }
            case > 32 and <= 64:
            {
                _ = new AbiDecoder(input).Number(out long value, false, bitSize);
                Assert.Equal(long.MaxValue >> (64 - bitSize), value);
                break;
            }
            case > 64 and <= 256:
            {
                _ = new AbiDecoder(input).Number(out BigInteger value, false, bitSize);
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
                _ = new AbiDecoder(input).Number(out byte value, true, bitSize);
                Assert.Equal(0, value);
                break;
            }
            case 16:
            {
                _ = new AbiDecoder(input).Number(out ushort value, true, bitSize);
                Assert.Equal(0, value);
                break;
            }
            case > 16 and <= 32:
            {
                _ = new AbiDecoder(input).Number(out uint value, true, bitSize);
                Assert.Equal((uint) 0, value);
                break;
            }
            case > 32 and <= 64:
            {
                _ = new AbiDecoder(input).Number(out ulong value, true, bitSize);
                Assert.Equal((ulong) 0, value);
                break;
            }
            case > 64 and <= 256:
            {
                _ = new AbiDecoder(input).Number(out BigInteger value, true, bitSize);
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
                _ = new AbiDecoder(input).Number(out byte value, true, bitSize);
                Assert.Equal(byte.MaxValue, value);
                break;
            }
            case 16:
            {
                _ = new AbiDecoder(input).Number(out ushort value, true, bitSize);
                Assert.Equal(ushort.MaxValue, value);
                break;
            }
            case > 16 and <= 32:
            {
                _ = new AbiDecoder(input).Number(out uint value, true, bitSize);
                Assert.Equal(uint.MaxValue >> (32 - bitSize), value);
                break;
            }
            case > 32 and <= 64:
            {
                _ = new AbiDecoder(input).Number(out ulong value, true, bitSize);
                Assert.Equal(ulong.MaxValue >> (64 - bitSize), value);
                break;
            }
            case > 64 and <= 256:
            {
                _ = new AbiDecoder(input).Number(out BigInteger value, true, bitSize);
                Assert.Equal(BigInteger.Pow(2, bitSize) - 1, value);
                break;
            }
            default:
                throw new NotSupportedException();
        }
    }
}