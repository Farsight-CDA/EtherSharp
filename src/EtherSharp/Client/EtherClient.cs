using EtherSharp.Client.Modules.Blocks;
using EtherSharp.Client.Modules.Ether;
using EtherSharp.Client.Modules.Events;
using EtherSharp.Client.Modules.Query;
using EtherSharp.Client.Modules.Trace;
using EtherSharp.Client.Services;
using EtherSharp.Client.Services.ContractFactory;
using EtherSharp.Client.Services.GasFeeProvider;
using EtherSharp.Client.Services.Subscriptions;
using EtherSharp.Client.Services.TxScheduler;
using EtherSharp.Contract;
using EtherSharp.RPC;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.Transport;
using EtherSharp.Tx;
using EtherSharp.Tx.PendingHandler;
using EtherSharp.Types;
using EtherSharp.Wallet;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Numerics;

namespace EtherSharp.Client;

internal class EtherClient : IEtherClient, IEtherTxClient, IInternalEtherClient
{
    private readonly IServiceProvider _provider;
    private readonly ILoggerFactory? _loggerFactory;
    private readonly bool _isTxClient;

    private IEtherTxModule _etherModule = null!;
    private ITraceModule _traceModule = null!;
    private IBlocksModule _blocksModule = null!;

    private IRpcClient _rpcClient = null!;
    private IEthRpcModule _ethRpcModule = null!;

    private IEtherSigner _signer = null!;
    private ITxScheduler _txScheduler = null!;

    private ISubscriptionsManager _subscriptionsManager = null!;
    private ContractFactory _contractFactory = null!;

    private bool _initialized;
    private ulong _chainId;

    IServiceProvider IInternalEtherClient.Provider => _provider;
    IRpcClient IInternalEtherClient.RPC => _rpcClient;

    ulong IEtherClient.ChainId
    {
        get
        {
            AssertReady();
            return _chainId;
        }
    }

    IEtherTxModule IEtherTxClient.ETH
    {
        get
        {
            AssertReady();
            AssertTxClient();
            return _etherModule;
        }
    }

    IEtherModule IEtherClient.ETH
    {
        get
        {
            AssertReady();
            return _etherModule;
        }
    }

    IBlocksModule IEtherClient.Blocks
    {
        get
        {
            AssertReady();
            return _blocksModule;
        }
    }

    public IQueryBuilder<T> Query<T>() => new QueryBuilder<T>(_ethRpcModule, _loggerFactory?.CreateLogger<IQueryBuilder<T>>());
    public async Task<(T1, T2)> QueryAsync<T1, T2>(IQuery<T1> c1, IQuery<T2> c2,
        TargetBlockNumber targetBlockNumber, CancellationToken cancellationToken)
    {
        var builder = new QueryBuilder<object>(_ethRpcModule, _loggerFactory?.CreateLogger<IQueryBuilder<(T1, T2)>>())
            .AddQuery(c1, x => x!)
            .AddQuery(c2, x => x!);
        var results = await builder.QueryAsync(targetBlockNumber, cancellationToken);
        return ((T1) results[0], (T2) results[1]);
    }
    public async Task<(T1, T2, T3)> QueryAsync<T1, T2, T3>(IQuery<T1> c1, IQuery<T2> c2, IQuery<T3> c3,
        TargetBlockNumber targetBlockNumber, CancellationToken cancellationToken)
    {
        var builder = new QueryBuilder<object>(_ethRpcModule, _loggerFactory?.CreateLogger<IQueryBuilder<(T1, T2, T3)>>())
            .AddQuery(c1, x => x!)
            .AddQuery(c2, x => x!)
            .AddQuery(c3, x => x!);
        var results = await builder.QueryAsync(targetBlockNumber, cancellationToken);
        return ((T1) results[0], (T2) results[1], (T3) results[2]);
    }
    public async Task<(T1, T2, T3, T4)> QueryAsync<T1, T2, T3, T4>(IQuery<T1> c1, IQuery<T2> c2, IQuery<T3> c3, IQuery<T4> c4,
        TargetBlockNumber targetBlockNumber, CancellationToken cancellationToken)
    {
        var builder = new QueryBuilder<object>(_ethRpcModule, _loggerFactory?.CreateLogger<IQueryBuilder<(T1, T2, T3)>>())
            .AddQuery(c1, x => x!)
            .AddQuery(c2, x => x!)
            .AddQuery(c3, x => x!)
            .AddQuery(c4, x => x!);
        var results = await builder.QueryAsync(targetBlockNumber, cancellationToken);
        return ((T1) results[0], (T2) results[1], (T3) results[2], (T4) results[3]);
    }
    public async Task<(T1, T2, T3, T4, T5)> QueryAsync<T1, T2, T3, T4, T5>(IQuery<T1> c1, IQuery<T2> c2, IQuery<T3> c3, IQuery<T4> c4, IQuery<T5> c5,
        TargetBlockNumber targetBlockNumber, CancellationToken cancellationToken)
    {
        var builder = new QueryBuilder<object>(_ethRpcModule, _loggerFactory?.CreateLogger<IQueryBuilder<(T1, T2, T3)>>())
            .AddQuery(c1, x => x!)
            .AddQuery(c2, x => x!)
            .AddQuery(c3, x => x!)
            .AddQuery(c4, x => x!)
            .AddQuery(c5, x => x!);
        var results = await builder.QueryAsync(targetBlockNumber, cancellationToken);
        return ((T1) results[0], (T2) results[1], (T3) results[2], (T4) results[3], (T5) results[4]);
    }

