using EtherSharp.Contract;
using System.Diagnostics.CodeAnalysis;

namespace EtherSharp.Tx;

internal sealed class FlashCode(EVMInitCode? initCode, EVMRuntimeCode? runtimeCode) : IFlashCode
{
    private readonly EVMInitCode? _initCode = initCode;
    private readonly EVMRuntimeCode? _runtimeCode = runtimeCode;

    public bool TryGetRuntimeCode([MaybeNullWhen(false)] out EVMRuntimeCode runtimeCode)
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

    public EVMInitCode GetInitCode()
        => _initCode ?? IFlashCode.CreateInitCode(_runtimeCode!);
}
