using EtherSharp.Contract;

namespace EtherSharp.Tests.Contract;

public sealed class EVMByteCodeTests
{
    private static EVMByteCode CreateByteCode(params byte[] bytes)
        => new(bytes);

    [Fact]
    public void ContainsOpcode_Should_Return_True_When_Opcode_Exists()
    {
        var byteCode = CreateByteCode(0x60, 0x00, 0x52);

        Assert.True(byteCode.ContainsOpcode(0x52));
    }

    [Fact]
    public void ContainsOpcode_Should_Ignore_Opcode_Inside_Push_Data()
    {
        var byteCode = CreateByteCode(0x60, 0x52, 0x00);

        Assert.False(byteCode.ContainsOpcode(0x52));
    }

    [Fact]
    public void ContainsOpcode_Should_Treat_Push_As_Opcode()
    {
        var byteCode = CreateByteCode(0x60, 0xFF);

        Assert.True(byteCode.ContainsOpcode(0x60));
    }

    [Fact]
    public void ContainsOpcode_Should_Ignore_All_Push32_Data()
    {
        byte[] bytes = new byte[33];
        bytes[0] = 0x7F;
        Array.Fill(bytes, (byte) 0x52, 1, 32);

        var byteCode = new EVMByteCode(bytes);

        Assert.False(byteCode.ContainsOpcode(0x52));
    }

    [Fact]
    public void ContainsOpcode_Should_Ignore_Truncated_Push_Data()
    {
        var byteCode = CreateByteCode(0x61, 0xFF);

        Assert.False(byteCode.ContainsOpcode(0xFF));
    }

    [Fact]
    public void ContainsOpcode_Should_Ignore_Opcode_Inside_Compiler_Metadata()
    {
        var byteCode = CreateByteCode(
            0x00,
            0xA1, 0x64, 0x73, 0x6F, 0x6C, 0x63, 0x43, 0x52, 0xFF, 0x00,
            0x00, 0x0A);

        Assert.False(byteCode.ContainsOpcode(0x52));
    }

    [Fact]
    public void ContainsOpcode_Should_Scan_All_Code_When_Metadata_Is_Omitted()
    {
        var byteCode = CreateByteCode(0xFF, 0x52, 0x00, 0x00);

        Assert.True(byteCode.ContainsOpcode(0x52));
    }

    [Fact]
    public void ContainsOpcode_Should_Scan_All_Code_When_Trailer_Is_Not_Metadata()
    {
        var byteCode = CreateByteCode(0xFF, 0x52, 0x00, 0x01);

        Assert.True(byteCode.ContainsOpcode(0x52));
    }
}
