using EtherSharp.Contract;
using EtherSharp.Types;

namespace EtherSharp.Tests.Contract;

public sealed class EVMByteCodeTests
{
    private static EVMByteCode CreateByteCode(params byte[] bytes)
        => new(bytes);

    private static EVMByteCode CreateByteCodeWithSolcMetadata(params byte[] metadataBytes)
    {
        byte[] metadata = [0xA1, 0x64, 0x73, 0x6F, 0x6C, 0x63, .. metadataBytes];
        byte[] byteCode = [0x00, .. metadata, (byte) (metadata.Length >> 8), (byte) metadata.Length];

        return new EVMByteCode(byteCode);
    }

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

    [Fact]
    public void HasError_Should_Match_Optimized_Shifted_Leading_Zero_Selector()
    {
        var byteCode = new EVMByteCode(Convert.FromHexString(
            "6080604052348015600e575f5ffd5b50600436106026575f3560e01c8063a9cc471814602a575b5f5ffd5b60306032565b005b60405162bfc92160e01b815260040160405180910390fdfea2646970667358221220a037d236010d3be1534daf508d321c76a9d83cf8b4a845a41a8793e03b6f026964736f6c634300081e0033"));

        Assert.True(byteCode.HasError(Bytes4.Parse("0x00bfc921")));
    }

    [Fact]
    public void HasError_Should_Match_Leading_Zero_Selector_In_Push31_Word()
    {
        byte[] bytes = [0x7E, 0xBF, 0xC9, 0x21, .. new byte[28]];
        var byteCode = new EVMByteCode(bytes);

        Assert.True(byteCode.HasError(Bytes4.Parse("0x00bfc921")));
    }

    [Fact]
    public void HasError_Should_Match_Direct_Push4_Selector()
    {
        var byteCode = CreateByteCode(0x63, 0x13, 0xBE, 0x25, 0x2B, 0x5F, 0x52, 0x60, 0x04, 0x60, 0x1C, 0xFD);

        Assert.True(byteCode.HasError(Bytes4.Parse("0x13be252b")));
    }

    [Fact]
    public void HasError_Should_Match_Right_Shifted_Selector_In_Push_Data()
    {
        var byteCode = CreateByteCode(0x63, 0x10, 0x09, 0xA9, 0xAD, 0x60, 0xE1, 0x1B);

        Assert.True(byteCode.HasError(Bytes4.Parse("0x2013535a")));
    }

    [Fact]
    public void HasError_Should_Match_Optimized_Shifted_Non_Leading_Zero_Selector()
    {
        var byteCode = CreateByteCode(0x63, 0x12, 0x34, 0x56, 0x78, 0x60, 0xE0, 0x1B);

        Assert.True(byteCode.HasError(Bytes4.Parse("0x12345678")));
    }

    [Fact]
    public void HasFunction_Should_Match_Leading_Zero_Selector_When_Compiler_Uses_Short_Push()
    {
        var byteCode = CreateByteCode(0x62, 0x4A, 0xA3, 0x20, 0x14);

        Assert.True(byteCode.HasFunction(Convert.FromHexString("004aa320")));
    }

    [Fact]
    public void HasEvent_Should_Match_Topic_In_Executable_Bytecode()
    {
        byte[] topic = Convert.FromHexString("8be0079c531659141344cd1fd0a4f28419497f9722a3daafe3b4186f6b6457e0");
        byte[] bytes = [0xFE, .. topic];
        var byteCode = new EVMByteCode(bytes);

        Assert.True(byteCode.HasEvent(topic));
    }

    [Fact]
    public void HasEvent_Should_Match_Leading_Zero_Topic_In_Short_Push()
    {
        byte[] topic = Convert.FromHexString("004e8d79e4b41c5fad7561dc7c07786ee4e52292da7a3f5dc7ab90e32cc30423");
        byte[] bytes = [0x7E, .. topic[1..]];
        var byteCode = new EVMByteCode(bytes);

        Assert.True(byteCode.HasEvent(topic));
    }

    [Fact]
    public void HasFunction_Should_Ignore_Selector_Inside_Compiler_Metadata()
    {
        var byteCode = CreateByteCodeWithSolcMetadata(0x63, 0x12, 0x34, 0x56, 0x78, 0x14);

        Assert.False(byteCode.HasFunction(Convert.FromHexString("12345678")));
    }

    [Fact]
    public void HasError_Should_Ignore_Selector_Inside_Compiler_Metadata()
    {
        var byteCode = CreateByteCodeWithSolcMetadata(0x62, 0xBF, 0xC9, 0x21, 0x60, 0xE0, 0x1B);

        Assert.False(byteCode.HasError(Bytes4.Parse("0x00bfc921")));
    }

    [Fact]
    public void HasEvent_Should_Ignore_Topic_Inside_Compiler_Metadata()
    {
        byte[] topic = Convert.FromHexString("8be0079c531659141344cd1fd0a4f28419497f9722a3daafe3b4186f6b6457e0");
        byte[] metadataBytes = [0x7F, .. topic];
        var byteCode = CreateByteCodeWithSolcMetadata(metadataBytes);

        Assert.False(byteCode.HasEvent(topic));
    }
}
