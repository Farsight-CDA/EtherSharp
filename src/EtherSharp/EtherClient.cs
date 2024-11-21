using EtherSharp.Contract;
using EtherSharp.Crypto;
using EtherSharp.RLP;
using EtherSharp.RPC;
using EtherSharp.Tx;
using EtherSharp.Tx.Types;
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


    private Task<TransactionReceipt> SendAsync<T>(TxInput<T> call)
    {
        var tx = new EIP1559Transaction(3000, 3000000, 30, call.Target, call.Value, 10000000000, 3534534555, []);

        Span<int> lengthBuffer = stackalloc int[EIP1559Transaction.NestedListCount];
        Span<byte> dataBuffer = stackalloc byte[call.DataLength];

        call.WriteDataTo(dataBuffer);
        tx.GetEncodedSize(dataBuffer, lengthBuffer);

        Span<byte> signatureBuffer = stackalloc byte[64];
        EncodeTemplateAndSign(tx, lengthBuffer, dataBuffer, signatureBuffer);

        throw new NotImplementedException();
    }

    public void EncodeTemplateAndSign<TTransaction>(TTransaction tx, ReadOnlySpan<int> lengthBuffer, ReadOnlySpan<byte> dataBuffer, Span<byte> signatureBuffer)
        where TTransaction : ITransaction
    {
        if (_signer is null)
        {
            throw new InvalidOperationException("No signer configured");
        }

        Span<byte> txTemplateBuffer = stackalloc byte[lengthBuffer[0]];
        tx.Encode(lengthBuffer, dataBuffer, txTemplateBuffer);

        Span<byte> hashBuffer = stackalloc byte[32];
        Keccak256.TryHashData(txTemplateBuffer, hashBuffer);

        _signer.TrySign(hashBuffer, signatureBuffer);
    }

    Task<ulong> IEtherClient.GetChainIdAsync() => GetChainIdAsync();
    Task<BigInteger> IEtherClient.GetBalanceAsync(string address, TargetBlockNumber targetHeight) => GetBalanceAsync(address, targetHeight);
    Task<int> IEtherClient.GetTransactionCount(string address, TargetBlockNumber targetHeight) => GetTransactionCount(address, targetHeight);
    TContract IEtherClient.Contract<TContract>(string address) => Contract<TContract>(address);
    Task<T> IEtherClient.CallAsync<T>(TxInput<T> call, TargetBlockNumber targetHeight) => CallAsync(call, targetHeight);
}
