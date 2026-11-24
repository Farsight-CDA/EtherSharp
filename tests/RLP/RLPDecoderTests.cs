using EtherSharp.Numerics;
using EtherSharp.RLP;
using EtherSharp.Tx;
using EtherSharp.Types;
using System.Numerics;
using System.Text;

namespace EtherSharp.Tests.RLP;

public sealed class RLPDecoderTests
{
    [Fact]
    public void Should_Decode_Known_List_Vector()
    {
        var decoder = new RLPDecoder(Convert.FromHexString("c88363617483646f67"));

        Assert.True(decoder.TryReadList(out var list));
        Assert.True(decoder.IsComplete);
        Assert.True(list.TryReadString(out var cat));
        Assert.Equal("cat", Encoding.ASCII.GetString(cat));
        Assert.True(list.TryReadString(out var dog));
        Assert.Equal("dog", Encoding.ASCII.GetString(dog));
        Assert.True(list.IsComplete);
    }

    [Theory]
    [InlineData(0ul)]
    [InlineData(0x7ful)]
    [InlineData(0x80ul)]
    [InlineData(UInt64.MaxValue)]
    public void Should_Round_Trip_UInt64(ulong expected)
    {
        byte[] buffer = new byte[RLPEncoder.GetIntSize(expected)];
        _ = new RLPEncoder(buffer).EncodeInt(expected);

        var decoder = new RLPDecoder(buffer);

        Assert.True(decoder.TryReadInt(out ulong value));
        Assert.Equal(expected, value);
        Assert.True(decoder.IsComplete);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("18446744073709551616")]
    [InlineData("115792089237316195423570985008687907853269984665640564039457584007913129639935")]
    public void Should_Round_Trip_UInt256(string decimalValue)
    {
        var expected = (UInt256) BigInteger.Parse(decimalValue);
        byte[] buffer = new byte[RLPEncoder.GetIntSize(in expected)];
        _ = new RLPEncoder(buffer).EncodeInt(expected);

        var decoder = new RLPDecoder(buffer);

        Assert.True(decoder.TryReadInt(out UInt256 value));
        Assert.Equal(expected, value);
        Assert.True(decoder.IsComplete);
    }

    [Theory]
    [InlineData("00")]
    [InlineData("820001")]
    [InlineData("89010000000000000000")]
    public void Should_Reject_Non_Canonical_Int_Without_Advancing(string hex)
    {
        byte[] encoded = Convert.FromHexString(hex);
        var decoder = new RLPDecoder(encoded);

        Assert.False(decoder.TryReadInt(out ulong _));
        AssertNextItem(decoder, encoded);
    }

    [Theory]
    [InlineData(55)]
    [InlineData(56)]
    public void Should_Round_Trip_String_Around_Long_Form_Boundary(int length)
    {
        byte[] payload = new byte[length];
        payload.AsSpan().Fill(0x61);
        byte[] buffer = new byte[RLPEncoder.GetStringSize(payload)];
        _ = new RLPEncoder(buffer).EncodeString(payload);

        var decoder = new RLPDecoder(buffer);

        Assert.True(decoder.TryReadString(out var value));
        Assert.Equal(payload, value.ToArray());
        Assert.True(decoder.IsComplete);
    }

    [Fact]
    public void Should_Read_Raw_Item_With_Header()
    {
        var decoder = new RLPDecoder(Convert.FromHexString("c18083646f67"));

        Assert.True(decoder.TryReadRaw(out var list, out var listHeader));
        Assert.True(listHeader.IsList);
        Assert.Equal(Convert.FromHexString("c180"), list.ToArray());
        Assert.True(decoder.TryReadRaw(out var dog, out var dogHeader));
        Assert.False(dogHeader.IsList);
        Assert.Equal("dog", Encoding.ASCII.GetString(dog[dogHeader.PrefixLength..]));
        Assert.True(decoder.IsComplete);
    }

