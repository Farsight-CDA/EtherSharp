using EtherSharp.Contract;
using EtherSharp.Crypto;
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
    private readonly IEtherHdWallet _etherHdWallet;

    internal EtherClient(EvmRpcClient evmRpcClient, IEtherHdWallet etherHdWallet)
    {
        _evmRPCClient = evmRpcClient;
        _etherHdWallet = etherHdWallet;
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

    private void BuildTx(string from, string to, int value, string data, Span<byte> outBytes)
    {
        int nonce = GetTransactionCount(from, TargetBlockNumber.Latest).Result;
        ulong chainId = GetChainIdAsync().Result;
        var gasPrice = _evmRPCClient.EthGasPriceAsync().Result;
        uint gas = _evmRPCClient.EthEstimateGasAsync(from, to, null, gasPrice, value, data, TargetBlockNumber.Latest).Result;

        int noncelenght = RLPEncoder.GetMaxEncodeIntSize(nonce);
        int chainIdLenght = RLPEncoder.GetMaxEncodeIntSize(chainId);
        int gasPriceLenght = RLPEncoder.GetMaxEncodeIntSize(gasPrice);
        int gasLenght = RLPEncoder.GetMaxEncodeIntSize(gas);

        Span<byte> bytes = stackalloc byte[noncelenght + chainIdLenght + gasPriceLenght + gasLenght];

        var noncebytes = bytes[..noncelenght];
        RLPEncoder.EncodeInt(nonce, noncebytes);

        var chainIdBytes = bytes.Slice(noncelenght, chainIdLenght);
        RLPEncoder.EncodeInt(chainId, chainIdBytes);

        var gasPriceBytes = bytes.Slice(noncelenght + chainIdLenght, gasPriceLenght);
        RLPEncoder.EncodeInt(gasPrice, gasPriceBytes);

        var gasBytes = bytes.Slice(noncelenght + chainIdLenght + gasPriceLenght, gasLenght);
        RLPEncoder.EncodeInt(gas, gasBytes);

        _ = Keccak256.TryHashData(bytes, outBytes);
    }

    private Task<TransactionReceipt> SendAsync<T>(TxInput<T> call)
    {

        Span<byte> outBytes = stackalloc byte[32];
        BuildTx(_etherHdWallet.Address, call.Target, call.Value, call.GetCalldataHex(), outBytes);

        Span<byte> singedBytes = stackalloc byte[32];
        _etherHdWallet.Sign(outBytes, singedBytes);

        return _evmRPCClient.Eth.EthSendRawTransactionAsync(singedBytes);
    }

    Task<ulong> IEtherClient.GetChainIdAsync() => GetChainIdAsync();
    Task<BigInteger> IEtherClient.GetBalanceAsync(string address, TargetBlockNumber targetHeight) => GetBalanceAsync(address, targetHeight);
    Task<int> IEtherClient.GetTransactionCount(string address, TargetBlockNumber targetHeight) => GetTransactionCount(address, targetHeight);
    TContract IEtherClient.Contract<TContract>(string address) => Contract<TContract>(address);
    Task<T> IEtherClient.CallAsync<T>(TxInput<T> call, TargetBlockNumber targetHeight) => CallAsync(call, targetHeight);
    Task<TransactionReceipt> IEtherTxClient.SendAsync<T>(TxInput<T> call) => SendAsync(call);
}
