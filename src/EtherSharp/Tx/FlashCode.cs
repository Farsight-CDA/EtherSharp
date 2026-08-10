using EtherSharp.Contract;

namespace EtherSharp.Tx;

internal sealed class FlashCode : IFlashCode
{
    public EVMByteCode ByteCode { get; }
    public bool IsRuntimeCode { get; }

    public FlashCode(EVMByteCode byteCode, bool isRuntimeCode)
    {
        if(!isRuntimeCode)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(byteCode.Length, EVMByteCode.MAX_INIT_LENGTH);
        }

        ByteCode = byteCode;
        IsRuntimeCode = isRuntimeCode;
    }
}
