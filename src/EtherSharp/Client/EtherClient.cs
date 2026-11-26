using EtherSharp.Client.Modules.Blocks;
using EtherSharp.Client.Modules.Debug;
using EtherSharp.Client.Modules.Ether;
using EtherSharp.Client.Modules.Events;
using EtherSharp.Client.Modules.Mempool;
using EtherSharp.Client.Modules.Trace;
using EtherSharp.Client.Services;
using EtherSharp.Client.Services.ContractFactory;
using EtherSharp.Client.Services.FlashCall;
using EtherSharp.Client.Services.GasFeeProvider;
using EtherSharp.Client.Services.QueryExecutor;
using EtherSharp.Client.Services.Subscriptions;
using EtherSharp.Common;
using EtherSharp.Contract;
using EtherSharp.Numerics;
using EtherSharp.Query;
using EtherSharp.RPC;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.RPC.Transport;
using EtherSharp.Tx;
using EtherSharp.Types;
using EtherSharp.Wallet;
using Microsoft.Extensions.DependencyInjection;

namespace EtherSharp.Client;

internal sealed class EtherClient : IEtherClient, IInternalEtherClient
{
    private readonly IServiceProvider _provider;
    private readonly (ulong ChainId, ChainIdMode Mode)? _configuredChainId;
    private readonly CallGasLimitSettings _callGasLimitSettings;
    private readonly IRPCTransport _rpcTransport;

    private IEtherModule _etherModule = null!;
    private ITraceModule _traceModule = null!;
    private IBlocksModule _blocksModule = null!;
    private IMempoolModule _mempoolModule = null!;
    private IDebugModule _debugModule = null!;

    private IEthRpcModule _ethRpcModule = null!;

    private QueryExecutor _queryExecutor = null!;
    private FlashCallExecutor _flashCallExecutor = null!;
    private ISubscriptionsManager _subscriptionsManager = null!;
    private ContractFactory _contractFactory = null!;
    private EtherSharpJsonSerializerContext _jsonSerializerContext = null!;

    private bool _initialized;
    private bool _isDisposed;
    private ulong _chainId;
    private readonly Lock _disposeLock = new Lock();

    internal EtherClient(
        IServiceProvider provider,
        (ulong ChainId, ChainIdMode Mode)? configuredChainId
    )
    {
        _provider = provider;
        _configuredChainId = configuredChainId;
        _callGasLimitSettings = provider.GetRequiredService<CallGasLimitSettings>();
        _rpcTransport = provider.GetRequiredService<IRPCTransport>();
    }

    IServiceProvider IInternalEtherClient.Provider => _provider;

    ulong IEtherClient.ChainId
        => _initialized
            ? _chainId
            : throw new InvalidOperationException("Client not initialized");

    IEtherModule IEtherClient.ETH
        => _initialized
            ? _etherModule
            : throw new InvalidOperationException("Client not initialized");

    IBlocksModule IEtherClient.Blocks
        => _initialized
            ? _blocksModule
            : throw new InvalidOperationException("Client not initialized");

    IMempoolModule IEtherClient.Mempool
        => _initialized
            ? _mempoolModule
            : throw new InvalidOperationException("Client not initialized");

    ITraceModule IEtherClient.Trace
        => _initialized
            ? _traceModule
            : throw new InvalidOperationException("Client not initialized");

    IDebugModule IEtherClient.Debug
        => _initialized
            ? _debugModule
            : throw new InvalidOperationException("Client not initialized");

    bool IEtherClient.IsInitialized => _initialized;

    public Task<T1> QueryAsync<T1>(
        IQuery<T1> c1,
        ulong? gasLimit = null,
        in CallOptions options = default,
        RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default)
        => ExecuteQueryAsync(c1, gasLimit, options, requestOptions, cancellationToken);

    public Task<(T1, T2)> QueryAsync<T1, T2>(
        IQuery<T1> c1, IQuery<T2> c2,
        ulong? gasLimit = null,
        in CallOptions options = default,
        RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default)
        => ExecuteQueryAsync(IQuery.Combine(c1, c2), gasLimit, options, requestOptions, cancellationToken);

