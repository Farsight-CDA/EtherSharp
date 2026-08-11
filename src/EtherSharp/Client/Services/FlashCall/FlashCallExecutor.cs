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
        return _runtimeExecutor is not null && code.TryGetRuntimeCode(out _)
            ? _runtimeExecutor.GetMaxPayloadSize(resolvedGasLimit, targetHeight)
            : _initCodeExecutor.GetMaxPayloadSize(code.GetInitCodeLength(), resolvedGasLimit, targetHeight);
    }

    public int GetMaxResultSize(IFlashCode code, TargetHeight targetHeight)
        => _runtimeExecutor is not null && code.TryGetRuntimeCode(out _)
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

        if(_runtimeExecutor is not null && code.TryGetRuntimeCode(out var runtimeCode))
        {
            return _runtimeExecutor.ExecuteFlashCallAsync(runtimeCode, call, resolvedGasLimit, options, cancellationToken);
        }

        var initCode = code.GetInitCode();

        if(code is IContractDeployment deployment && deployment.Value > 0)
        {
            throw new NotSupportedException("Contract deployment cannot contain any value");
        }
        //
        return _initCodeExecutor.ExecuteFlashCallAsync(initCode, call, resolvedGasLimit, options, cancellationToken);
    }
}
