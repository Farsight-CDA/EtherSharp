using EtherSharp.Contract;
using EtherSharp.Tx;
using EtherSharp.Types;

namespace EtherSharp.Client.Services.FlashCall;

internal sealed class FlashCallExecutor(
    IFlashInitCodeExecutor initCodeExecutor,
    CallGasLimitSettings callGasLimitSettings,
    IFlashRuntimeExecutor? runtimeExecutor = null)
{
    private readonly IFlashInitCodeExecutor _initCodeExecutor = initCodeExecutor;
    private readonly IFlashRuntimeExecutor? _runtimeExecutor = runtimeExecutor;
    private readonly CallGasLimitSettings _callGasLimitSettings = callGasLimitSettings;

    public int GetMaxPayloadSize(IFlashCode code, ulong? flashCallGasLimit, TargetHeight targetHeight)
    {
        ulong? resolvedGasLimit = flashCallGasLimit ?? _callGasLimitSettings.GetFlashCallGasLimit();
        return code.IsRuntimeCode switch
        {
            true => _runtimeExecutor is not null
                ? _runtimeExecutor.GetMaxPayloadSize(resolvedGasLimit, targetHeight)
                : _initCodeExecutor.GetMaxPayloadSize(IFlashCode.GetInitCodeLength(code.ByteCode), resolvedGasLimit, targetHeight),
            false => _initCodeExecutor.GetMaxPayloadSize(code.ByteCode.Length, resolvedGasLimit, targetHeight)
        };
    }

    public int GetMaxResultSize(IFlashCode code, TargetHeight targetHeight)
        => code.IsRuntimeCode && _runtimeExecutor is not null
            ? _runtimeExecutor.GetMaxResultSize(targetHeight)
            : _initCodeExecutor.GetMaxResultSize(targetHeight);

    public Task<TxCallResult> ExecuteFlashCallAsync(
        IFlashCode code,
        IFlashCall call,
        ulong? flashCallGasLimit,
        CallOptions options,
        CancellationToken cancellationToken)
    {
        ulong? resolvedGasLimit = flashCallGasLimit ?? _callGasLimitSettings.GetFlashCallGasLimit();

        if(code.IsRuntimeCode && _runtimeExecutor is not null)
        {
            return _runtimeExecutor.ExecuteFlashCallAsync(code.ByteCode, call, resolvedGasLimit, options, cancellationToken);
        }

        var initCode = code.IsRuntimeCode
            ? IFlashCode.CreateInitCode(code.ByteCode)
            : code.ByteCode;

        if(initCode.Length > EVMByteCode.MAX_INIT_LENGTH)
        {
            throw new InvalidOperationException($"Maximum initcode length exceeded, {initCode.Length} > {EVMByteCode.MAX_INIT_LENGTH}");
        }
        if(code is IContractDeployment deployment && deployment.Value > 0)
        {
            throw new NotSupportedException("Contract deployment cannot contain any value");
        }
        //
        return _initCodeExecutor.ExecuteFlashCallAsync(initCode, call, resolvedGasLimit, options, cancellationToken);
    }
}
