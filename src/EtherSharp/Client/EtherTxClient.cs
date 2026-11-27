using EtherSharp.Client.Modules.Blocks;
using EtherSharp.Client.Modules.Debug;
using EtherSharp.Client.Modules.Ether;
using EtherSharp.Client.Modules.Events;
using EtherSharp.Client.Modules.Mempool;
using EtherSharp.Client.Modules.Trace;
using EtherSharp.Client.Services.ResiliencyLayer;
using EtherSharp.Client.Services.TxPublisher;
using EtherSharp.Contract;
using EtherSharp.Numerics;
using EtherSharp.Query;
using EtherSharp.RPC;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.RPC.Transport;
using EtherSharp.Tx;
using EtherSharp.Tx.Nonce;
using EtherSharp.Tx.Watcher;
using EtherSharp.Types;
using EtherSharp.Wallet;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EtherSharp.Client;

internal sealed class EtherTxClient(
    EtherClient baseClient,
    IEtherSigner signer,
    INonceAllocator nonceAllocator,
    ITxPublisher txPublisher
) : IEtherTxClient
{
    private readonly EtherClient _ownerClient = baseClient;
    private readonly IEtherClient _baseClient = baseClient;
    private readonly INonceAllocator _nonceAllocator = nonceAllocator;

    private readonly CancellationTokenSource _disposalCts = new CancellationTokenSource();
    private readonly SemaphoreSlim _preparationLock = new SemaphoreSlim(1, 1);
    private volatile bool _isDisposed;

    internal IEtherSigner Signer { get; } = signer;
    internal ITxPublisher TxPublisher { get; } = txPublisher;
    internal ITxConfirmationWatcher TxConfirmationWatcher { get; }
        = ((IEtherClient) baseClient).AsInternal().Provider.GetRequiredService<ITxConfirmationWatcher>();
    internal IEthRpcModule EthRpcModule { get; }
        = ((IEtherClient) baseClient).AsInternal().Provider.GetRequiredService<IEthRpcModule>();
    internal RpcRetryOptions RetryOptions { get; }
        = ((IEtherClient) baseClient).AsInternal().Provider.GetRequiredService<RpcRetryOptions>();
    internal IResiliencyLayer? ResiliencyLayer { get; }
        = ((IEtherClient) baseClient).AsInternal().Provider.GetService<IResiliencyLayer>();
    internal ILogger? Logger { get; }
        = ((IEtherClient) baseClient).AsInternal().Provider.GetService<ILoggerFactory>()?.CreateLogger<EtherTxClient>();

    Address IEtherTxClient.Address => Signer.Address;
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
        ? _baseClient.SafeCallAsync(call, options with { From = Signer.Address }, requestOptions, cancellationToken)
        : _baseClient.SafeCallAsync(call, in options, requestOptions, cancellationToken);

    Task<T> IEtherClient.CallAsync<T>(
        ITxInput<T> call, in CallOptions options, RpcRequestOptions requestOptions, CancellationToken cancellationToken
    ) => options.From is null
        ? _baseClient.CallAsync(call, options with { From = Signer.Address }, requestOptions, cancellationToken)
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
        ? _baseClient.EstimateGasLimitAsync(call, options with { From = Signer.Address }, requestOptions, cancellationToken)
        : _baseClient.EstimateGasLimitAsync(call, in options, requestOptions, cancellationToken);

    Task<AccessListResult> IEtherClient.CreateAccessListAsync(
        ITxInput call, in CallOptions options, RpcRequestOptions requestOptions, CancellationToken cancellationToken
    ) => options.From is null
        ? _baseClient.CreateAccessListAsync(call, options with { From = Signer.Address }, requestOptions, cancellationToken)
        : _baseClient.CreateAccessListAsync(call, in options, requestOptions, cancellationToken);

    Task<TTxGasParams> IEtherClient.EstimateTxGasParamsAsync<TTxParams, TTxGasParams>(
        ITxInput call, TTxParams? txParams, Address? from, RpcRequestOptions requestOptions, CancellationToken cancellationToken
    ) where TTxParams : class
        => _baseClient.EstimateTxGasParamsAsync<TTxParams, TTxGasParams>(
            call, txParams, from ?? Signer.Address, requestOptions, cancellationToken
        );

    async Task<IPendingTx<TTxParams, TTxGasParams>> IEtherTxClient.PrepareTxAsync<TTransaction, TTxParams, TTxGasParams>(
        ITxInput call, TTxParams? txParams, TTxGasParams? txGasParams,
        RpcRequestOptions requestOptions, CancellationToken cancellationToken
    )
        where TTxParams : class
        where TTxGasParams : class
    {
        ArgumentNullException.ThrowIfNull(call);
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        var disposalToken = _disposalCts.Token;
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(disposalToken, cancellationToken);

        txParams ??= TTxParams.Default;
        txGasParams ??= await _baseClient.EstimateTxGasParamsAsync<TTxParams, TTxGasParams>(
            call, txParams, Signer.Address, requestOptions, linkedCts.Token
        );

        // Serializes nonce allocation with signing so a failed or cancelled signature can always
        // release its nonce, instead of leaving a gap below nonces already handed to concurrent callers.
        await _preparationLock.WaitAsync(linkedCts.Token);
        try
        {
            ulong nonce = await _nonceAllocator.AllocateAsync(requestOptions, linkedCts.Token);

            try
            {
                var transaction = TTransaction.Create(_baseClient.ChainId, nonce, call, txParams, txGasParams);
                var signedTransaction = await transaction.SignAsync(Signer, linkedCts.Token);
                var submission = new TxSubmission<TTxParams, TTxGasParams>(
                    _baseClient.ChainId,
                    nonce,
                    0,
                    signedTransaction.Hash,
                    signedTransaction.EncodedTx,
                    call,
                    txParams,
                    txGasParams
                );

                linkedCts.Token.ThrowIfCancellationRequested();

                var id = Guid.NewGuid();
                if(Logger?.IsEnabled(LogLevel.Information) == true)
                {
                    Logger.LogInformation(
                        "Tx {TxId} {Sender}/{Nonce}: Prepared {TxHash}",
                        id, Signer.Address, nonce, submission.TxHash
                    );
                }

                return new PendingTx<TTransaction, TTxParams, TTxGasParams>(
                    id,
                    [submission],
                    this,
                    isPersisted: false,
                    clientDisposalToken: disposalToken
                );
            }
            catch
            {
                if(!_isDisposed)
                {
                    _ = await _nonceAllocator.TryReleaseAsync(nonce, CancellationToken.None);
                }

                throw;
            }
        }
        finally
        {
            _ = _preparationLock.Release();
        }
    }

    async Task<IPendingTx<TTxParams, TTxGasParams>?> IEtherTxClient.AttachPendingTxAsync<TTransaction, TTxParams, TTxGasParams>(
        Guid id, CancellationToken cancellationToken
    )
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if(ResiliencyLayer is null)
        {
            throw new NotSupportedException("Missing ResiliencyLayer");
        }

        var disposalToken = _disposalCts.Token;
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(disposalToken, cancellationToken);
        var senderKey = new TxSenderKey(_baseClient.ChainId, Signer.Address);
        var stored = await ResiliencyLayer.FetchTxSubmissionsAsync(senderKey, id, linkedCts.Token);
        linkedCts.Token.ThrowIfCancellationRequested();

        if(stored.Count == 0)
        {
            return null;
        }

        ulong nonce = stored[0].Nonce;
        foreach(var submission in stored)
        {
            if(submission.Id != id || submission.ChainId != senderKey.ChainId || submission.Sender != senderKey.Sender)
            {
                throw new InvalidOperationException($"Persisted submissions do not match transaction {id} for the current chain and sender");
            }

            if(submission.Nonce != nonce)
            {
                throw new InvalidOperationException($"Transaction {id} has persisted submissions across multiple nonces");
            }
        }

        var typedSubmissions = new TxSubmission<TTxParams, TTxGasParams>[stored.Count];
        for(int i = 0; i < stored.Count; i++)
        {
            typedSubmissions[i] = stored[i].ToTxSubmission<TTxParams, TTxGasParams>();
        }

        Array.Sort(typedSubmissions, static (a, b) => b.Sequence.CompareTo(a.Sequence));

        if(Logger?.IsEnabled(LogLevel.Information) == true)
        {
            Logger.LogInformation(
                "Tx {TxId} {Sender}/{Nonce}: Attached with {SubmissionCount} submissions, latest {TxHash}",
                id, Signer.Address, nonce, typedSubmissions.Length, typedSubmissions[0].TxHash
            );
        }

        return new PendingTx<TTransaction, TTxParams, TTxGasParams>(
            id,
            typedSubmissions,
            this,
            isPersisted: true,
            clientDisposalToken: disposalToken
        );
    }

    public async ValueTask DisposeAsync()
    {
        if(Interlocked.Exchange(ref _isDisposed, true))
        {
            return;
        }

        await _disposalCts.CancelAsync();
        _disposalCts.Dispose();

        _ownerClient.UnregisterTxClient(this);
        _nonceAllocator.Dispose();
        await TxPublisher.DisposeAsync();
    }
}
