using EtherSharp.Common;
using EtherSharp.Common.Converters.Json;
using EtherSharp.Numerics;
using EtherSharp.Types;
using System.Buffers;
using System.Text.Json;

namespace EtherSharp.Tests.Common.Converters.Json;

public sealed class JsonHexReaderTests
{
    [Theory]
    [InlineData("\"123\"")]
    [InlineData("\"0x123\"")]
    [InlineData("\"0X123\"")]
    [InlineData("\"0\\u0078123\"")]
    public void Should_Read_Hex_Numbers_With_Optional_Prefix(string json)
    {
        var options = ParsingUtils.EvmSerializerOptions;

        Assert.Equal(0x123, JsonSerializer.Deserialize<int>(json, options));
        Assert.Equal(0x123u, JsonSerializer.Deserialize<uint>(json, options));
        Assert.Equal(0x123L, JsonSerializer.Deserialize<long>(json, options));
        Assert.Equal(0x123UL, JsonSerializer.Deserialize<ulong>(json, options));
        Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(0x123), JsonSerializer.Deserialize<DateTimeOffset>(json, options));
    }

    [Fact]
    public void Should_Read_Number_From_Segmented_String_Token()
    {
        var first = new BufferSegment("\"0"u8.ToArray());
        var last = first.Append("X123\""u8.ToArray());
        var sequence = new ReadOnlySequence<byte>(first, 0, last, last.Memory.Length);
        var reader = new Utf8JsonReader(sequence);

        Assert.True(reader.Read());
        Assert.True(reader.HasValueSequence);
        Assert.Equal(0x123UL, ULongHexConverter.Instance.Read(ref reader, typeof(ulong), ParsingUtils.EvmSerializerOptions));
    }

    [Fact]
    public void Should_Preserve_Full_Width_Bits()
    {
        var options = ParsingUtils.EvmSerializerOptions;

        Assert.Equal(-1, JsonSerializer.Deserialize<int>("\"0xFFFFFFFF\"", options));
        Assert.Equal(-1L, JsonSerializer.Deserialize<long>("\"FFFFFFFFFFFFFFFF\"", options));
        Assert.Equal(UInt64.MaxValue, JsonSerializer.Deserialize<ulong>("\"0XFFFFFFFFFFFFFFFF\"", options));
    }

    [Theory]
    [InlineData("\"zz3\"")]
    [InlineData("\"0x\"")]
    public void Should_Reject_Invalid_Hex_Numbers(string json)
        => Assert.Throws<FormatException>(() => JsonSerializer.Deserialize<ulong>(json, ParsingUtils.EvmSerializerOptions));

    [Fact]
    public void Should_Reject_Too_Many_Digits_Even_When_Numeric_Value_Fits()
        => Assert.Throws<InvalidOperationException>(() => JsonSerializer.Deserialize<uint>("\"0x000000001\"", ParsingUtils.EvmSerializerOptions));

    [Theory]
    [InlineData("\"0123\"")]
    [InlineData("\"0x0123\"")]
    [InlineData("\"0X0123\"")]
    public void Should_Strip_The_Same_Prefix_For_Byte_And_256_Bit_Values(string json)
    {
        var options = ParsingUtils.EvmSerializerOptions;

        Assert.Equal(Bytes2.Parse("0123"), JsonSerializer.Deserialize<Bytes2>(json, options));
        Assert.Equal<byte>([0x01, 0x23], JsonSerializer.Deserialize<byte[]>(json, options)!);
        Assert.Equal(new UInt256(0x123), JsonSerializer.Deserialize<UInt256>(json, options));
        Assert.Equal(new Int256(0x123), JsonSerializer.Deserialize<Int256>(json, options));
    }

    [Theory]
    [InlineData("\"123\"")]
    [InlineData("\"0X123\"")]
    [InlineData("\"0\\u0078123\"")]
    public void Should_Still_Pad_Odd_Length_Byte_Values(string json)
        => Assert.Equal<byte>([0x01, 0x23], JsonSerializer.Deserialize<byte[]>(json, ParsingUtils.EvmSerializerOptions)!);

    [Theory]
    [InlineData("\"10\"", 10u)]
    [InlineData("\"0X10\"", 16u)]
    [InlineData("10", 10u)]
    public void Should_Keep_Unprefixed_Transaction_Types_Decimal(string json, uint expected)
        => Assert.Equal((TxType) expected, JsonSerializer.Deserialize<TxType>(json, ParsingUtils.EvmSerializerOptions));

    private sealed class BufferSegment : ReadOnlySequenceSegment<byte>
    {
        public BufferSegment(ReadOnlyMemory<byte> memory)
        {
            Memory = memory;
        }

        public BufferSegment Append(ReadOnlyMemory<byte> memory)
        {
            var next = new BufferSegment(memory) { RunningIndex = RunningIndex + Memory.Length };
            Next = next;
            return next;
        }
    }
}
