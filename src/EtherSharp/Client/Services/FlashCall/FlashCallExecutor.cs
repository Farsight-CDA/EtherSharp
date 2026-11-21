using EtherSharp.Contract;
using EtherSharp.RPC.Transport;
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

    public Task<TxCallResult> ExecuteFlashCallAsync(
        IFlashCode code,
        IFlashCall call,
        ulong? flashCallGasLimit,
        in CallOptions options,
        RpcRequestOptions requestOptions,
        CancellationToken cancellationToken)
    {
        if(_runtimeExecutor is null && options.StateOverrides is { Count: > 0 })
        {
            throw new NotSupportedException(
                "State overrides in queries and flash calls require WithFlashCalls(enableStateOverrides: true)."
            );
        }

        ulong? resolvedGasLimit = flashCallGasLimit ?? _callGasLimitSettings.GetFlashCallGasLimit();

        if(_runtimeExecutor is not null && code.TryGetRuntimeCode(out var runtimeCode))
        {
            return _runtimeExecutor.ExecuteFlashCallAsync(
                runtimeCode, call, resolvedGasLimit, options, requestOptions, cancellationToken);
        }

        var initCode = code.GetInitCode();

        if(code is IContractDeployment deployment && !deployment.Value.IsZero)
        {
            throw new NotSupportedException("Contract deployment cannot contain any value");
        }
        //
        return _initCodeExecutor.ExecuteFlashCallAsync(
            initCode, call, resolvedGasLimit, options, requestOptions, cancellationToken);
    }
}