    IEventsModule<TEvent> IEtherClient.Events<TEvent>()
    {
        AssertReady();
        return new EventsModule<TEvent>(_rpcClient, _ethRpcModule, _subscriptionsManager);
    }

    internal EtherClient(IServiceProvider provider, bool isTxClient)
    {
        _provider = provider;
        _loggerFactory = provider.GetService<ILoggerFactory>();
        _isTxClient = isTxClient;
    }

    private void AssertTxClient()
    {
        if(!_isTxClient)
        {
            throw new InvalidOperationException("Client is not a tx client");
        }
    }
    private void AssertReady()
    {
        if(!_initialized)
        {
            throw new InvalidOperationException("Client not initialized");
        }
    }

    IInternalEtherClient IEtherClient.AsInternal() => this;

    async Task IEtherClient.InitializeAsync(CancellationToken cancellationToken)
    {
        if(_initialized)
        {
            throw new InvalidOperationException("Client already initialized");
        }

        await _provider.GetRequiredService<IRPCTransport>()
            .InitializeAsync(cancellationToken);

        _etherModule = _provider.GetRequiredService<IEtherTxModule>();
        _traceModule = _provider.GetRequiredService<ITraceModule>();
        _blocksModule = _provider.GetRequiredService<IBlocksModule>();

        _rpcClient = _provider.GetRequiredService<IRpcClient>();
        _ethRpcModule = _provider.GetRequiredService<IEthRpcModule>();

        _contractFactory = _provider.GetRequiredService<ContractFactory>();
        _subscriptionsManager = _provider.GetRequiredService<ISubscriptionsManager>();

        if(_isTxClient)
        {
            _signer = _provider.GetRequiredService<IEtherSigner>();
            _txScheduler = _provider.GetRequiredService<ITxScheduler>();
        }

        _chainId = await _ethRpcModule.ChainIdAsync(cancellationToken);

        foreach(var initializeableService in _provider.GetServices<IInitializableService>())
        {
            await initializeableService.InitializeAsync(_chainId, cancellationToken);
        }

        _initialized = true;
    }

    Task<Transaction?> IEtherClient.GetTransactionAsync(string hash, CancellationToken cancellationToken)
    {
        AssertReady();
        return _ethRpcModule.TransactionByHashAsync(hash, cancellationToken);
    }
    Task<TransactionReceipt?> IEtherClient.GetTransactionReceiptAsync(string hash, CancellationToken cancellationToken)
    {
        AssertReady();
        return _ethRpcModule.GetTransactionReceiptAsync(hash, cancellationToken);
    }

