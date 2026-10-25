using EtherSharp.Contract;
using System.Diagnostics.CodeAnalysis;

namespace EtherSharp.Tx;

internal sealed class FlashCode(EVMByteCode? initCode, EVMByteCode? runtimeCode) : IFlashCode
{
    private readonly EVMByteCode? _initCode = initCode;
    private readonly EVMByteCode? _runtimeCode = runtimeCode;

    public bool TryGetRuntimeCode([MaybeNullWhen(false)] out EVMByteCode runtimeCode)
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
        => _initCode?.Length ?? IFlashCode.GetInitCodeLength(_runtimeCode!);

    public EVMByteCode GetInitCode()
        => _initCode ?? IFlashCode.CreateInitCode(_runtimeCode!);
}
