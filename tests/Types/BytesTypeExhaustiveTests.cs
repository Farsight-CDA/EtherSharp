using EtherSharp.Common;
using EtherSharp.Types;
using System.Text.Json;

namespace EtherSharp.Tests.Types;

public sealed class BytesTypeExhaustiveTests
{
    [Fact]
    public void Should_Exercise_Core_Semantics_For_All_Bytes_Types()
    {
        Validate<Bytes1>(1);
        Validate<Bytes2>(2);
        Validate<Bytes3>(3);
        Validate<Bytes4>(4);
        Validate<Bytes5>(5);
        Validate<Bytes6>(6);
        Validate<Bytes7>(7);
        Validate<Bytes8>(8);
        Validate<Bytes9>(9);
        Validate<Bytes10>(10);
        Validate<Bytes11>(11);
        Validate<Bytes12>(12);
        Validate<Bytes13>(13);
        Validate<Bytes14>(14);
        Validate<Bytes15>(15);
        Validate<Bytes16>(16);
        Validate<Bytes17>(17);
        Validate<Bytes18>(18);
        Validate<Bytes19>(19);
        Validate<Bytes20>(20);
        Validate<Bytes21>(21);
        Validate<Bytes22>(22);
        Validate<Bytes23>(23);
        Validate<Bytes24>(24);
        Validate<Bytes25>(25);
        Validate<Bytes26>(26);
        Validate<Bytes27>(27);
        Validate<Bytes28>(28);
        Validate<Bytes29>(29);
        Validate<Bytes30>(30);
        Validate<Bytes31>(31);
        Validate<Bytes32>(32);
    }

    [Fact]
    public void Should_Apply_Bitwise_Operators_Across_Storage_Widths()
    {
        ValidateBitwise<Bytes1>(1, static (left, right) => left | right, static (left, right) => left & right);
        ValidateBitwise<Bytes2>(2, static (left, right) => left | right, static (left, right) => left & right);
        ValidateBitwise<Bytes3>(3, static (left, right) => left | right, static (left, right) => left & right);
        ValidateBitwise<Bytes4>(4, static (left, right) => left | right, static (left, right) => left & right);
        ValidateBitwise<Bytes5>(5, static (left, right) => left | right, static (left, right) => left & right);
        ValidateBitwise<Bytes6>(6, static (left, right) => left | right, static (left, right) => left & right);
        ValidateBitwise<Bytes7>(7, static (left, right) => left | right, static (left, right) => left & right);
        ValidateBitwise<Bytes8>(8, static (left, right) => left | right, static (left, right) => left & right);
        ValidateBitwise<Bytes9>(9, static (left, right) => left | right, static (left, right) => left & right);
        ValidateBitwise<Bytes10>(10, static (left, right) => left | right, static (left, right) => left & right);
        ValidateBitwise<Bytes11>(11, static (left, right) => left | right, static (left, right) => left & right);
        ValidateBitwise<Bytes12>(12, static (left, right) => left | right, static (left, right) => left & right);
        ValidateBitwise<Bytes13>(13, static (left, right) => left | right, static (left, right) => left & right);
        ValidateBitwise<Bytes14>(14, static (left, right) => left | right, static (left, right) => left & right);
        ValidateBitwise<Bytes15>(15, static (left, right) => left | right, static (left, right) => left & right);
        ValidateBitwise<Bytes16>(16, static (left, right) => left | right, static (left, right) => left & right);
        ValidateBitwise<Bytes17>(17, static (left, right) => left | right, static (left, right) => left & right);
        ValidateBitwise<Bytes18>(18, static (left, right) => left | right, static (left, right) => left & right);
        ValidateBitwise<Bytes19>(19, static (left, right) => left | right, static (left, right) => left & right);
        ValidateBitwise<Bytes20>(20, static (left, right) => left | right, static (left, right) => left & right);
        ValidateBitwise<Bytes21>(21, static (left, right) => left | right, static (left, right) => left & right);
        ValidateBitwise<Bytes22>(22, static (left, right) => left | right, static (left, right) => left & right);
        ValidateBitwise<Bytes23>(23, static (left, right) => left | right, static (left, right) => left & right);
        ValidateBitwise<Bytes24>(24, static (left, right) => left | right, static (left, right) => left & right);
        ValidateBitwise<Bytes25>(25, static (left, right) => left | right, static (left, right) => left & right);
        ValidateBitwise<Bytes26>(26, static (left, right) => left | right, static (left, right) => left & right);
        ValidateBitwise<Bytes27>(27, static (left, right) => left | right, static (left, right) => left & right);
        ValidateBitwise<Bytes28>(28, static (left, right) => left | right, static (left, right) => left & right);
        ValidateBitwise<Bytes29>(29, static (left, right) => left | right, static (left, right) => left & right);
        ValidateBitwise<Bytes30>(30, static (left, right) => left | right, static (left, right) => left & right);
        ValidateBitwise<Bytes31>(31, static (left, right) => left | right, static (left, right) => left & right);
        ValidateBitwise<Bytes32>(32, static (left, right) => left | right, static (left, right) => left & right);
    }

