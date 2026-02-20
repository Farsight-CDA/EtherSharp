using EtherSharp.ABI;
using System.Buffers.Binary;

namespace EtherSharp.Tests.ABI.Decoder;

public class NonStrictAliasedOffsetTests
{
    [Fact]
    public void Should_Decode_Aliased_Empty_String_And_Bytes()
    {
        byte[] payload = BuildAliasedEmptyDynamicPairPayload();

        var decoder = new AbiDecoder(payload);

        string str = decoder.String();
        var bytes = decoder.Bytes();

        Assert.Equal(String.Empty, str);
        Assert.True(bytes.IsEmpty);
    }

    [Fact]
    public void Should_Decode_Aliased_Empty_NumberArray_And_AddressArray()
    {
        byte[] payload = BuildAliasedEmptyDynamicPairPayload();

        var decoder = new AbiDecoder(payload);

        uint[] numbers = decoder.NumberArray<uint>(true, 32);
        var addresses = decoder.AddressArray();

        Assert.Empty(numbers);
        Assert.Empty(addresses);
    }

    [Fact]
    public void Should_Decode_Aliased_Empty_Bytes32Array_And_BoolArray()
    {
        byte[] payload = BuildAliasedEmptyDynamicPairPayload();

        var decoder = new AbiDecoder(payload);

        var bytes32 = decoder.Bytes32Array();
        bool[] bools = decoder.BoolArray();

        Assert.Empty(bytes32);
        Assert.Empty(bools);
    }

    [Fact]
    public void Should_Decode_Aliased_Empty_StringArray_And_BytesArray()
    {
        byte[] payload = BuildAliasedEmptyDynamicPairPayload();

        var decoder = new AbiDecoder(payload);

        string[] strings = decoder.StringArray();
        var bytes = decoder.BytesArray();

        Assert.Empty(strings);
        Assert.Empty(bytes);
    }

    [Fact]
    public void Should_Decode_Aliased_Empty_Generic_Arrays()
    {
        byte[] payload = BuildAliasedEmptyDynamicPairPayload();

        var decoder = new AbiDecoder(payload);

        bool[][] first = decoder.Array(d => d.BoolArray());
        bool[][] second = decoder.Array(d => d.BoolArray());

        Assert.Empty(first);
        Assert.Empty(second);
    }

    [Fact]
    public void Should_Decode_Aliased_Dynamic_Tuples()
    {
        byte[] payload = new byte[128];

        // Top-level tuple has two dynamic tuple heads; both point to the same struct payload at offset 64.
        WriteSlotOffset(payload, 0, 64);
        WriteSlotOffset(payload, 1, 64);

        // Shared dynamic tuple payload at offset 64: one field (string), field offset = 32.
        WriteSlotOffset(payload, 2, 32);

        // String payload at offset 64 + 32 => slot index 3: empty string length = 0.
        // Slot is already zeroed.

        var decoder = new AbiDecoder(payload);

        string first = decoder.DynamicTuple(d => d.String());
        string second = decoder.DynamicTuple(d => d.String());

        Assert.Equal(String.Empty, first);
        Assert.Equal(String.Empty, second);
    }

    private static byte[] BuildAliasedEmptyDynamicPairPayload()
    {
        byte[] payload = new byte[96];

        // Two dynamic heads both pointing to the same empty payload at offset 64.
        WriteSlotOffset(payload, 0, 64);
        WriteSlotOffset(payload, 1, 64);

        // Shared payload slot at offset 64 contains length = 0 (already zeroed).
        return payload;
    }

    private static void WriteSlotOffset(byte[] payload, int slotIndex, int offset)
        => BinaryPrimitives.WriteUInt32BigEndian(payload.AsSpan((slotIndex * 32) + 28, 4), (uint) offset);
}