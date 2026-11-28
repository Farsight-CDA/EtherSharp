using EtherSharp.Client.Modules.Blocks;
using EtherSharp.Client.Modules.Debug;
using EtherSharp.Client.Modules.Ether;
using EtherSharp.Client.Modules.Events;
using EtherSharp.Client.Modules.Mempool;
using EtherSharp.Client.Modules.Trace;
using EtherSharp.Contract;
using EtherSharp.Numerics;
using EtherSharp.Query;
using EtherSharp.RPC.Transport;
using EtherSharp.Tx;
using EtherSharp.Tx.Types;
using EtherSharp.Types;
using EtherSharp.Wallet;

namespace EtherSharp.Client;

internal sealed class EtherTxClient(EtherClient baseClient, TxSender sender) : IEtherTxClient
{
    private readonly EtherClient _baseClient = baseClient;
    private readonly TxSender _sender = sender;

    private IEtherClient BaseClient => _baseClient;

    Address IEtherTxClient.Address => _sender.Address;
    bool IEtherClient.IsInitialized => BaseClient.IsInitialized;
    ulong IEtherClient.ChainId => BaseClient.ChainId;
    IEtherModule IEtherClient.ETH => BaseClient.ETH;
    IBlocksModule IEtherClient.Blocks => BaseClient.Blocks;
    IMempoolModule IEtherClient.Mempool => BaseClient.Mempool;
    IDebugModule IEtherClient.Debug => BaseClient.Debug;
    ITraceModule IEtherClient.Trace => BaseClient.Trace;

    IInternalEtherClient IEtherClient.AsInternal()
        => BaseClient.AsInternal();

    EtherTxClientBuilder IEtherClient.Attach(IEtherSigner signer)
        => BaseClient.Attach(signer);

    Task IEtherClient.InitializeAsync(bool forceNoQuery, RpcRequestOptions requestOptions, CancellationToken cancellationToken)
        => BaseClient.InitializeAsync(forceNoQuery, requestOptions, cancellationToken);

    Task<T> IEtherClient.InitializeAsync<T>(IQuery<T> initQuery, RpcRequestOptions requestOptions, CancellationToken cancellationToken)
        => BaseClient.InitializeAsync(initQuery, requestOptions, cancellationToken);

    Task<T1> IEtherClient.QueryAsync<T1>(
        IQuery<T1> c1, ulong? gasLimit, in CallOptions options,
        RpcRequestOptions requestOptions, CancellationToken cancellationToken
    ) => BaseClient.QueryAsync(c1, gasLimit, in options, requestOptions, cancellationToken);

    Task<(T1, T2)> IEtherClient.QueryAsync<T1, T2>(
        IQuery<T1> c1, IQuery<T2> c2, ulong? gasLimit, in CallOptions options,
        RpcRequestOptions requestOptions, CancellationToken cancellationToken
    ) => BaseClient.QueryAsync(c1, c2, gasLimit, in options, requestOptions, cancellationToken);

    Task<(T1, T2, T3)> IEtherClient.QueryAsync<T1, T2, T3>(
        IQuery<T1> c1, IQuery<T2> c2, IQuery<T3> c3, ulong? gasLimit, in CallOptions options,
        RpcRequestOptions requestOptions, CancellationToken cancellationToken
    ) => BaseClient.QueryAsync(c1, c2, c3, gasLimit, in options, requestOptions, cancellationToken);

    Task<(T1, T2, T3, T4)> IEtherClient.QueryAsync<T1, T2, T3, T4>(
        IQuery<T1> c1, IQuery<T2> c2, IQuery<T3> c3, IQuery<T4> c4, ulong? gasLimit, in CallOptions options,
        RpcRequestOptions requestOptions, CancellationToken cancellationToken
    ) => BaseClient.QueryAsync(c1, c2, c3, c4, gasLimit, in options, requestOptions, cancellationToken);

    Task<(T1, T2, T3, T4, T5)> IEtherClient.QueryAsync<T1, T2, T3, T4, T5>(
        IQuery<T1> c1, IQuery<T2> c2, IQuery<T3> c3, IQuery<T4> c4, IQuery<T5> c5, ulong? gasLimit, in CallOptions options,
        RpcRequestOptions requestOptions, CancellationToken cancellationToken
    ) => BaseClient.QueryAsync(c1, c2, c3, c4, c5, gasLimit, in options, requestOptions, cancellationToken);

    IEventsModule<TEvent> IEtherClient.Events<TEvent>()
        => BaseClient.Events<TEvent>();

