using EtherSharp.Client.Services.TxScheduler;
using EtherSharp.Contract;
using EtherSharp.RPC;
using EtherSharp.Tx;
using EtherSharp.Tx.Types;
using EtherSharp.Types;
using EtherSharp.Wallet;
using Microsoft.Extensions.DependencyInjection;
using System.Numerics;
using static EtherSharp.RPC.EvmRpcClient;

namespace EtherSharp.Client;
public class EtherClient : IEtherClient, IEtherTxClient
{
    private readonly IServiceProvider _provider;
    private readonly EvmRpcClient _evmRPCClient;

    private readonly bool _isTxClient;
    private readonly IEtherSigner _signer = null!;
    private readonly ITxScheduler _txScheduler = null!;

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
        _evmRPCClient = provider.GetRequiredService<EvmRpcClient>();
        _isTxClient = isTxClient;

        if (isTxClient)
        {
            _signer = provider.GetRequiredService<IEtherSigner>();
            _txScheduler = provider.GetRequiredService<ITxScheduler>();
        }
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

        _chainId = await _evmRPCClient.EthChainId();
        _initialized = true;
    }

    private Task<BigInteger> GetBalanceAsync(string address, TargetBlockNumber targetHeight = default)
    {
        AssertReady();
        return _evmRPCClient.EthGetBalance(address, targetHeight);
    }

    private Task<int> GetTransactionCount(string address, TargetBlockNumber targetHeight = default)
    {
        AssertReady();
        return _evmRPCClient.EthGetTransactionCount(address, targetHeight);
    }

    private TContract Contract<TContract>(string address) where TContract : IContract
    {
        AssertReady();
        throw new NotImplementedException();
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

    private Task<string> SendAsync<T>(TxInput<T> call)
        => _txScheduler.PublishTxAsync(TxType.EIP1559, call);

    Task<BigInteger> IEtherClient.GetBalanceAsync(string address, TargetBlockNumber targetHeight) => GetBalanceAsync(address, targetHeight);
    Task<int> IEtherClient.GetTransactionCount(string address, TargetBlockNumber targetHeight) => GetTransactionCount(address, targetHeight);
    TContract IEtherClient.Contract<TContract>(string address) => Contract<TContract>(address);
    Task<T> IEtherClient.CallAsync<T>(TxInput<T> call, TargetBlockNumber targetHeight) => CallAsync(call, targetHeight);
}