    Task<uint> IEtherClient.GetTransactionCount(
        Address address, TargetBlockNumber targetHeight, CancellationToken cancellationToken)
    {
        AssertReady();
        return _ethRpcModule.GetTransactionCountAsync(address, targetHeight, cancellationToken);
    }

    private TContract Contract<TContract>(Address address)
        where TContract : IEVMContract
    {
        AssertReady();
        return _contractFactory.Create<TContract>(address);
    }

    Task<FeeHistory> IEtherClient.GetFeeHistoryAsync(int blockCount, TargetBlockNumber newestBlock,
        double[] rewardPercentiles, CancellationToken cancellationToken)
    {
        AssertReady();
        return _ethRpcModule.GetFeeHistoryAsync(blockCount, newestBlock, rewardPercentiles, cancellationToken);
    }
    Task<BigInteger> IEtherClient.GetGasPriceAsync(CancellationToken cancellationToken)
    {
        AssertReady();
        return _ethRpcModule.GasPriceAsync(cancellationToken);
    }

    Task<BigInteger> IEtherClient.GetMaxPriorityFeePerGasAsync(CancellationToken cancellationToken)
    {
        AssertReady();
        return _ethRpcModule.MaxPriorityFeePerGasAsync(cancellationToken);
    }

    Task<ulong> IEtherClient.EstimateGasLimitAsync(ITxInput call, Address? from, CancellationToken cancellationToken)
    {
        AssertReady();

        if(from is null && _isTxClient)
        {
            from = _signer.Address;
        }

        string data = $"0x{Convert.ToHexString(call.Data)}";
        return _ethRpcModule.EstimateGasAsync(from, call.To, call.Value, data, cancellationToken);
    }

    async Task<TTxGasParams> IEtherClient.EstimateTxGasParamsAsync<TTxParams, TTxGasParams>(
        ITxInput call, TTxParams? txParams, CancellationToken cancellationToken)
        where TTxParams : class
    {
        AssertReady();
        var gasFeeProvider = _provider.GetService<IGasFeeProvider<TTxParams, TTxGasParams>>()
            ?? throw new InvalidOperationException(
                $"No GasFeeProvider found that supports {typeof(TTxParams).FullName};{typeof(TTxGasParams).FullName} is not registered");

        return await gasFeeProvider.EstimateGasParamsAsync(call, txParams ?? TTxParams.Default, cancellationToken);
    }

    TContract IEtherClient.Contract<TContract>(Address address)
        => Contract<TContract>(address);

    async Task<T> IEtherClient.CallAsync<T>(ITxInput<T> call, TargetBlockNumber targetHeight, CancellationToken cancellationToken)
    {
        AssertReady();

        var sender = _isTxClient
            ? _signer.Address
            : null;

        var result = await _ethRpcModule.CallAsync(
            sender,
            call.To,
            null,
            null,
            call.Value,
            $"0x{Convert.ToHexString(call.Data)}",
            targetHeight,
            cancellationToken
        );

        return call.ReadResultFrom(result.Unwrap(call.To));
    }

    async Task<IPendingTxHandler<TTxParams, TTxGasParams>> IEtherTxClient.PrepareTxAsync<TTransaction, TTxParams, TTxGasParams>(
        ITxInput call, TTxParams? txParams, TTxGasParams? txGasParams
    )
        where TTxParams : class
        where TTxGasParams : class
        => await _txScheduler.PrepareTxAsync<TTransaction, TTxParams, TTxGasParams>(call, txParams, txGasParams);

    async Task<IPendingTxHandler<TTxParams, TTxGasParams>> IEtherTxClient.AttachPendingTxAsync<TTransaction, TTxParams, TTxGasParams>(uint nonce)
        => await _txScheduler.AttachPendingTxAsync<TTransaction, TTxParams, TTxGasParams>(nonce);
}
