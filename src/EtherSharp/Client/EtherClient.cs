﻿using EtherSharp.Client.Services;
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
using EtherSharp.Tx.EIP1559;
using EtherSharp.Types;
using EtherSharp.Wallet;
using Microsoft.Extensions.DependencyInjection;
using System.Numerics;

namespace EtherSharp.Client;
public class EtherClient : IEtherClient, IEtherTxClient
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
    public ulong ChainId
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

    ulong IEtherClient.ChainId => throw new NotImplementedException();

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

    public async Task InitializeAsync(CancellationToken cancellationToken)
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

    public Task<BlockDataTrasactionAsString> GetBlockAsync(TargetBlockNumber targetBlockNumber, CancellationToken cancellationToken) 
    {
        AssertReady();
        return _rpcClient.EthGetBlockByNumberAsync(targetBlockNumber, cancellationToken);
    }
    public Task<Transaction> GetTransactionAsync(string hash, CancellationToken cancellationToken)
    {
        AssertReady();
        return _rpcClient.EthTransactionByHash(hash, cancellationToken);
    }
    public Task<ulong> GetPeakHeightAsync(CancellationToken cancellationToken)
    {
        AssertReady();
        return _rpcClient.EthBlockNumberAsync(cancellationToken);
    }
    public Task<uint> GetTransactionCount(
        string address, TargetBlockNumber targetHeight = default, CancellationToken cancellationToken = default)
    {
        AssertReady();
        return _rpcClient.EthGetTransactionCount(address, targetHeight, cancellationToken);
    }

    private Task<BigInteger> GetBalanceAsync(
        string address, TargetBlockNumber targetHeight = default, CancellationToken cancellationToken = default)
    {
        AssertReady();
        return _rpcClient.EthGetBalance(address, targetHeight, cancellationToken);
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

    private async Task<T> CallAsync<T>(
        TxInput<T> call, TargetBlockNumber targetHeight = default, TxStateOverride? stateOverride = default, CancellationToken cancellationToken = default)
    {
        AssertReady();
        ITxInput icall = call;

        Span<byte> callDataBuffer = stackalloc byte[icall.DataLength];
        icall.WriteDataTo(callDataBuffer);

        string? sender = _isTxClient
            ? _signer.Address.String
            : null;

        var result = await _rpcClient.EthCallAsync(
            sender,
            call.To.String,
            null,
            null,
            null,
            $"0x{Convert.ToHexString(callDataBuffer)}",
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

    public Task<FeeHistory> GetFeeHistoryAsync(int blockCount, TargetBlockNumber newestBlock,
        double[] rewardPercentiles, CancellationToken cancellationToken)
    {
        AssertReady();
        return _rpcClient.EthGetFeeHistory(blockCount, newestBlock, rewardPercentiles, cancellationToken);
    }
    public Task<BigInteger> GetGasPriceAsync(CancellationToken cancellationToken)
    {
        AssertReady();
        return _rpcClient.EthGasPriceAsync(cancellationToken);
    }

    public Task<BigInteger> GetMaxPriorityFeePerGasAsync(CancellationToken cancellationToken = default)
    {
        AssertReady();
        return _rpcClient.EthMaxPriorityFeePerGas(cancellationToken);
    }

    public Task<ulong> EstimateGasLimitAsync(ITxInput call, string? from = null, CancellationToken cancellationToken = default)
    {
        AssertReady();

        if (from is null && _isTxClient)
        {
            from = _signer.Address.String;
        }

        Span<byte> buffer = stackalloc byte[call.DataLength];
        call.WriteDataTo(buffer);
        string data = $"0x{Convert.ToHexString(buffer)}";
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

        Span<byte> inputData = stackalloc byte[call.DataLength];
        call.WriteDataTo(inputData);
        return await gasFeeProvider.EstimateGasParamsAsync(call.To, call.Value, inputData, txParams ?? TTxParams.Default, cancellationToken);
    }

    Task<TransactionReceipt> IEtherTxClient.ExecuteTxAsync(ITxInput call, Func<ValueTask<TxTimeoutAction>> onTxTimeout,
        EIP1559TxParams? txParams, EIP1559GasParams? txGasParams)
    {
        AssertTxClient();
        AssertReady();
        return _txScheduler.PublishTxAsync<EIP1559Transaction, EIP1559TxParams, EIP1559GasParams>(
            call, txParams, txGasParams, onTxTimeout
        );
    }
    Task<TransactionReceipt> IEtherTxClient.ExecuteTxAsync(ITxInput call, Func<TxTimeoutAction> onTxTimeout,
        EIP1559TxParams? txParams, EIP1559GasParams? txGasParams)
    {
        AssertTxClient();
        AssertReady();
        return _txScheduler.PublishTxAsync<EIP1559Transaction, EIP1559TxParams, EIP1559GasParams>(
            call, txParams, txGasParams, () => ValueTask.FromResult(onTxTimeout())
        );
    }

    Task<TransactionReceipt> IEtherTxClient.ExecuteTxAsync<TTransaction, TTxParams, TTxGasParams>(
        ITxInput call, Func<TxTimeoutAction> onTxTimeout,
        TTxParams? txParams, TTxGasParams? txGasParams)
        where TTxParams : class
        where TTxGasParams : class
    {
        AssertTxClient();
        AssertReady();
        return _txScheduler.PublishTxAsync<TTransaction, TTxParams, TTxGasParams>(
            call, txParams, txGasParams, () => ValueTask.FromResult(onTxTimeout())
        );
    }
    Task<TransactionReceipt> IEtherTxClient.ExecuteTxAsync<TTransaction, TTxParams, TTxGasParams>(
        ITxInput call, Func<ValueTask<TxTimeoutAction>> onTxTimeout,
        TTxParams? txParams, TTxGasParams? txGasParams)
        where TTxParams : class
        where TTxGasParams : class
    {
        AssertTxClient();
        AssertReady();
        return _txScheduler.PublishTxAsync<TTransaction, TTxParams, TTxGasParams>(
            call, txParams, txGasParams, onTxTimeout
        );
    }

    TContract IEtherClient.Contract<TContract>(string address)
        => Contract<TContract>(address); 
    TContract IEtherClient.Contract<TContract>(Address address)
        => Contract<TContract>(address);
    Task<T> IEtherClient.CallAsync<T>(TxInput<T> call, TargetBlockNumber targetHeight, TxStateOverride? stateOverride, CancellationToken cancellationToken)
        => CallAsync(call, targetHeight, stateOverride, cancellationToken);
}
