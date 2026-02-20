using EtherSharp.ABI;
using EtherSharp.Types;

namespace EtherSharp.Tests.ABI.E2E;

public class AbiRoundTripTests
{
    [Fact]
    public void Should_Encode_And_Decode_Int8_Array()
    {
        // Arrange
        sbyte[] input = [1, 2, 3, 4, 5];

        // Act
        byte[] encoded = new AbiEncoder().NumberArray(false, 8, input).Build();
        var decoder = new AbiDecoder(encoded);
        sbyte[] result = decoder.NumberArray<sbyte>(false, 8);

        // Assert
        Assert.Equal(input, result);
    }

    [Fact]
    public void Should_Encode_And_Decode_UInt16_String()
    {
        // Arrange
        ushort number = 16;
        string str = "Hello, World!";

        // Act
        byte[] encoded = new AbiEncoder()
            .Number(number, true, 16)
            .String(str)
            .Build();

        var decoder = new AbiDecoder(encoded);
        ushort outputNumber = decoder.Number<ushort>(true, 16);
        string outputStr = decoder.String();

        // Assert
        Assert.Equal(number, outputNumber);
        Assert.Equal(str, outputStr);
    }

    [Fact]
    public void Should_Encode_And_Decode_Address_Array()
    {
        Address[] input = [
            Address.Zero,
            Address.FromString("0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF"),
            Address.Zero
        ];

        var encoder = new AbiEncoder().AddressArray(input);
        var decoder = new AbiDecoder(encoder.Build());

        var output = decoder.AddressArray();

        Assert.Equal(input, output);
    }

    [Fact]
    public void Should_Encode_And_Decode_Bool_Array()
    {
        bool[] input = [
            true,
            false,
            true
        ];

        var encoder = new AbiEncoder().BoolArray(input);
        var decoder = new AbiDecoder(encoder.Build());

        bool[] output = decoder.BoolArray();

        Assert.Equal(input, output);
    }

    [Fact]
    public void Should_Encode_And_Decode_Bytes1_Array()
    {
        byte[] input = [0x12, 0x34, 0xab, 0xcd];

        var encoder = new AbiEncoder().Bytes1Array(input);
        var decoder = new AbiDecoder(encoder.Build());

        byte[] output = decoder.Bytes1Array();

        Assert.Equal(input, output);
    }

    [Fact]
    public void Should_Encode_And_Decode_Bytes32_Array()
    {
        // Arrange
        ReadOnlyMemory<byte>[] input = [
            new byte[32] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1a, 0x1b, 0x1c, 0x1d, 0x1e, 0x1f },
            new byte[32] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff },
            new byte[32] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }
        ];

        // Act
        var encoder = new AbiEncoder().Bytes32Array(input);
        var decoder = new AbiDecoder(encoder.Build());
        var output = decoder.Bytes32Array();

        // Assert
        Assert.Equal(input.Length, output.Length);
        for(int i = 0; i < input.Length; i++)
        {
            Assert.Equal(input[i].Span, output[i].Span);
        }
    }

    [Fact]
    public void Should_Encode_And_Decode_Empty_Bytes32_Array()
    {
        // Arrange
        ReadOnlyMemory<byte>[] input = [];

        // Act
        var encoder = new AbiEncoder().Bytes32Array(input);
        var decoder = new AbiDecoder(encoder.Build());
        var output = decoder.Bytes32Array();

        // Assert
        Assert.Empty(output);
    }

    [Fact]
    public void Should_Encode_And_Decode_Bytes32_Array_With_Other_Types()
    {
        // Arrange
        ushort number = 42;
        ReadOnlyMemory<byte>[] bytes32Input = [
            new byte[32] { 0xde, 0xad, 0xbe, 0xef, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
            new byte[32] { 0xca, 0xfe, 0xba, 0xbe, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }
        ];
        string str = "Test String";
        var address = Address.Zero;

        // Act
        byte[] encoded = new AbiEncoder()
            .Number(number, true, 16)
            .Bytes32Array(bytes32Input)
            .String(str)
            .Address(address)
            .Build();

        var decoder = new AbiDecoder(encoded);
        ushort outputNumber = decoder.Number<ushort>(true, 16);
        var outputBytes32 = decoder.Bytes32Array();
        string outputStr = decoder.String();
        var outputAddress = decoder.Address();

        // Assert
        Assert.Equal(number, outputNumber);
        Assert.Equal(bytes32Input.Length, outputBytes32.Length);
        for(int i = 0; i < bytes32Input.Length; i++)
        {
            Assert.Equal(bytes32Input[i].Span, outputBytes32[i].Span);
        }
        Assert.Equal(str, outputStr);
        Assert.Equal(address, outputAddress);
    }

    [Fact]
    public void Should_Advance_Head_After_StringArray_When_Decoding_Next_Value()
    {
        uint expected = 1337;
        byte[] encoded = new AbiEncoder()
            .StringArray("alpha", "beta")
            .UInt32(expected)
            .Build();

        var decoder = new AbiDecoder(encoded);
        string[] values = decoder.StringArray();
        uint actual = decoder.UInt32();

        Assert.Equal(["alpha", "beta"], values);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Should_Advance_Head_After_BytesArray_When_Decoding_Next_Value()
    {
        uint expected = 7331;
        byte[] encoded = new AbiEncoder()
            .BytesArray([new byte[] { 0x01, 0x02 }, new byte[] { 0x03 }])
            .UInt32(expected)
            .Build();

        var decoder = new AbiDecoder(encoded);
        var values = decoder.BytesArray();
        uint actual = decoder.UInt32();

        Assert.Equal(2, values.Length);
        Assert.Equal(new byte[] { 0x01, 0x02 }, values[0].ToArray());
        Assert.Equal(new byte[] { 0x03 }, values[1].ToArray());
        Assert.Equal(expected, actual);
    }
}