    IConfiguredEventsModule<TEvent> IEtherClient.Events<TEvent>(in EventFilter eventFilter)
        => BaseClient.Events<TEvent>(in eventFilter);

    Task<TxData?> IEtherClient.GetTransactionAsync(in Bytes32 hash, RpcRequestOptions requestOptions, CancellationToken cancellationToken)
        => BaseClient.GetTransactionAsync(in hash, requestOptions, cancellationToken);

    Task<TxReceipt?> IEtherClient.GetTransactionReceiptAsync(in Bytes32 hash, RpcRequestOptions requestOptions, CancellationToken cancellationToken)
        => BaseClient.GetTransactionReceiptAsync(in hash, requestOptions, cancellationToken);

    Task<ulong> IEtherClient.GetTransactionCount(
        in Address address, TargetHeight targetHeight, RpcRequestOptions requestOptions, CancellationToken cancellationToken
    ) => BaseClient.GetTransactionCount(in address, targetHeight, requestOptions, cancellationToken);

    Task<byte[]> IEtherClient.GetStorageAtAsync(
        in Address address, byte[] slot, TargetHeight targetHeight, RpcRequestOptions requestOptions, CancellationToken cancellationToken
    ) => BaseClient.GetStorageAtAsync(in address, slot, targetHeight, requestOptions, cancellationToken);

    Task<byte[]> IEtherClient.GetStorageAtAsync(
        IEVMContract contract, byte[] slot, TargetHeight targetHeight, RpcRequestOptions requestOptions, CancellationToken cancellationToken
    ) => BaseClient.GetStorageAtAsync(contract, slot, targetHeight, requestOptions, cancellationToken);

    TContract IEtherClient.Contract<TContract>(in Address address)
        => BaseClient.Contract<TContract>(in address);

    Task<CallResult<T>> IEtherClient.SafeCallAsync<T>(
        ITxInput<T> call, in CallOptions options, RpcRequestOptions requestOptions, CancellationToken cancellationToken
    ) => options.From is null
        ? BaseClient.SafeCallAsync(call, options with { From = _sender.Address }, requestOptions, cancellationToken)
        : BaseClient.SafeCallAsync(call, in options, requestOptions, cancellationToken);

    Task<T> IEtherClient.CallAsync<T>(
        ITxInput<T> call, in CallOptions options, RpcRequestOptions requestOptions, CancellationToken cancellationToken
    ) => options.From is null
        ? BaseClient.CallAsync(call, options with { From = _sender.Address }, requestOptions, cancellationToken)
        : BaseClient.CallAsync(call, in options, requestOptions, cancellationToken);

    Task<CallResult<T>> IEtherClient.SafeFlashCallAsync<T>(
        EVMRuntimeCode runtimeCode, IFlashCall<T> call, ulong? flashCallGasLimit,
        in CallOptions options, RpcRequestOptions requestOptions, CancellationToken cancellationToken
    ) => BaseClient.SafeFlashCallAsync(runtimeCode, call, flashCallGasLimit, in options, requestOptions, cancellationToken);

    Task<T> IEtherClient.FlashCallAsync<T>(
        EVMRuntimeCode runtimeCode, IFlashCall<T> call, ulong? flashCallGasLimit,
        in CallOptions options, RpcRequestOptions requestOptions, CancellationToken cancellationToken
    ) => BaseClient.FlashCallAsync(runtimeCode, call, flashCallGasLimit, in options, requestOptions, cancellationToken);

    Task<FeeHistory> IEtherClient.GetFeeHistoryAsync(
        int blockCount, TargetHeight newestBlock, double[] rewardPercentiles,
        RpcRequestOptions requestOptions, CancellationToken cancellationToken
    ) => BaseClient.GetFeeHistoryAsync(blockCount, newestBlock, rewardPercentiles, requestOptions, cancellationToken);

    Task<UInt256> IEtherClient.GetGasPriceAsync(RpcRequestOptions requestOptions, CancellationToken cancellationToken)
        => BaseClient.GetGasPriceAsync(requestOptions, cancellationToken);

    Task<UInt256> IEtherClient.GetMaxPriorityFeePerGasAsync(RpcRequestOptions requestOptions, CancellationToken cancellationToken)
        => BaseClient.GetMaxPriorityFeePerGasAsync(requestOptions, cancellationToken);

    void IEtherClient.SetDefaultCallGasLimits(ulong? ethCallGasLimit, ulong? flashCallGasLimit)
        => BaseClient.SetDefaultCallGasLimits(ethCallGasLimit, flashCallGasLimit);

