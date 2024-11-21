using EtherSharp.Contract;
using EtherSharp.Crypto;
using EtherSharp.RLP;
using EtherSharp.RPC;
using EtherSharp.Tx;
using EtherSharp.Types;
using EtherSharp.Wallet;
using System.Numerics;
using static EtherSharp.RPC.EvmRpcClient;

namespace EtherSharp;
public class EtherClient : IEtherClient, IEtherTxClient
{
    private readonly EvmRpcClient _evmRPCClient;
    private readonly IEtherSigner? _signer;

    internal EtherClient(EvmRpcClient evmRpcClient, IEtherSigner signer)
    {
        _evmRPCClient = evmRpcClient;
        _signer = signer;
    }

    internal EtherClient(EvmRpcClient evmRpcClient)
    {
        _evmRPCClient = evmRpcClient;
        _signer = null;
    }

    private Task<ulong> GetChainIdAsync()
        => _evmRPCClient.EthChainId();

    private Task<BigInteger> GetBalanceAsync(string address, TargetBlockNumber targetHeight = default)
        => _evmRPCClient.EthGetBalance(address, targetHeight);

    private Task<int> GetTransactionCount(string address, TargetBlockNumber targetHeight = default)
        => _evmRPCClient.EthGetTransactionCount(address, targetHeight);

    private TContract Contract<TContract>(string address) where TContract : IContract
        => throw new NotImplementedException();

    private async Task<T> CallAsync<T>(TxInput<T> call, TargetBlockNumber targetHeight = default)
    {
        var result = await _evmRPCClient.EthCallAsync(
            null,
            call.Target,
            null,
            null,
            null,
            call.GetCalldataHex(),
            targetHeight
        );

        return result switch
        {
            ContractReturn.Success s => call.ReadResultFrom(s.Data),
            ContractReturn.Reverted => throw new Exception("Call reverted"),
            _ => throw new NotImplementedException()
        };
    }

    private void BuildTx(ReadOnlySpan<byte> from, ReadOnlySpan<byte> to, ReadOnlySpan<byte> data, uint nonce, BigInteger gasPrice, ulong gas, BigInteger value)
    {
        int bufferSize = RLPEncoder.GetListSize(
            RLPEncoder.GetIntSize(nonce) +
            RLPEncoder.GetIntSize(gasPrice) +
            RLPEncoder.GetIntSize(gas) +
            RLPEncoder.GetStringSize(to) +
            RLPEncoder.GetIntSize(value) +
            RLPEncoder.GetStringSize(data)
        );

        Span<byte> rlpBuffer = bufferSize > 2048
            ? new byte[bufferSize]
            : stackalloc byte[bufferSize];

        var encoder = new RLPEncoder(rlpBuffer);
        encoder.EncodeList(bufferSize)
            .EncodeInt(nonce)
            .EncodeInt(gasPrice)
            .EncodeInt(gas)
            .EncodeString(to)
            .EncodeInt(value)
            .EncodeString(data);

        _ = Keccak256.HashData(rlpBuffer);
    }

    //private Task<TransactionReceipt> SendAsync<T>(TxInput<T> call)
    //{

    //    Span<byte> outBytes = stackalloc byte[32];
    //    BuildTx();

    //    Span<byte> singedBytes = stackalloc byte[32];
    //    _etherHdWallet.Sign(outBytes, singedBytes);

    //    return _evmRPCClient.Eth.EthSendRawTransactionAsync(singedBytes);
    //}

    Task<ulong> IEtherClient.GetChainIdAsync() => GetChainIdAsync();
    Task<BigInteger> IEtherClient.GetBalanceAsync(string address, TargetBlockNumber targetHeight) => GetBalanceAsync(address, targetHeight);
    Task<int> IEtherClient.GetTransactionCount(string address, TargetBlockNumber targetHeight) => GetTransactionCount(address, targetHeight);
    TContract IEtherClient.Contract<TContract>(string address) => Contract<TContract>(address);
    Task<T> IEtherClient.CallAsync<T>(TxInput<T> call, TargetBlockNumber targetHeight) => CallAsync(call, targetHeight);
    public Task<TransactionReceipt> SendAsync<T>(TxInput<T> call) => throw new NotImplementedException();
    //Task<TransactionReceipt> IEtherTxClient.SendAsync<T>(TxInput<T> call) => SendAsync(call);
}
