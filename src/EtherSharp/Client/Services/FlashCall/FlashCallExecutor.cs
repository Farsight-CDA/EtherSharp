using EtherSharp.Contract;
using EtherSharp.RPC.Transport;
using EtherSharp.Tx;
using EtherSharp.Types;

namespace EtherSharp.Client.Services.FlashCall;

/// <summary>
/// Executes runtime code at a temporary account through one of the flash call backends.
/// </summary>
internal closed class FlashCallExecutor
{
    /// <summary>
    /// Executes runtime code at a temporary account. <see cref="StateOverrideFlashCallExecutor"/> installs it at the
    /// code's flash call address; the other backends deploy it at an unspecified address.
    /// </summary>
    public abstract Task<TxCallResult> ExecuteAsync(
        EVMRuntimeCode runtimeCode,
        IFlashCall call,
        ulong? flashCallGasLimit,
        CallOptions options,
        RpcRequestOptions requestOptions,
        CancellationToken cancellationToken
    );
}