    public Task<(T1, T2, T3)> QueryAsync<T1, T2, T3>(
        IQuery<T1> c1, IQuery<T2> c2, IQuery<T3> c3,
        ulong? gasLimit = null,
        in CallOptions options = default,
        RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default)
        => ExecuteQueryAsync(IQuery.Combine(c1, c2, c3), gasLimit, options, requestOptions, cancellationToken);

    public Task<(T1, T2, T3, T4)> QueryAsync<T1, T2, T3, T4>(
        IQuery<T1> c1, IQuery<T2> c2, IQuery<T3> c3, IQuery<T4> c4,
        ulong? gasLimit = null,
        in CallOptions options = default,
        RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default)
        => ExecuteQueryAsync(IQuery.Combine(c1, c2, c3, c4), gasLimit, options, requestOptions, cancellationToken);

    public Task<(T1, T2, T3, T4, T5)> QueryAsync<T1, T2, T3, T4, T5>(
        IQuery<T1> c1, IQuery<T2> c2, IQuery<T3> c3, IQuery<T4> c4, IQuery<T5> c5,
        ulong? gasLimit = null,
        in CallOptions options = default,
        RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default)
        => ExecuteQueryAsync(IQuery.Combine(c1, c2, c3, c4, c5), gasLimit, options, requestOptions, cancellationToken);

    public Task<(T1, T2, T3, T4, T5, T6)> QueryAsync<T1, T2, T3, T4, T5, T6>(
        IQuery<T1> c1, IQuery<T2> c2, IQuery<T3> c3, IQuery<T4> c4, IQuery<T5> c5, IQuery<T6> c6,
        ulong? gasLimit = null,
        in CallOptions options = default,
        RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default)
        => ExecuteQueryAsync(IQuery.Combine(c1, c2, c3, c4, c5, c6), gasLimit, options, requestOptions, cancellationToken);

    public Task<(T1, T2, T3, T4, T5, T6, T7)> QueryAsync<T1, T2, T3, T4, T5, T6, T7>(
        IQuery<T1> c1, IQuery<T2> c2, IQuery<T3> c3, IQuery<T4> c4, IQuery<T5> c5, IQuery<T6> c6, IQuery<T7> c7,
        ulong? gasLimit = null,
        in CallOptions options = default,
        RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default)
        => ExecuteQueryAsync(IQuery.Combine(c1, c2, c3, c4, c5, c6, c7), gasLimit, options, requestOptions, cancellationToken);

    public Task<(T1, T2, T3, T4, T5, T6, T7, T8)> QueryAsync<T1, T2, T3, T4, T5, T6, T7, T8>(
        IQuery<T1> c1, IQuery<T2> c2, IQuery<T3> c3, IQuery<T4> c4, IQuery<T5> c5, IQuery<T6> c6, IQuery<T7> c7, IQuery<T8> c8,
        ulong? gasLimit = null,
        in CallOptions options = default,
        RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default)
        => ExecuteQueryAsync(IQuery.Combine(c1, c2, c3, c4, c5, c6, c7, c8), gasLimit, options, requestOptions, cancellationToken);

