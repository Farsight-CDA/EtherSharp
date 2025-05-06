using EtherSharp.ABI;
using System.Numerics;
namespace EtherSharp.Tests.ABI.Decoder;

public class NumberArryAbiDecoderTests
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
    public void Should_Match_Int_MinValue_Output(uint bitSize)
    {
        byte[] input = Convert.FromHexString("000000000000000000000000000000000000000000000000000000000000002000000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000000000000000000");

        input.AsSpan()[64..(96 - ((int) bitSize / 8))].Fill(255);
        input[^((int) bitSize / 8)] = 128;

        switch(bitSize)
        {
            case 8:
            {
                sbyte[] value = new AbiDecoder(input).NumberArray<sbyte>(false, bitSize);
                Assert.Equal([sbyte.MinValue], value);
                break;
            }
            case 16:
            {
                short[] value = new AbiDecoder(input).NumberArray<short>(false, bitSize);
                Assert.Equal([short.MinValue], value);
                break;
            }
            case > 16 and <= 32:
            {
                int[] value = new AbiDecoder(input).NumberArray<int>(false, bitSize);
                Assert.Equal([int.MinValue >> (32 - (int) bitSize)], value);
                break;
            }
            case > 32 and <= 64:
            {
                long[] value = new AbiDecoder(input).NumberArray<long>(false, bitSize);
                Assert.Equal([long.MinValue >> (64 - (int) bitSize)], value);
                break;
            }
            case > 64 and <= 256:
            {
                var value = new AbiDecoder(input).NumberArray<BigInteger>(false, bitSize);
                Assert.Equal([-BigInteger.Pow(2, (int) (bitSize - 1))], value);
                break;
            }
            default:
                throw new NotSupportedException();
        }
    }

    [Theory]
    [MemberData(nameof(BitSizes))]
    public void Should_Match_Int_MaxValue_Output(uint bitSize)
    {
        byte[] input = Convert.FromHexString("000000000000000000000000000000000000000000000000000000000000002000000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000000000000000000");

        input.AsSpan()[(96 - ((int) bitSize / 8) + 1)..].Fill(255);
        input[^((int) bitSize / 8)] = 127;

        switch(bitSize)
        {
            case 8:
            {
                sbyte[] value = new AbiDecoder(input).NumberArray<sbyte>(false, bitSize);
                Assert.Equal([sbyte.MaxValue], value);
                break;
            }
            case 16:
            {
                short[] value = new AbiDecoder(input).NumberArray<short>(false, bitSize);
                Assert.Equal([short.MaxValue], value);
                break;
            }
            case > 16 and <= 32:
            {
                int[] value = new AbiDecoder(input).NumberArray<int>(false, bitSize);
                Assert.Equal([int.MaxValue >> (32 - (int) bitSize)], value);
                break;
            }
            case > 32 and <= 64:
            {
                long[] value = new AbiDecoder(input).NumberArray<long>(false, bitSize);
                Assert.Equal([long.MaxValue >> (64 - (int) bitSize)], value);
                break;
            }
            case > 64 and <= 256:
            {
                var value = new AbiDecoder(input).NumberArray<BigInteger>(false, bitSize);
                Assert.Equal([BigInteger.Pow(2, (int) (bitSize - 1)) - 1], value);
                break;
            }
            default:
                throw new NotSupportedException();
        }
    }

    [Theory]
    [MemberData(nameof(BitSizes))]
    public void Should_Match_UInt_MinValue_Output(uint bitSize)
    {
        byte[] input = Convert.FromHexString("000000000000000000000000000000000000000000000000000000000000002000000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000000000000000000");

        switch(bitSize)
        {
            case 8:
            {
                byte[] value = new AbiDecoder(input).NumberArray<byte>(true, bitSize);
                Assert.Equal([byte.MinValue], value);
                break;
            }
            case 16:
            {
                ushort[] value = new AbiDecoder(input).NumberArray<ushort>(true, bitSize);
                Assert.Equal([ushort.MinValue], value);
                break;
            }
            case > 16 and <= 32:
            {
                uint[] value = new AbiDecoder(input).NumberArray<uint>(true, bitSize);
                Assert.Equal([uint.MinValue >> (32 - (int) bitSize)], value);
                break;
            }
            case > 32 and <= 64:
            {
                ulong[] value = new AbiDecoder(input).NumberArray<ulong>(true, bitSize);
                Assert.Equal([ulong.MinValue >> (64 - (int) bitSize)], value);
                break;
            }
            case > 64 and <= 256:
            {
                var value = new AbiDecoder(input).NumberArray<BigInteger>(true, bitSize);
                Assert.Equal([BigInteger.Zero], value);
                break;
            }
            default:
                throw new NotSupportedException();
        }
    }

    [Theory]
    [MemberData(nameof(BitSizes))]
    public void Should_Match_UInt_MaxValue_Output(uint bitSize)
    {
        byte[] input = Convert.FromHexString("000000000000000000000000000000000000000000000000000000000000002000000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000000000000000000");

        input.AsSpan()[(96 - ((int) bitSize / 8) + 1)..].Fill(255);

        input[^((int) bitSize / 8)] = 255;

        switch(bitSize)
        {
            case 8:
            {
                byte[] value = new AbiDecoder(input).NumberArray<byte>(true, bitSize);
                Assert.Equal([byte.MaxValue], value);
                break;
            }
            case 16:
            {
                ushort[] value = new AbiDecoder(input).NumberArray<ushort>(true, bitSize);
                Assert.Equal([ushort.MaxValue], value);
                break;
            }
            case > 16 and <= 32:
            {
                uint[] value = new AbiDecoder(input).NumberArray<uint>(true, bitSize);
                Assert.Equal([uint.MaxValue >> (32 - (int) bitSize)], value);
                break;
            }
            case > 32 and <= 64:
            {
                ulong[] value = new AbiDecoder(input).NumberArray<ulong>(true, bitSize);
                Assert.Equal([ulong.MaxValue >> (64 - (int) bitSize)], value);
                break;
            }
            case > 64 and <= 256:
            {
                var value = new AbiDecoder(input).NumberArray<BigInteger>(true, bitSize);
                Assert.Equal([BigInteger.Pow(2, (int) bitSize) - 1], value);
                break;
            }
            default:
                throw new NotSupportedException();
        }
    }
}
