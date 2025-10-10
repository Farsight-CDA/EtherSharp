using EtherSharp.Client.Modules.Ether;
using EtherSharp.Client.Modules.Events;
using EtherSharp.Client.Modules.Trace;
using EtherSharp.Client.Services;
using EtherSharp.Client.Services.ContractFactory;
using EtherSharp.Client.Services.GasFeeProvider;
using EtherSharp.Client.Services.Subscriptions;
using EtherSharp.Client.Services.TxScheduler;
using EtherSharp.Contract;
using EtherSharp.Realtime.Blocks.Subscription;
using EtherSharp.RPC;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.Transport;
using EtherSharp.Tx;
using EtherSharp.Tx.PendingHandler;
using EtherSharp.Types;
using EtherSharp.Wallet;
using Microsoft.Extensions.DependencyInjection;
using System.Numerics;

namespace EtherSharp.Client;

internal class EtherClient : IEtherClient, IEtherTxClient, IInternalEtherClient
{
    private readonly IServiceProvider _provider;
    private readonly bool _isTxClient;

    private EtherModule _etherModule = null!;
    private TraceModule _traceModule = null!;

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

    IEventsModule<TEvent> IEtherClient.Events<TEvent>()
    {
        AssertReady();
        return new EventsModule<TEvent>(_rpcClient, _ethRpcModule, _subscriptionsManager);
    }

    internal EtherClient(IServiceProvider provider, bool isTxClient)
    {
        _provider = provider;
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

        _etherModule = _provider.GetRequiredService<EtherModule>();
        _traceModule = _provider.GetRequiredService<TraceModule>();

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

    async Task<IBlocksSubscription> IEtherClient.SubscribeNewHeadsAsync(CancellationToken cancellationToken)
    {
        AssertReady();
        var subscription = new BlocksSubscription(_ethRpcModule, _subscriptionsManager);
        await _subscriptionsManager.InstallSubscriptionAsync(subscription, cancellationToken);
        return subscription;
    }

    Task<BlockDataTrasactionAsString> IEtherClient.GetBlockAsync(TargetBlockNumber targetBlockNumber, CancellationToken cancellationToken)
    {
        AssertReady();
        return _ethRpcModule.GetBlockByNumberAsync(targetBlockNumber, cancellationToken);
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

    Task<ulong> IEtherClient.GetPeakHeightAsync(CancellationToken cancellationToken)
    {
        AssertReady();
        return _ethRpcModule.BlockNumberAsync(cancellationToken);
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

        return await gasFeeProvider.EstimateGasParamsAsync(call.To, call.Value, call.Data, txParams ?? TTxParams.Default, cancellationToken);
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

        return call.ReadResultFrom(result.Unwrap());
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