    Task<ulong> IEtherClient.EstimateGasLimitAsync(
        ITxInput call, in CallOptions options, RpcRequestOptions requestOptions, CancellationToken cancellationToken
    ) => options.From is null
        ? BaseClient.EstimateGasLimitAsync(call, options with { From = _sender.Address }, requestOptions, cancellationToken)
        : BaseClient.EstimateGasLimitAsync(call, in options, requestOptions, cancellationToken);

    Task<AccessListResult> IEtherClient.CreateAccessListAsync(
        ITxInput call, in CallOptions options, RpcRequestOptions requestOptions, CancellationToken cancellationToken
    ) => options.From is null
        ? BaseClient.CreateAccessListAsync(call, options with { From = _sender.Address }, requestOptions, cancellationToken)
        : BaseClient.CreateAccessListAsync(call, in options, requestOptions, cancellationToken);

    Task<TTxGasParams> IEtherClient.EstimateTxGasParamsAsync<TTxParams, TTxGasParams>(
        ITxInput call, TTxParams? txParams, Address? from, RpcRequestOptions requestOptions, CancellationToken cancellationToken
    ) where TTxParams : class
        => BaseClient.EstimateTxGasParamsAsync<TTxParams, TTxGasParams>(
            call, txParams, from ?? _sender.Address, requestOptions, cancellationToken
        );

    Task<IPendingTx<TTxParams, TTxGasParams>> IEtherTxClient.PrepareTxAsync<TTransaction, TTxParams, TTxGasParams>(
        ITxInput call, TTxParams? txParams, TTxGasParams? txGasParams,
        RpcRequestOptions requestOptions, CancellationToken cancellationToken
    )
        where TTxParams : class
        where TTxGasParams : class
    {
        ArgumentNullException.ThrowIfNull(call);
        return PrepareTxCoreAsync<TTransaction, TTxParams, TTxGasParams>(
            replaceTx: null, call, txParams, txGasParams, requestOptions, cancellationToken
        );
    }

    Task<IPendingTx<TTxParams, TTxGasParams>> IEtherTxClient.ReplaceTxAsync<TTransaction, TTxParams, TTxGasParams>(
        IPendingTx pendingTx, ITxInput? call, TTxParams? txParams, TTxGasParams? txGasParams,
        RpcRequestOptions requestOptions, CancellationToken cancellationToken
    )
        where TTxParams : class
        where TTxGasParams : class
    {
        ArgumentNullException.ThrowIfNull(pendingTx);
        var resolvedCall = call ?? pendingTx.Call;
        var resolvedTxParams = txParams ?? (
            call is null && pendingTx is IPendingTx<TTxParams, TTxGasParams> sameTyped
                ? sameTyped.LatestSubmission.Params
                : TTxParams.Default
        );

        return PrepareTxCoreAsync<TTransaction, TTxParams, TTxGasParams>(
            replaceTx: pendingTx, resolvedCall, resolvedTxParams, txGasParams, requestOptions, cancellationToken
        );
    }

    private async Task<IPendingTx<TTxParams, TTxGasParams>> PrepareTxCoreAsync<TTransaction, TTxParams, TTxGasParams>(
        IPendingTx? replaceTx, ITxInput call, TTxParams? txParams, TTxGasParams? txGasParams,
        RpcRequestOptions requestOptions, CancellationToken cancellationToken
    )
        where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams<TTxGasParams>
    {
        _sender.ThrowIfDisposed();

        txParams ??= TTxParams.Default;
        if(txGasParams is null)
        {
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_sender.DisposalToken, cancellationToken);
            txGasParams = await BaseClient.EstimateTxGasParamsAsync<TTxParams, TTxGasParams>(
                call, txParams, _sender.Address, requestOptions, linkedCts.Token
            );
        }

        var pendingTx = await _sender.PrepareAsync<TTransaction, TTxParams, TTxGasParams>(
            replaceTx?.Id, call, txParams, txGasParams, requestOptions, cancellationToken
        );

        if(replaceTx is not null)
        {
            await replaceTx.DisposeAsync();
        }

        return pendingTx;
    }

    async Task<IPendingTx<TTxParams, TTxGasParams>?> IEtherTxClient.AttachPendingTxAsync<TTransaction, TTxParams, TTxGasParams>(
        Guid id, CancellationToken cancellationToken
    ) => await _sender.AttachAsync<TTransaction, TTxParams, TTxGasParams>(id, cancellationToken);

    public async ValueTask DisposeAsync()
    {
        _baseClient.UnregisterTxClient(this);
        await _sender.DisposeAsync();
    }
}
