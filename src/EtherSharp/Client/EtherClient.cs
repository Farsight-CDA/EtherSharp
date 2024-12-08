﻿using EtherSharp.Client.Services;
using EtherSharp.Client.Services.ContractFactory;
using EtherSharp.Client.Services.EtherApi;
using EtherSharp.Client.Services.LogsApi;
using EtherSharp.Client.Services.RPC;
using EtherSharp.Client.Services.TxScheduler;
using EtherSharp.Contract;
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

    public async Task InitializeAsync()
    {
        if(_initialized)
        {
            throw new InvalidOperationException("Client already initialized");
        }

        _etherApi = _provider.GetRequiredService<EtherApi>();
        _rpcClient = _provider.GetRequiredService<IRpcClient>();
        _contractFactory = _provider.GetRequiredService<ContractFactory>();

        if(_isTxClient)
        {
            _signer = _provider.GetRequiredService<IEtherSigner>();
            _txScheduler = _provider.GetRequiredService<ITxScheduler>();
        }

        _chainId = await _rpcClient.EthChainId();

        foreach(var initializeableService in _provider.GetServices<IInitializableService>())
        {
            await initializeableService.InitializeAsync(_chainId);
        }

        _initialized = true;
    }

    public Task<long> GetPeakHeightAsync()
    {
        AssertReady();
        return _rpcClient.EthBlockNumberAsync();
    }

    private Task<BigInteger> GetBalanceAsync(string address, TargetBlockNumber targetHeight = default)
    {
        AssertReady();
        return _rpcClient.EthGetBalance(address, targetHeight);
    }

    private Task<uint> GetTransactionCount(string address, TargetBlockNumber targetHeight = default)
    {
        AssertReady();
        return _rpcClient.EthGetTransactionCount(address, targetHeight);
    }
    Task<uint> IEtherClient.GetTransactionCount(string address, TargetBlockNumber targetHeight)
        => GetTransactionCount(address, targetHeight);

    private TContract Contract<TContract>(string contractAddress)
        where TContract : IEVMContract
    {
        AssertReady();
        return _contractFactory.Create<TContract>(contractAddress);
    }

    private async Task<T> CallAsync<T>(TxInput<T> call, TargetBlockNumber targetHeight = default)
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
            targetHeight
        );

        return result switch
        {
            TxCallResult.Success s => call.ReadResultFrom(s.Data),
            TxCallResult.Reverted => throw new Exception("Call reverted"),
            _ => throw new NotImplementedException()
        };
    }

    Task<TransactionReceipt> IEtherTxClient.ExecuteTxAsync(ITxInput call,
        Func<ValueTask<TxTimeoutAction>> onTxTimeout)
    {
        AssertTxClient();
        AssertReady();
        return _txScheduler.PublishTxAsync(new EIP1559TxParams([]), call, onTxTimeout);
    }

    Task<TransactionReceipt> IEtherTxClient.ExecuteTxAsync(ITxInput call,
        Func<TxTimeoutAction> onTxTimeout)
    {
        AssertTxClient();
        AssertReady();
        return _txScheduler.PublishTxAsync(new EIP1559TxParams([]), call, () => ValueTask.FromResult(onTxTimeout()));
    }

    TContract IEtherClient.Contract<TContract>(string address)
        => Contract<TContract>(address);
    Task<T> IEtherClient.CallAsync<T>(TxInput<T> call, TargetBlockNumber targetHeight)
        => CallAsync(call, targetHeight);
}
