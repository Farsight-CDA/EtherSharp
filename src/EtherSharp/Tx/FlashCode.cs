using EtherSharp.Contract;

namespace EtherSharp.Tx;

internal sealed class FlashCode(EVMByteCode? initCode, EVMByteCode? runtimeCode) : IFlashCode
{
    private readonly EVMByteCode? _initCode = initCode;
    private readonly EVMByteCode? _runtimeCode = runtimeCode;

    public bool TryGetRuntimeCode(out EVMByteCode runtimeCode)
    {
        if(_runtimeCode is not { } code)
        {
            runtimeCode = default;
            return false;
        }

        runtimeCode = code;
        return true;
    }

    public int GetInitCodeLength()
        => _initCode?.Length ?? IFlashCode.GetInitCodeLength(_runtimeCode!.Value);

    public EVMByteCode GetInitCode()
        => _initCode ?? IFlashCode.CreateInitCode(_runtimeCode!.Value);
}
