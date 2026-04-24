using EtherSharp.ABI;
using EtherSharp.Common.Exceptions;
using EtherSharp.Types;

namespace EtherSharp.Tests.Common.Exceptions;

public sealed class CallRevertedExceptionTests
{
    [Fact]
    public void Parse_With_No_Data_Includes_Target_Address()
    {
        var address = Address.FromString("0x4838b106fce9647bdf1e7877bf73ce8b0bad5f97");

        var exception = CallRevertedException.Parse(address, []);

        Assert.IsType<CallRevertedException.CallRevertedWithNoDataException>(exception);
        Assert.Equal(address, exception.CallAddress);
        Assert.Equal("Contract call to 0x4838b106fce9647bdf1e7877bf73ce8b0bad5f97 reverted: no revert data was returned.", exception.Message);
    }

    [Fact]
    public void Parse_With_Error_String_Includes_Decoded_Message_And_Raw_Data()
    {
        byte[] data = [0x08, 0xc3, 0x79, 0xa0, .. new AbiEncoder().String("insufficient balance").Build()];

        var exception = Assert.IsType<CallRevertedException.CallRevertedWithMessageException>(
            CallRevertedException.Parse(null, data)
        );

        Assert.Equal("insufficient balance", exception.ContractErrorMessage);
        Assert.Contains("Solidity Error(string) returned 'insufficient balance'", exception.Message);
        Assert.Contains($"Revert data: 0x{Convert.ToHexStringLower(data)}.", exception.Message);
    }

    [Fact]
    public void Parse_With_Panic_Includes_Code_Name_And_Raw_Data()
    {
        byte[] data = [0x4e, 0x48, 0x7b, 0x71, .. new byte[31], (byte) PanicType.DivideByZero];

        var exception = Assert.IsType<CallRevertedException.CallRevertedWithPanicException>(
            CallRevertedException.Parse(null, data)
        );

        Assert.Equal(PanicType.DivideByZero, exception.PanicType);
        Assert.Contains("Solidity Panic(0x12) DivideByZero", exception.Message);
        Assert.Contains($"Revert data: 0x{Convert.ToHexStringLower(data)}.", exception.Message);
    }

    [Fact]
    public void Parse_With_Custom_Error_Includes_Selector_And_Raw_Data()
    {
        byte[] data = [0xde, 0xad, 0xbe, 0xef, 0x01, 0x02];

        var exception = Assert.IsType<CallRevertedException.CallRevertedWithCustomErrorException>(
            CallRevertedException.Parse(null, data)
        );

        Assert.Equal(data, exception.ContractErrorData);
        Assert.Contains("custom error selector 0xdeadbeef", exception.Message);
        Assert.Contains("Revert data: 0xdeadbeef0102.", exception.Message);
    }

    [Fact]
    public void Parse_With_Short_Data_Does_Not_Throw()
    {
        byte[] data = [0x01, 0x02, 0x03];

        var exception = Assert.IsType<CallRevertedException.CallRevertedWithCustomErrorException>(
            CallRevertedException.Parse(null, data)
        );

        Assert.Equal(data, exception.ContractErrorData);
        Assert.Contains("unrecognized revert payload shorter than a 4-byte selector (3 bytes)", exception.Message);
        Assert.Contains("Revert data: 0x010203.", exception.Message);
    }
}
