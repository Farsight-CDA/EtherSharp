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
        Span<byte> callDataBuffer = stackalloc byte[call.DataLength];
        call.WriteDataTo(callDataBuffer);

        var result = await _evmRPCClient.EthCallAsync(
            null,
            call.Target.String,
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

    private async Task<string> SendAsync<T>(TxInput<T> call)
    {
        if(_signer is null)
        {
            throw new InvalidOperationException("No signer configured");
        }

        var tx = new EIP1559Transaction(3000, 3000000, 30, call.Target, call.Value, 10000000000, 3534534555, []);

        Span<int> lengthBuffer = stackalloc int[EIP1559Transaction.NestedListCount];
        Span<byte> dataBuffer = stackalloc byte[call.DataLength];

        call.WriteDataTo(dataBuffer);
        tx.GetEncodedSize(dataBuffer, lengthBuffer);

        Span<byte> txBuffer = stackalloc byte[lengthBuffer[0] + TxRLPEncoder.MaxEncodedSignatureLength];
        Span<byte> hashBuffer = stackalloc byte[32];

        var txTemplateBuffer = txBuffer[..lengthBuffer[0]];
        var signatureBuffer = txBuffer[lengthBuffer[0]..];

        tx.Encode(lengthBuffer, dataBuffer, txTemplateBuffer);
        Keccak256.TryHashData(txTemplateBuffer, hashBuffer);

        SignAndEncode(hashBuffer, signatureBuffer, out int signatureLength);

        var signedTxBuffer = txBuffer[..(lengthBuffer[0] + signatureLength)];

        return await _evmRPCClient.EthSendRawTransactionAsync($"0x{Convert.ToHexString(signedTxBuffer)}");
    }

    private void SignAndEncode(Span<byte> hashBuffer, Span<byte> signatureBuffer, out int encodedSignatureLength)
    {
        Span<byte> tempBuffer = stackalloc byte[65];
        _signer!.TrySign(hashBuffer, tempBuffer);
        new RLPEncoder(signatureBuffer).EncodeSignature(tempBuffer, out encodedSignatureLength);
    }

    Task<ulong> IEtherClient.GetChainIdAsync() => GetChainIdAsync();
    Task<BigInteger> IEtherClient.GetBalanceAsync(string address, TargetBlockNumber targetHeight) => GetBalanceAsync(address, targetHeight);
    Task<int> IEtherClient.GetTransactionCount(string address, TargetBlockNumber targetHeight) => GetTransactionCount(address, targetHeight);
    TContract IEtherClient.Contract<TContract>(string address) => Contract<TContract>(address);
    Task<T> IEtherClient.CallAsync<T>(TxInput<T> call, TargetBlockNumber targetHeight) => CallAsync(call, targetHeight);
}
