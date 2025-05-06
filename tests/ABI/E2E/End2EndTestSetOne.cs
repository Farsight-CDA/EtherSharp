﻿using EtherSharp.ABI;

namespace EtherSharp.Tests.ABI.E2E;

public class End2EndTests
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
}