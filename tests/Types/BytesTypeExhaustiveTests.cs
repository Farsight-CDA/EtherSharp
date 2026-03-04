using EtherSharp.Types;

namespace EtherSharp.Tests.Types;

public class BytesTypeExhaustiveTests
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

        Assert.True(firstValue.Bytes.SequenceEqual(first));

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
}
