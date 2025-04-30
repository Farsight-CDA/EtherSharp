using EtherSharp.Client.Services;
using EtherSharp.Client.Services.ContractFactory;
using EtherSharp.Client.Services.EtherApi;
using EtherSharp.Client.Services.GasFeeProvider;
using EtherSharp.Client.Services.LogsApi;
using EtherSharp.Client.Services.RPC;
using EtherSharp.Client.Services.TxScheduler;
using EtherSharp.Contract;
using EtherSharp.StateOverride;
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

    private EtherApi _etherApi = null!;
    private IRpcClient _rpcClient = null!;
    private IEtherSigner _signer = null!;
    private ITxScheduler _txScheduler = null!;

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

    IEtherTxApi IEtherTxClient.ETH
    {
        get
        {
            AssertReady();
            AssertTxClient();
            return _etherApi;
        }
    }

    IEtherApi IEtherClient.ETH
    {
        get
        {
            AssertReady();
            return _etherApi;
        }
    }

    ILogsApi<TEvent> IEtherClient.Logs<TEvent>()
    {
        AssertReady();
        return new LogsApi<TEvent>(_rpcClient);
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

        _etherApi = _provider.GetRequiredService<EtherApi>();
        _rpcClient = _provider.GetRequiredService<IRpcClient>();
        _contractFactory = _provider.GetRequiredService<ContractFactory>();

        if(_isTxClient)
        {
            _signer = _provider.GetRequiredService<IEtherSigner>();
            _txScheduler = _provider.GetRequiredService<ITxScheduler>();
        }

        _chainId = await _rpcClient.EthChainIdAsync(cancellationToken);

        foreach(var initializeableService in _provider.GetServices<IInitializableService>())
        {
            await initializeableService.InitializeAsync(_chainId, cancellationToken);
        }

        _initialized = true;
    }

    Task<BlockDataTrasactionAsString> IEtherClient.GetBlockAsync(TargetBlockNumber targetBlockNumber, CancellationToken cancellationToken)
    {
        AssertReady();
        return _rpcClient.EthGetBlockByNumberAsync(targetBlockNumber, cancellationToken);
    }
    Task<Transaction> IEtherClient.GetTransactionAsync(string hash, CancellationToken cancellationToken)
    {
        AssertReady();
        return _rpcClient.EthTransactionByHash(hash, cancellationToken);
    }
    Task<ulong> IEtherClient.GetPeakHeightAsync(CancellationToken cancellationToken)
    {
        AssertReady();
        return _rpcClient.EthBlockNumberAsync(cancellationToken);
    }
    Task<uint> IEtherClient.GetTransactionCount(
        string address, TargetBlockNumber targetHeight, CancellationToken cancellationToken)
    {
        AssertReady();
        return _rpcClient.EthGetTransactionCount(address, targetHeight, cancellationToken);
    }

    private TContract Contract<TContract>(string contractAddress)
        where TContract : IEVMContract
    {
        AssertReady();
        return _contractFactory.Create<TContract>(Address.FromString(contractAddress));
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
        return _rpcClient.EthGetFeeHistory(blockCount, newestBlock, rewardPercentiles, cancellationToken);
    }
    Task<BigInteger> IEtherClient.GetGasPriceAsync(CancellationToken cancellationToken)
    {
        AssertReady();
        return _rpcClient.EthGasPriceAsync(cancellationToken);
    }

    Task<BigInteger> IEtherClient.GetMaxPriorityFeePerGasAsync(CancellationToken cancellationToken)
    {
        AssertReady();
        return _rpcClient.EthMaxPriorityFeePerGas(cancellationToken);
    }

    Task<ulong> IEtherClient.EstimateGasLimitAsync(ITxInput call, string? from, CancellationToken cancellationToken)
    {
        AssertReady();

        if(from is null && _isTxClient)
        {
            from = _signer.Address.String;
        }

        string data = $"0x{Convert.ToHexString(call.Data)}";
        return _rpcClient.EthEstimateGasAsync(from, call.To.String, call.Value, data, cancellationToken);
    }

    async Task<TTxGasParams> IEtherClient.EstimateTxGasParamsAsync<TTxParams, TTxGasParams>(
        ITxInput call, TTxParams? txParams, TxStateOverride? stateOverride, CancellationToken cancellationToken)
        where TTxParams : class
    {
        AssertReady();
        var gasFeeProvider = _provider.GetService<IGasFeeProvider<TTxParams, TTxGasParams>>()
            ?? throw new InvalidOperationException(
                $"No GasFeeProvider found that supports {typeof(TTxParams).FullName};{typeof(TTxGasParams).FullName} is not registered");

        return await gasFeeProvider.EstimateGasParamsAsync(call.To, call.Value, call.Data, txParams ?? TTxParams.Default, cancellationToken);
    }

    TContract IEtherClient.Contract<TContract>(string address)
        => Contract<TContract>(address);
    TContract IEtherClient.Contract<TContract>(Address address)
        => Contract<TContract>(address);

    async Task<T> IEtherClient.CallAsync<T>(ITxInput<T> call, TargetBlockNumber targetHeight, TxStateOverride? stateOverride, CancellationToken cancellationToken)
    {
        AssertReady();

        string? sender = _isTxClient
            ? _signer.Address.String
            : null;

        var result = await _rpcClient.EthCallAsync(
            sender,
            call.To.String,
            null,
            null,
            null,
            $"0x{Convert.ToHexString(call.Data)}",
            targetHeight,
            stateOverride,
            cancellationToken
        );

        return result switch
        {
            TxCallResult.Success s => call.ReadResultFrom(s.Data),
            TxCallResult.Reverted => throw new Exception("Call reverted"),
            _ => throw new NotImplementedException()
        };
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