    private static void Validate<TBytes>(int length)
        where TBytes : struct, IFixedBytes<TBytes>, IComparable<TBytes>, IEquatable<TBytes>
    {
        byte[] first = CreateBytes(length, seed: 3);
        byte[] same = (byte[]) first.Clone();
        byte[] higher = (byte[]) first.Clone();
        higher[length - 1]++;

        var firstValue = TBytes.FromBytes(first);
        var sameValue = TBytes.FromBytes(same);
        var higherValue = TBytes.FromBytes(higher);

        Assert.Equal(first, firstValue.ToArray());
        for(int i = 0; i < length; i++)
        {
            Assert.Equal(first[i], firstValue[i]);
        }
        Assert.Equal(first[^1], firstValue[^1]);
        Assert.Equal(first[0], firstValue[^length]);

        Assert.Throws<IndexOutOfRangeException>(() => _ = firstValue[-1]);
        Assert.Throws<IndexOutOfRangeException>(() => _ = firstValue[length]);
        Assert.Throws<IndexOutOfRangeException>(() => _ = firstValue[^0]);
        Assert.Throws<IndexOutOfRangeException>(() => _ = firstValue[^(length + 1)]);

        Span<byte> copied = stackalloc byte[length];
        Assert.True(firstValue.TryWriteTo(copied));
        Assert.Equal(first, copied.ToArray());

        Span<byte> tooSmall = stackalloc byte[length - 1];
        Assert.False(firstValue.TryWriteTo(tooSmall));

        firstValue.CopyTo(copied);
        Assert.Equal(first, copied.ToArray());
        Assert.Equal(Convert.ToHexString(first), firstValue.ToHexUpper());
        Assert.Equal(Convert.ToHexStringLower(first), firstValue.ToHexLower());
        string expectedStringLower = $"0x{Convert.ToHexStringLower(first)}";
        Assert.Equal(expectedStringLower, firstValue.ToStringLower());
        Assert.Equal(expectedStringLower, firstValue.ToString());

        string expectedJson = $"\"0x{Convert.ToHexString(first)}\"";
        string defaultJson = JsonSerializer.Serialize(firstValue);
        var defaultRoundtrip = JsonSerializer.Deserialize<TBytes>(defaultJson);
        string evmJson = JsonSerializer.Serialize(firstValue, ParsingUtils.EvmSerializerOptions);
        var evmRoundtrip = JsonSerializer.Deserialize<TBytes>(evmJson, ParsingUtils.EvmSerializerOptions);

        Assert.Equal(expectedJson, defaultJson);
        Assert.Equal(firstValue, defaultRoundtrip);
        Assert.Equal(expectedJson, evmJson);
        Assert.Equal(firstValue, evmRoundtrip);

        Assert.True(firstValue.Equals(sameValue));
        Assert.Equal(firstValue.GetHashCode(), sameValue.GetHashCode());

        Assert.False(firstValue.Equals(higherValue));
        Assert.True(firstValue.CompareTo(higherValue) < 0);
        Assert.True(higherValue.CompareTo(firstValue) > 0);
    }

    private static byte[] CreateBytes(int length, byte seed)
    {
        byte[] bytes = new byte[length];
        for(int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = (byte) (seed + i);
        }

        return bytes;
    }

    private static void ValidateBitwise<TBytes>(
        int length,
        Func<TBytes, TBytes, TBytes> bitwiseOr,
        Func<TBytes, TBytes, TBytes> bitwiseAnd)
        where TBytes : struct, IFixedBytes<TBytes>
    {
        byte[] left = CreateBytes(length, 0x31);
        byte[] right = CreateBytes(length, 0xA4);
        byte[] expectedOr = new byte[length];
        byte[] expectedAnd = new byte[length];
        for(int index = 0; index < length; index++)
        {
            expectedOr[index] = (byte) (left[index] | right[index]);
            expectedAnd[index] = (byte) (left[index] & right[index]);
        }

        var leftValue = TBytes.FromBytes(left);
        var rightValue = TBytes.FromBytes(right);
        Assert.Equal(expectedOr, bitwiseOr(leftValue, rightValue).ToArray());
        Assert.Equal(expectedAnd, bitwiseAnd(leftValue, rightValue).ToArray());
    }
}
