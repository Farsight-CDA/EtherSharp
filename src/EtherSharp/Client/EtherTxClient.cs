using EtherSharp.Client.Modules.Blocks;
using EtherSharp.Client.Modules.Debug;
using EtherSharp.Client.Modules.Ether;
using EtherSharp.Client.Modules.Events;
using EtherSharp.Client.Modules.Mempool;
using EtherSharp.Client.Modules.Trace;
using EtherSharp.Client.Services.TxScheduler;
using EtherSharp.Contract;
using EtherSharp.Numerics;
using EtherSharp.Query;
using EtherSharp.Realtime.Events;
using EtherSharp.RPC.Transport;
using EtherSharp.Tx;
using EtherSharp.Tx.PendingHandler;
using EtherSharp.Types;
using EtherSharp.Wallet;

namespace EtherSharp.Client;

internal sealed class EtherTxClient(
    EtherClient baseClient,
    IEtherSigner signer,
    BlockingSequentialTxScheduler txScheduler
) : IEtherTxClient
{
    private readonly EtherClient _ownerClient = baseClient;
    private readonly IEtherClient _baseClient = baseClient;
    private readonly IEtherSigner _signer = signer;
    private readonly BlockingSequentialTxScheduler _txScheduler = txScheduler;

    private int _isDisposed;

    bool IEtherClient.IsInitialized => _baseClient.IsInitialized;
    ulong IEtherClient.ChainId => _baseClient.ChainId;
    IEtherModule IEtherClient.ETH => _baseClient.ETH;
    IBlocksModule IEtherClient.Blocks => _baseClient.Blocks;
    IMempoolModule IEtherClient.Mempool => _baseClient.Mempool;
    IDebugModule IEtherClient.Debug => _baseClient.Debug;
    ITraceModule IEtherClient.Trace => _baseClient.Trace;

    IInternalEtherClient IEtherClient.AsInternal()
        => _baseClient.AsInternal();

    EtherTxClientBuilder IEtherClient.Attach(IEtherSigner signer)
        => _baseClient.Attach(signer);

    Task IEtherClient.InitializeAsync(bool forceNoQuery, RpcRequestOptions requestOptions, CancellationToken cancellationToken)
        => _baseClient.InitializeAsync(forceNoQuery, requestOptions, cancellationToken);

    Task<T> IEtherClient.InitializeAsync<T>(IQuery<T> initQuery, RpcRequestOptions requestOptions, CancellationToken cancellationToken)
        => _baseClient.InitializeAsync(initQuery, requestOptions, cancellationToken);

    Task<T1> IEtherClient.QueryAsync<T1>(
        IQuery<T1> c1, ulong? gasLimit, in CallOptions options,
        RpcRequestOptions requestOptions, CancellationToken cancellationToken
    ) => _baseClient.QueryAsync(c1, gasLimit, in options, requestOptions, cancellationToken);

    Task<(T1, T2)> IEtherClient.QueryAsync<T1, T2>(
        IQuery<T1> c1, IQuery<T2> c2, ulong? gasLimit, in CallOptions options,
        RpcRequestOptions requestOptions, CancellationToken cancellationToken
    ) => _baseClient.QueryAsync(c1, c2, gasLimit, in options, requestOptions, cancellationToken);

    Task<(T1, T2, T3)> IEtherClient.QueryAsync<T1, T2, T3>(
        IQuery<T1> c1, IQuery<T2> c2, IQuery<T3> c3, ulong? gasLimit, in CallOptions options,
        RpcRequestOptions requestOptions, CancellationToken cancellationToken
    ) => _baseClient.QueryAsync(c1, c2, c3, gasLimit, in options, requestOptions, cancellationToken);

    Task<(T1, T2, T3, T4)> IEtherClient.QueryAsync<T1, T2, T3, T4>(
        IQuery<T1> c1, IQuery<T2> c2, IQuery<T3> c3, IQuery<T4> c4, ulong? gasLimit, in CallOptions options,
        RpcRequestOptions requestOptions, CancellationToken cancellationToken
    ) => _baseClient.QueryAsync(c1, c2, c3, c4, gasLimit, in options, requestOptions, cancellationToken);

    Task<(T1, T2, T3, T4, T5)> IEtherClient.QueryAsync<T1, T2, T3, T4, T5>(
        IQuery<T1> c1, IQuery<T2> c2, IQuery<T3> c3, IQuery<T4> c4, IQuery<T5> c5, ulong? gasLimit, in CallOptions options,
        RpcRequestOptions requestOptions, CancellationToken cancellationToken
    ) => _baseClient.QueryAsync(c1, c2, c3, c4, c5, gasLimit, in options, requestOptions, cancellationToken);

    IEventsModule<TEvent> IEtherClient.Events<TEvent>()
        => _baseClient.Events<TEvent>();

    IConfiguredEventsModule<TEvent> IEtherClient.Events<TEvent>(in EventFilter eventFilter)
        => _baseClient.Events<TEvent>(in eventFilter);

    Task<TxData?> IEtherClient.GetTransactionAsync(in Bytes32 hash, RpcRequestOptions requestOptions, CancellationToken cancellationToken)
        => _baseClient.GetTransactionAsync(in hash, requestOptions, cancellationToken);

    Task<TxReceipt?> IEtherClient.GetTransactionReceiptAsync(in Bytes32 hash, RpcRequestOptions requestOptions, CancellationToken cancellationToken)
        => _baseClient.GetTransactionReceiptAsync(in hash, requestOptions, cancellationToken);

    Task<ulong> IEtherClient.GetTransactionCount(
        in Address address, TargetHeight targetHeight, RpcRequestOptions requestOptions, CancellationToken cancellationToken
    ) => _baseClient.GetTransactionCount(in address, targetHeight, requestOptions, cancellationToken);

    Task<byte[]> IEtherClient.GetStorageAtAsync(
        in Address address, byte[] slot, TargetHeight targetHeight, RpcRequestOptions requestOptions, CancellationToken cancellationToken
    ) => _baseClient.GetStorageAtAsync(in address, slot, targetHeight, requestOptions, cancellationToken);

    Task<byte[]> IEtherClient.GetStorageAtAsync(
        IEVMContract contract, byte[] slot, TargetHeight targetHeight, RpcRequestOptions requestOptions, CancellationToken cancellationToken
    ) => _baseClient.GetStorageAtAsync(contract, slot, targetHeight, requestOptions, cancellationToken);

    TContract IEtherClient.Contract<TContract>(in Address address)
        => _baseClient.Contract<TContract>(in address);

    Task<CallResult<T>> IEtherClient.SafeCallAsync<T>(
        ITxInput<T> call, in CallOptions options, RpcRequestOptions requestOptions, CancellationToken cancellationToken
    ) => options.From is null
        ? _baseClient.SafeCallAsync(call, options with { From = _signer.Address }, requestOptions, cancellationToken)
        : _baseClient.SafeCallAsync(call, in options, requestOptions, cancellationToken);

    Task<T> IEtherClient.CallAsync<T>(
        ITxInput<T> call, in CallOptions options, RpcRequestOptions requestOptions, CancellationToken cancellationToken
    ) => options.From is null
        ? _baseClient.CallAsync(call, options with { From = _signer.Address }, requestOptions, cancellationToken)
        : _baseClient.CallAsync(call, in options, requestOptions, cancellationToken);

    Task<CallResult<T>> IEtherClient.SafeFlashCallAsync<T>(
        EVMRuntimeCode runtimeCode, IFlashCall<T> call, ulong? flashCallGasLimit,
        in CallOptions options, RpcRequestOptions requestOptions, CancellationToken cancellationToken
    ) => _baseClient.SafeFlashCallAsync(runtimeCode, call, flashCallGasLimit, in options, requestOptions, cancellationToken);

    Task<T> IEtherClient.FlashCallAsync<T>(
        EVMRuntimeCode runtimeCode, IFlashCall<T> call, ulong? flashCallGasLimit,
        in CallOptions options, RpcRequestOptions requestOptions, CancellationToken cancellationToken
    ) => _baseClient.FlashCallAsync(runtimeCode, call, flashCallGasLimit, in options, requestOptions, cancellationToken);

    Task<FeeHistory> IEtherClient.GetFeeHistoryAsync(
        int blockCount, TargetHeight newestBlock, double[] rewardPercentiles,
        RpcRequestOptions requestOptions, CancellationToken cancellationToken
    ) => _baseClient.GetFeeHistoryAsync(blockCount, newestBlock, rewardPercentiles, requestOptions, cancellationToken);

    Task<UInt256> IEtherClient.GetGasPriceAsync(RpcRequestOptions requestOptions, CancellationToken cancellationToken)
        => _baseClient.GetGasPriceAsync(requestOptions, cancellationToken);

    Task<UInt256> IEtherClient.GetMaxPriorityFeePerGasAsync(RpcRequestOptions requestOptions, CancellationToken cancellationToken)
        => _baseClient.GetMaxPriorityFeePerGasAsync(requestOptions, cancellationToken);

    void IEtherClient.SetDefaultCallGasLimits(ulong? ethCallGasLimit, ulong? flashCallGasLimit)
        => _baseClient.SetDefaultCallGasLimits(ethCallGasLimit, flashCallGasLimit);

    Task<ulong> IEtherClient.EstimateGasLimitAsync(
        ITxInput call, in CallOptions options, RpcRequestOptions requestOptions, CancellationToken cancellationToken
    ) => options.From is null
        ? _baseClient.EstimateGasLimitAsync(call, options with { From = _signer.Address }, requestOptions, cancellationToken)
        : _baseClient.EstimateGasLimitAsync(call, in options, requestOptions, cancellationToken);

    Task<AccessListResult> IEtherClient.CreateAccessListAsync(
        ITxInput call, in CallOptions options, RpcRequestOptions requestOptions, CancellationToken cancellationToken
    ) => options.From is null
        ? _baseClient.CreateAccessListAsync(call, options with { From = _signer.Address }, requestOptions, cancellationToken)
        : _baseClient.CreateAccessListAsync(call, in options, requestOptions, cancellationToken);

    Task<TTxGasParams> IEtherClient.EstimateTxGasParamsAsync<TTxParams, TTxGasParams>(
        ITxInput call, TTxParams? txParams, Address? from, RpcRequestOptions requestOptions, CancellationToken cancellationToken
    ) where TTxParams : class
        => _baseClient.EstimateTxGasParamsAsync<TTxParams, TTxGasParams>(
            call, txParams, from ?? _signer.Address, requestOptions, cancellationToken
        );

    async Task<IPendingTxHandler<TTxParams, TTxGasParams>> IEtherTxClient.PrepareTxAsync<TTransaction, TTxParams, TTxGasParams>(
        ITxInput call, TTxParams? txParams, TTxGasParams? txGasParams,
        RpcRequestOptions requestOptions, CancellationToken cancellationToken
    )
        where TTxParams : class
        where TTxGasParams : class
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _isDisposed) != 0, this);
        return await _txScheduler.PrepareTxAsync<TTransaction, TTxParams, TTxGasParams>(
            call, txParams, txGasParams, requestOptions, cancellationToken
        );
    }

    async Task<IPendingTxHandler<TTxParams, TTxGasParams>> IEtherTxClient.AttachPendingTxAsync<TTransaction, TTxParams, TTxGasParams>(
        Guid id, CancellationToken cancellationToken
    )
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _isDisposed) != 0, this);
        return await _txScheduler.AttachPendingTxAsync<TTransaction, TTxParams, TTxGasParams>(id, cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if(Interlocked.Exchange(ref _isDisposed, 1) != 0)
        {
            return;
        }

        _ownerClient.UnregisterTxClient(this);
        await _txScheduler.DisposeAsync();
    }
}
