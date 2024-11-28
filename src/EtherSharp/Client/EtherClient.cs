using EtherSharp.Client.Services;
using EtherSharp.Client.Services.ContractFactory;
using EtherSharp.Client.Services.TxScheduler;
using EtherSharp.Contract;
using EtherSharp.RPC;
using EtherSharp.Tx;
using EtherSharp.Tx.EIP1559;
using EtherSharp.Types;
using EtherSharp.Wallet;
using Microsoft.Extensions.DependencyInjection;
using System.Numerics;
using static EtherSharp.RPC.EvmRpcClient;

namespace EtherSharp.Client;
public class EtherClient : IEtherClient, IEtherTxClient
{
    private readonly IServiceProvider _provider;
    private readonly bool _isTxClient;

    private EvmRpcClient _evmRPCClient = null!;
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

        _evmRPCClient = _provider.GetRequiredService<EvmRpcClient>();
        _contractFactory = _provider.GetRequiredService<ContractFactory>();


        if(_isTxClient)
        {
            _signer = _provider.GetRequiredService<IEtherSigner>();
            _txScheduler = _provider.GetRequiredService<ITxScheduler>();
        }

        _chainId = await _evmRPCClient.EthChainId();

        foreach(var initializeableService in _provider.GetServices<IInitializableService>())
        {
            await initializeableService.InitializeAsync(_chainId);
        }

        _initialized = true;
    }

    private Task<BigInteger> GetBalanceAsync(string address, TargetBlockNumber targetHeight = default)
    {
        AssertReady();
        return _evmRPCClient.EthGetBalance(address, targetHeight);
    }

    private Task<uint> GetTransactionCount(string address, TargetBlockNumber targetHeight = default)
    {
        AssertReady();
        return _evmRPCClient.EthGetTransactionCount(address, targetHeight);
    }

    private TContract Contract<TContract>(string contractAddress)
        where TContract : IContract
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

        var result = await _evmRPCClient.EthCallAsync(
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
            ContractReturn.Success s => call.ReadResultFrom(s.Data),
            ContractReturn.Reverted => throw new Exception("Call reverted"),
            _ => throw new NotImplementedException()
        };
    }

    public Task<string> SendAsync<T>(ITxInput call, ulong gas, BigInteger maxFeePerGas, BigInteger maxPriorityFeePerGas)
        => _txScheduler.PublishTxAsync(new EIP1559TxParams(gas, maxFeePerGas, maxPriorityFeePerGas, []), call);

    Task<BigInteger> IEtherClient.GetBalanceAsync(string address, TargetBlockNumber targetHeight) => GetBalanceAsync(address, targetHeight);
    Task<uint> IEtherClient.GetTransactionCount(string address, TargetBlockNumber targetHeight) => GetTransactionCount(address, targetHeight);
    TContract IEtherClient.Contract<TContract>(string address) 
        => Contract<TContract>(address);
    Task<T> IEtherClient.CallAsync<T>(TxInput<T> call, TargetBlockNumber targetHeight) => CallAsync(call, targetHeight);
}