    public Task<(T1, T2, T3, T4, T5, T6, T7, T8, T9)> QueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
        IQuery<T1> c1, IQuery<T2> c2, IQuery<T3> c3, IQuery<T4> c4, IQuery<T5> c5, IQuery<T6> c6, IQuery<T7> c7, IQuery<T8> c8, IQuery<T9> c9,
        ulong? gasLimit = null,
        in CallOptions options = default,
        RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default)
        => ExecuteQueryAsync(IQuery.Combine(c1, c2, c3, c4, c5, c6, c7, c8, c9), gasLimit, options, requestOptions, cancellationToken);

    IEventsModule<TEvent> IEtherClient.Events<TEvent>()
    {
        AssertReady();
        return new EventsModule<TEvent>(_ethRpcModule, _subscriptionsManager, _jsonSerializerContext);
    }

    IConfiguredEventsModule<TEvent> IEtherClient.Events<TEvent>(in EventFilter eventFilter)
    {
        AssertReady();
        return new EventsModule<TEvent>(_ethRpcModule, _subscriptionsManager, _jsonSerializerContext, eventFilter);
    }

    void IEtherClient.SetDefaultCallGasLimits(ulong? ethCallGasLimit, ulong? flashCallGasLimit)
        => _callGasLimitSettings.Set(ethCallGasLimit, flashCallGasLimit);

    private Task<TQuery> ExecuteQueryAsync<TQuery>(
        IQuery<TQuery> query,
        ulong? gasLimit,
        in CallOptions options,
        RpcRequestOptions requestOptions, CancellationToken cancellationToken)
    {
        AssertReady();
        return _queryExecutor.ExecuteQueryAsync(query, gasLimit, options, requestOptions, cancellationToken);
    }

    async Task IEtherClient.InitializeAsync(bool forceNoQuery, RpcRequestOptions requestOptions, CancellationToken cancellationToken)
    {
        if(forceNoQuery)
        {
            await InitializeNoQueryAsync(requestOptions, cancellationToken);
        }
        else
        {
            await InitializeWithQueryAsync<object?>(null, requestOptions, cancellationToken);
        }
    }

    Task<T> IEtherClient.InitializeAsync<T>(IQuery<T> initQuery, RpcRequestOptions requestOptions, CancellationToken cancellationToken)
        => InitializeWithQueryAsync(initQuery, requestOptions, cancellationToken);

    private async Task BaseInitializeAsync(CancellationToken cancellationToken)
    {
        if(_initialized)
        {
            throw new InvalidOperationException("Client already initialized");
        }

        await _rpcTransport.InitializeAsync(cancellationToken);

        _etherModule = _provider.GetRequiredService<IEtherModule>();
        _traceModule = _provider.GetRequiredService<ITraceModule>();
        _blocksModule = _provider.GetRequiredService<IBlocksModule>();
        _mempoolModule = _provider.GetRequiredService<IMempoolModule>();
        _debugModule = _provider.GetRequiredService<IDebugModule>();

        _ethRpcModule = _provider.GetRequiredService<IEthRpcModule>();

        _queryExecutor = _provider.GetRequiredService<QueryExecutor>();
        _flashCallExecutor = _provider.GetRequiredService<FlashCallExecutor>();
        _subscriptionsManager = _provider.GetRequiredService<ISubscriptionsManager>();
        _contractFactory = _provider.GetRequiredService<ContractFactory>();
        _jsonSerializerContext = _provider.GetRequiredService<EtherSharpJsonSerializerContext>();
    }

    private async Task InitializeNoQueryAsync(RpcRequestOptions requestOptions, CancellationToken cancellationToken)
    {
        await BaseInitializeAsync(cancellationToken);

        ulong chainId = _configuredChainId is (ulong assumedChainId, ChainIdMode.Assume)
            ? assumedChainId
            : await _ethRpcModule.ChainIdAsync(requestOptions, cancellationToken);

        await CompleteInitializeAsync(chainId, requestOptions, cancellationToken);
    }

    private async Task<T> InitializeWithQueryAsync<T>(IQuery<T>? initQuery, RpcRequestOptions requestOptions, CancellationToken cancellationToken)
    {
        await BaseInitializeAsync(cancellationToken);

        var chainIdQuery = _configuredChainId is (ulong assumedChainId, ChainIdMode.Assume)
            ? IQuery.Noop(assumedChainId)
            : IQuery.GetChainId();
        initQuery ??= IQuery.Noop<T>(default!);

        var (chainId, initResult) = await _queryExecutor.ExecuteQueryAsync(
            IQuery.Combine(chainIdQuery, initQuery),
            null,
            TargetHeight.Latest,
            requestOptions,
            cancellationToken
        );

        await CompleteInitializeAsync(chainId, requestOptions, cancellationToken);
        return initResult;
    }

    private async Task CompleteInitializeAsync(ulong chainId, RpcRequestOptions requestOptions, CancellationToken cancellationToken)
    {
        ValidateChainId(chainId);
        _chainId = chainId;

        if(_flashCallExecutor is DeployedFlashCallExecutor deployedFlashCallExecutor)
        {
            deployedFlashCallExecutor.Initialize(chainId);
        }

        foreach(var initializeableService in _provider.GetServices<IInitializableService>())
        {
            await initializeableService.InitializeAsync(_chainId, requestOptions, cancellationToken);
        }

        _initialized = true;
    }

    private void ValidateChainId(ulong actualChainId)
    {
        if(_configuredChainId is (ulong expectedChainId, ChainIdMode.Verify) && actualChainId != expectedChainId)
        {
            throw new InvalidOperationException(
                $"Connected RPC reported chain ID {actualChainId}, expected {expectedChainId}."
            );
        }
    }

    private void AssertReady()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if(!_initialized)
        {
            throw new InvalidOperationException("Client not initialized");
        }
    }

    public async ValueTask DisposeAsync()
    {
        lock(_disposeLock)
        {
            if(_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            _initialized = false;
        }

        _subscriptionsManager?.CloseSubscriptions();

        if(_provider is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync();
            return;
        }

        if(_provider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    IInternalEtherClient IEtherClient.AsInternal() => this;

    EtherTxClientBuilder IEtherClient.Attach(IEtherSigner signer)
        => new EtherTxClientBuilder(this, signer);

    Task<TxData?> IEtherClient.GetTransactionAsync(in Bytes32 hash, RpcRequestOptions requestOptions, CancellationToken cancellationToken)
    {
        AssertReady();
        return _ethRpcModule.TransactionByHashAsync(in hash, requestOptions, cancellationToken);
    }

    Task<TxReceipt?> IEtherClient.GetTransactionReceiptAsync(in Bytes32 hash, RpcRequestOptions requestOptions, CancellationToken cancellationToken)
    {
        AssertReady();
        return _ethRpcModule.GetTransactionReceiptAsync(in hash, requestOptions, cancellationToken);
    }

    Task<ulong> IEtherClient.GetTransactionCount(
        in Address address, TargetHeight targetHeight, RpcRequestOptions requestOptions, CancellationToken cancellationToken)
    {
        AssertReady();
        return _ethRpcModule.GetTransactionCountAsync(in address, targetHeight, requestOptions, cancellationToken);
    }

    Task<byte[]> IEtherClient.GetStorageAtAsync(in Address address, byte[] slot, TargetHeight targetHeight, RpcRequestOptions requestOptions, CancellationToken cancellationToken)
    {
        AssertReady();
        return _ethRpcModule.GetStorageAtAsync(in address, slot, targetHeight, requestOptions, cancellationToken);
    }

    Task<byte[]> IEtherClient.GetStorageAtAsync(IEVMContract contract, byte[] slot, TargetHeight targetHeight, RpcRequestOptions requestOptions, CancellationToken cancellationToken)
    {
        AssertReady();
        return _ethRpcModule.GetStorageAtAsync(contract.Address, slot, targetHeight, requestOptions, cancellationToken);
    }

    Task<FeeHistory> IEtherClient.GetFeeHistoryAsync(int blockCount, TargetHeight newestBlock,
        double[] rewardPercentiles, RpcRequestOptions requestOptions, CancellationToken cancellationToken)
    {
        AssertReady();
        return _ethRpcModule.GetFeeHistoryAsync(blockCount, newestBlock, rewardPercentiles, requestOptions, cancellationToken);
    }

    Task<UInt256> IEtherClient.GetGasPriceAsync(RpcRequestOptions requestOptions, CancellationToken cancellationToken)
    {
        AssertReady();
        return _ethRpcModule.GasPriceAsync(requestOptions, cancellationToken);
    }

    Task<UInt256> IEtherClient.GetMaxPriorityFeePerGasAsync(RpcRequestOptions requestOptions, CancellationToken cancellationToken)
    {
        AssertReady();
        return _ethRpcModule.MaxPriorityFeePerGasAsync(requestOptions, cancellationToken);
    }

    public Task<ulong> EstimateGasLimitAsync(
        ITxInput call,
        in CallOptions options,
        RpcRequestOptions requestOptions, CancellationToken cancellationToken)
    {
        AssertReady();
        return _ethRpcModule.EstimateGasAsync(
            call.To, call.Value, call.Data, options, requestOptions, cancellationToken);
    }

    public Task<AccessListResult> CreateAccessListAsync(
        ITxInput call, in CallOptions options,
        RpcRequestOptions requestOptions, CancellationToken cancellationToken)
    {
        AssertReady();
        return _ethRpcModule.CreateAccessListAsync(
            call.To, call.Value, call.Data, options, requestOptions, cancellationToken);
    }

    async Task<TTxGasParams> IEtherClient.EstimateTxGasParamsAsync<TTxParams, TTxGasParams>(
        ITxInput call, TTxParams? txParams, Address? from, RpcRequestOptions requestOptions, CancellationToken cancellationToken)
        where TTxParams : class
    {
        AssertReady();
        var gasFeeProvider = _provider.GetService<IGasFeeProvider<TTxParams, TTxGasParams>>()
            ?? throw new InvalidOperationException(
                $"No GasFeeProvider found that supports {typeof(TTxParams).FullName};{typeof(TTxGasParams).FullName} is not registered");

        return await gasFeeProvider.EstimateGasParamsAsync(
            call, txParams ?? TTxParams.Default, from, requestOptions, cancellationToken
        );
    }

    TContract IEtherClient.Contract<TContract>(in Address address)
    {
        AssertReady();
        return _contractFactory.Create<TContract>(in address);
    }

    public Task<CallResult<T>> SafeCallAsync<T>(
        ITxInput<T> call,
        in CallOptions options,
        RpcRequestOptions requestOptions, CancellationToken cancellationToken)
    {
        AssertReady();

        var resultTask = _ethRpcModule.CallAsync(
            call.To, null, call.Value, call.Data, options, requestOptions, cancellationToken);

        return ParseAsync(call, resultTask);

        // Avoid CallOptions in the async state machine.
        static async Task<CallResult<T>> ParseAsync(
            ITxInput<T> call, Task<TxCallResult> resultTask)
        {
            var result = await resultTask;

            return CallResult<T>.ParseFrom(result, call.To, call.ReadResultFrom);
        }
    }

    public Task<T> CallAsync<T>(
        ITxInput<T> call,
        in CallOptions options,
        RpcRequestOptions requestOptions, CancellationToken cancellationToken)
    {
        return UnwrapAsync(SafeCallAsync(call, options, requestOptions, cancellationToken));

        // Avoid CallOptions in the async state machine.
        static async Task<T> UnwrapAsync(Task<CallResult<T>> resultTask)
        {
            var result = await resultTask;
            return result.Unwrap();
        }
    }

    public Task<CallResult<T>> SafeFlashCallAsync<T>(
        EVMRuntimeCode runtimeCode,
        IFlashCall<T> call,
        ulong? flashCallGasLimit = null,
        in CallOptions options = default,
        RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default)
    {
        AssertReady();

        return ParseAsync(
            call,
            _flashCallExecutor.ExecuteAsync(
                runtimeCode,
                call,
                flashCallGasLimit,
                options,
                requestOptions,
                cancellationToken
            )
        );

        // Avoid CallOptions in the async state machine.
        static async Task<CallResult<T>> ParseAsync(
            IFlashCall<T> call, Task<TxCallResult> resultTask)
        {
            var result = await resultTask;
            return CallResult<T>.ParseFrom(result, null, call.ReadResultFrom);
        }
    }

    public Task<T> FlashCallAsync<T>(
        EVMRuntimeCode runtimeCode,
        IFlashCall<T> call,
        ulong? flashCallGasLimit,
        in CallOptions options,
        RpcRequestOptions requestOptions, CancellationToken cancellationToken)
    {
        return UnwrapAsync(SafeFlashCallAsync(runtimeCode, call, flashCallGasLimit, options, requestOptions, cancellationToken));

        // Avoid CallOptions in the async state machine.
        static async Task<T> UnwrapAsync(Task<CallResult<T>> resultTask)
        {
            var result = await resultTask;
            return result.Unwrap();
        }
    }
}