    [Theory]
    [InlineData("8180", true)]
    [InlineData("c7c0c1c0c3c0c1c0", true)]
    [InlineData("8100", false)]
    [InlineData("b80100", false)]
    [InlineData("f80180", false)]
    [InlineData("bf0f000000000000021111", false)]
    [InlineData("c180c0", false)]
    [InlineData("c28100", false)]
    [InlineData("c3c18180", false)]
    public void Should_Validate_Canonical_Encoding(string hex, bool expected)
        => Assert.Equal(expected, RLPDecoder.IsCanonical(Convert.FromHexString(hex)));

    [Fact]
    public void Should_Round_Trip_Access_List()
    {
        StateAccess[] expected =
        [
            new StateAccess(
                Address.Parse("0x1111111111111111111111111111111111111111"),
                new[] { Bytes32.FromBytes(Enumerable.Repeat((byte) 0x22, 32).ToArray()), default }
            ),
            new StateAccess(Address.Parse("0x3333333333333333333333333333333333333333"), ReadOnlyMemory<Bytes32>.Empty)
        ];
        int contentSize = TxRLPEncoder.GetAccessListLength(expected);
        byte[] buffer = new byte[RLPEncoder.GetListSize(contentSize)];
        _ = new RLPEncoder(buffer)
            .EncodeList(contentSize)
            .EncodeAccessList(expected);

        var decoder = new RLPDecoder(buffer);

        Assert.True(decoder.TryReadAccessList(out var accessList));
        Assert.True(decoder.IsComplete);
        Assert.Equal(expected.Length, accessList.Length);
        for(int i = 0; i < expected.Length; i++)
        {
            Assert.Equal(expected[i].Address, accessList[i].Address);
            Assert.Equal(expected[i].StorageKeys.ToArray(), accessList[i].StorageKeys.ToArray());
        }
    }

    [Theory]
    [InlineData("d6d5941111111111111111111111111111111111111111")]
    [InlineData("d9d8941111111111111111111111111111111111111111c28001")]
    public void Should_Reject_Invalid_Access_List_Without_Advancing(string hex)
    {
        byte[] encoded = Convert.FromHexString(hex);
        var decoder = new RLPDecoder(encoded);

        Assert.False(decoder.TryReadAccessList(out _));
        AssertNextItem(decoder, encoded);
    }

    [Theory]
    [InlineData("80", null)]
    [InlineData("941111111111111111111111111111111111111111", "0x1111111111111111111111111111111111111111")]
    public void Should_Decode_Optional_Address(string hex, string? expected)
    {
        var decoder = new RLPDecoder(Convert.FromHexString(hex));

        Assert.True(decoder.TryReadOptionalAddress(out var address));
        Assert.Equal(expected is null ? default(Address?) : Address.Parse(expected), address);
    }

    [Fact]
    public void Should_Round_Trip_Signature()
    {
        byte[] signature = new byte[64];
        signature[2] = 0x7f;
        signature.AsSpan(32..).Fill(0xab);
        byte[] buffer = new byte[TxRLPEncoder.MaxEncodedSignatureLength];
        _ = new RLPEncoder(buffer).EncodeSignature(signature, 1, out int signatureLength);

        var decoder = new RLPDecoder(buffer.AsSpan(0, signatureLength));

        Assert.True(decoder.TryReadSignature(out ulong v, out var r, out var s));
        Assert.Equal(1ul, v);
        Assert.Equal(Bytes32.FromBytes(signature.AsSpan(0, 32)), r);
        Assert.Equal(Bytes32.FromBytes(signature.AsSpan(32)), s);
        Assert.True(decoder.IsComplete);
    }

    private static void AssertNextItem(RLPDecoder decoder, ReadOnlySpan<byte> expected)
    {
        Assert.True(decoder.TryReadRaw(out var item, out _));
        Assert.Equal(expected.ToArray(), item.ToArray());
    }
}
