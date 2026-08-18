using EtherSharp.Contract;
using EtherSharp.RPC.Transport;
using EtherSharp.Tx;
using EtherSharp.Types;

namespace EtherSharp.Client.Services.FlashCall;

internal interface IFlashRuntimeExecutor
{
    public int GetMaxPayloadSize(ulong? flashCallGasLimit, TargetHeight targetHeight);
    public int GetMaxResultSize(TargetHeight targetHeight);

    public Task<TxCallResult> ExecuteFlashCallAsync(
        EVMByteCode runtimeCode,
        IFlashCall call,
        ulong? flashCallGasLimit,
        CallOptions options,
        RpcRequestOptions requestOptions,
        CancellationToken cancellationToken
    );
}
