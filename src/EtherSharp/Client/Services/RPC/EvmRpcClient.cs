using EtherSharp.Common;
using EtherSharp.Common.Exceptions;
using EtherSharp.Events.Subscription;
using EtherSharp.Transport;
using EtherSharp.Types;
using System.Globalization;
using System.Numerics;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace EtherSharp.Client.Services.RPC;

internal partial class EvmRpcClient : IRpcClient
{
    private readonly IRPCTransport _transport;

    public event Action? OnConnectionEstablished;
    public event Action<string, ReadOnlySpan<byte>>? OnSubscriptionMessage;

    private record LogParams(LogResponse Params);
    private record LogResponse(Log Result);

    public EvmRpcClient(IRPCTransport transport)
    {
        _transport = transport;
        _transport.OnConnectionEstablished += () => OnConnectionEstablished?.Invoke();
        _transport.OnSubscriptionMessage += (subscriptionId, payload) => OnSubscriptionMessage?.Invoke(subscriptionId, payload);
    }

    public async Task<ulong> EthChainIdAsync(CancellationToken cancellationToken)
        => await _transport.SendRpcRequest<ulong>("eth_chainId", cancellationToken) switch
        {
            RpcResult<ulong>.Success result => result.Result,
            RpcResult<ulong>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<ulong> EthBlockNumberAsync(CancellationToken cancellationToken)
        => await _transport.SendRpcRequest<string>("eth_blockNumber", cancellationToken) switch
        {
            RpcResult<string>.Success result => ulong.Parse(result.Result.AsSpan()[2..], NumberStyles.HexNumber),
            RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<BigInteger> EthGetBalance(string address, TargetBlockNumber blockNumber, CancellationToken cancellationToken)
        => await _transport.SendRpcRequest<string, string, BigInteger>(
            "eth_getBalance", address, blockNumber.ToString(), cancellationToken) switch
        {
            RpcResult<BigInteger>.Success result => result.Result,
            RpcResult<BigInteger>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<uint> EthGetTransactionCount(string address, TargetBlockNumber blockNumber, CancellationToken cancellationToken)
        => await _transport.SendRpcRequest<string, string, uint>(
            "eth_getTransactionCount", address, blockNumber.ToString(), cancellationToken) switch
        {
            RpcResult<uint>.Success result => result.Result,
            RpcResult<uint>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<string[]> EthAccountsAsync(CancellationToken cancellationToken)
        => await _transport.SendRpcRequest<string[]>("eth_accounts", cancellationToken) switch
        {
            RpcResult<string[]>.Success result => result.Result,
            RpcResult<string[]>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<long> EthBlockTransactionCountByHashAsync(string blockHash, CancellationToken cancellationToken)
    {
        if(blockHash.Length != 66)
        {
            throw new InvalidOperationException("Blockhash string must be 32 Bytes");
        }
        //
        return await _transport.SendRpcRequest<string, string>(
            "eth_getBlockTransactionCountByHash", blockHash, cancellationToken
        ) switch
        {
            RpcResult<string>.Success result => long.Parse(result.Result.AsSpan()[2..], NumberStyles.HexNumber),
            RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<long> EthBlockTransactionCountByNumberAsync(TargetBlockNumber targetBlockNumber, CancellationToken cancellationToken) 
        => await _transport.SendRpcRequest<string, string>(
            "eth_getBlockTransactionCountByNumber", targetBlockNumber.ToString(), cancellationToken) switch
            {
                RpcResult<string>.Success result => long.Parse(result.Result.AsSpan()[2..], NumberStyles.HexNumber),
                RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
                _ => throw new NotImplementedException(),
            };

    public async Task<long> EthBlockTransactionCountByNumberAsync(ulong blockNumber, CancellationToken cancellationToken)
        => await EthBlockTransactionCountByNumberAsync(TargetBlockNumber.Height(blockNumber), cancellationToken);

    private record TransactionEthCall(string? From, string To, uint? Gas, BigInteger? GasPrice, int? Value, string? Data);
    private record FakeAccountData(string? Balance, string? Nonce, string? Code, object? State, int? StateDiff);

    public async Task<TxCallResult> EthCallAsync(
        string? from, string to, uint? gas, BigInteger? gasPrice, int? value, string? data, TargetBlockNumber blockNumber,
        CancellationToken cancellationToken)
    {
        TransactionEthCall transaction = new(from, to, gas, gasPrice, value, data);

        return await _transport.SendRpcRequest<TransactionEthCall, string, byte[]>(
            "eth_call", transaction, blockNumber.ToString(), cancellationToken) switch
        {
            RpcResult<byte[]>.Success result => new TxCallResult.Success(result.Result),
            RpcResult<byte[]>.Error error => error.Message == "execution reverted" && error.Code == -32000
                ? new TxCallResult.Reverted()
                : throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<TxSubmissionResult> EthSendRawTransactionAsync(string transaction, CancellationToken cancellationToken) 
        => await _transport.SendRpcRequest<string, string>("eth_sendRawTransaction", transaction, cancellationToken) switch
        {
            RpcResult<string>.Success result => new TxSubmissionResult.Success(result.Result),
            RpcResult<string>.Error error => error.Code != -32000
                ? throw RPCException.FromRPCError(error)
                : ParseTxSubmissionError(error.Message),
            _ => throw new NotImplementedException(),
        };

    private TxSubmissionResult ParseTxSubmissionError(string message)
    {
        if(TryParseNonceTooLow(message, out var result))
        {
            return result;
        }
        //
        return new TxSubmissionResult.Failure(message);
    }

    [GeneratedRegex("nonce too low: next nonce (\\d+), tx nonce (\\d+)")]
    private partial Regex NonceTooLowRegex { get; }
    private bool TryParseNonceTooLow(string message, out TxSubmissionResult.NonceTooLow result)
    {
        var match = NonceTooLowRegex.Match(message);

        if(!match.Success)
        {
            result = null!;
            return false;
        }

        result = new TxSubmissionResult.NonceTooLow(
            uint.Parse(match.Groups[2].ValueSpan),
            uint.Parse(match.Groups[1].ValueSpan)
        );

        return true;
    }

    public async Task<BigInteger> EthGasPriceAsync(CancellationToken cancellationToken)
    {
        var response = await _transport.SendRpcRequest<string>("eth_gasPrice", cancellationToken);
        switch(response)
        {
            case RpcResult<string>.Success result:
                int hexChars = result.Result.Length - 2;
                Span<char> rawHex = stackalloc char[((hexChars - 1) / 2 * 2) + 2];
                int missingChars = rawHex.Length - hexChars;
                rawHex[..missingChars].Fill('0');
                result.Result.AsSpan(2).CopyTo(rawHex[missingChars..]);
                Span<byte> buffer = stackalloc byte[rawHex.Length / 2];

                var res = Convert.FromHexString(rawHex, buffer, out _, out _);
                if (res != System.Buffers.OperationStatus.Done)
                {
                    throw new InvalidOperationException("Failed to parse resulting hex");
                }

                return new BigInteger(buffer, true, true);
            case RpcResult<string>.Error error:
                throw RPCException.FromRPCError(error);
            default:
                throw new NotImplementedException();
        }
    }

    public async Task<BigInteger> EthMaxPriorityFeePerGas(CancellationToken cancellationToken)
    {
        var response = await _transport.SendRpcRequest<string>("eth_maxPriorityFeePerGas", cancellationToken);
        switch(response)
        {
            case RpcResult<string>.Success result:
                Span<byte> buffer = stackalloc byte[result.Result.Length / 2 - 1];
                Convert.FromHexString(result.Result.AsSpan()[2..], buffer, out _, out _);
                return new BigInteger(buffer, true, true);
            case RpcResult<string>.Error error:
                throw RPCException.FromRPCError(error);
            default:
                throw new NotImplementedException();
        }
    }

    public async Task<FeeHistory> EthGetFeeHistory(int blockCount, TargetBlockNumber newestBlock,
        double[] rewardPercentiles, CancellationToken cancellationToken) 
        => await _transport.SendRpcRequest<int, string, double[], FeeHistory>(
            "eth_feeHistory", blockCount, newestBlock.ToString(), rewardPercentiles, cancellationToken) switch
        {
            RpcResult<FeeHistory>.Success result => result.Result,
            RpcResult<FeeHistory>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    private record EthEstimateGasRequest(string From, string To, BigInteger Value, string Data);
    public async Task<ulong> EthEstimateGasAsync(
        string from, string to, BigInteger value, string data, CancellationToken cancellationToken)
    {
        var transaction = new EthEstimateGasRequest(from, to, value, data);
        return await _transport.SendRpcRequest<EthEstimateGasRequest, ulong>(
            "eth_estimateGas", transaction, cancellationToken) switch
        {
            RpcResult<ulong>.Success result => result.Result,
            RpcResult<ulong>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<BlockData?> EthGetFullBlockByHashAsync(string blockHash, CancellationToken cancellationToken)
    {
        if(blockHash.Length != 66)
        {
            throw new InvalidOperationException("Blockhash string must be 32 Bytes");
        }
        //
        return await _transport.SendRpcRequest<string, bool, BlockData?>(
            "eth_getBlockByHash", blockHash, true, cancellationToken) switch
        {
            RpcResult<BlockData>.Success result => result.Result,
            RpcResult<BlockData>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<BlockDataTrasactionAsString?> EthGetBlockTransactionsByHashAsync(
        string blockHash, CancellationToken cancellationToken)
    {
        if(blockHash.Length != 66)
        {
            throw new InvalidOperationException("Blockhash string must be 32 Bytes");
        }
        //
        return await _transport.SendRpcRequest<string, bool, BlockDataTrasactionAsString>(
            "eth_getBlockByHash", blockHash, false, cancellationToken) switch
        {
            RpcResult<BlockDataTrasactionAsString>.Success result => result.Result,
            RpcResult<BlockDataTrasactionAsString>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<BlockData?> EthGetFullBlockByNumberAsync(
        TargetBlockNumber targetBlockNumber, CancellationToken cancellationToken) 
        => await _transport.SendRpcRequest<string, bool, BlockData>(
            "eth_getBlockByNumber", targetBlockNumber.ToString(), false, cancellationToken) switch
        {
            RpcResult<BlockData>.Success result => result.Result,
            RpcResult<BlockData>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<BlockDataTrasactionAsString> EthGetBlockByNumberAsync(
        TargetBlockNumber targetBlockNumber, CancellationToken cancellationToken) 
        => await _transport.SendRpcRequest<string, bool, BlockDataTrasactionAsString>(
            "eth_getBlockByNumber", targetBlockNumber.ToString(), false, cancellationToken) switch
        {
            RpcResult<BlockDataTrasactionAsString>.Success result => result.Result,
            RpcResult<BlockDataTrasactionAsString>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<Transaction> EthTransactionByHash(string hash, CancellationToken cancellationToken) 
        => await _transport.SendRpcRequest<string, Transaction>(
            "eth_getTransactionByHash", hash, cancellationToken) switch
        {
            RpcResult<Transaction>.Success result => result.Result,
            RpcResult<Transaction>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<Transaction> EthGetTransactionByBlockHashAndIndexAsync(
        string blockHash, int index, CancellationToken cancellationToken)
    {
        if(blockHash.Length != 66)
        {
            throw new InvalidOperationException("Blockhash string must be 32 Bytes");
        }
        //
        return await _transport.SendRpcRequest<string, int, Transaction>(
            "eth_getTransactionByBlockHashAndIndex", blockHash, index, cancellationToken) switch
        {
            RpcResult<Transaction>.Success result => result.Result,
            RpcResult<Transaction>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<Transaction> EthGetTransactionByBlockNumberAndIndexAsync(
        string blockNumber, int index, CancellationToken cancellationToken) 
        => await _transport.SendRpcRequest<string, int, Transaction>(
            "eth_getTransactionByBlockNumberAndIndex", blockNumber, index, cancellationToken) switch
        {
            RpcResult<Transaction>.Success result => result.Result,
            RpcResult<Transaction>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<TransactionReceipt?> EthGetTransactionReceiptAsync(string transactionHash, CancellationToken cancellationToken)   => await _transport.SendRpcRequest<string, TransactionReceipt>(
            "eth_getTransactionReceipt", transactionHash, cancellationToken) switch
        {
            RpcResult<TransactionReceipt>.Success result => result.Result,
            RpcResult<TransactionReceipt>.Error error => throw RPCException.FromRPCError(error),
            RpcResult<TransactionReceipt>.Null => null,
            _ => throw new NotImplementedException(),
        };

    public async Task<Uncle?> EthGetUncleByBlockHashAndIndexAsync(
        string blockHash, int uncleIndex, CancellationToken cancellationToken) 
        => await _transport.SendRpcRequest<string, int, Uncle>(
            "eth_getUncleByBlockHashAndIndex", blockHash, uncleIndex, cancellationToken) switch
        {
            RpcResult<Uncle>.Success result => result.Result,
            RpcResult<Uncle>.Error error => throw RPCException.FromRPCError(error),
            RpcResult<Uncle>.Null _ => null,
            _ => throw new NotImplementedException(),
        };

    public async Task<Uncle?> EthGetUncleByBlockNumberAndIndexAsync(
        TargetBlockNumber targetBlockNumber, uint uncleIndex, CancellationToken cancellationToken) 
        => await _transport.SendRpcRequest<string, uint, Uncle>(
            "eth_getUncleByBlockNumberAndIndex", targetBlockNumber.ToString(), uncleIndex, cancellationToken) switch
        {
            RpcResult<Uncle>.Success result => result.Result,
            RpcResult<Uncle>.Error error => throw RPCException.FromRPCError(error),
            RpcResult<Uncle>.Null _ => null,
            _ => throw new NotImplementedException(),
        };

    private record EthNewFilterRequest(
        string FromBlock,
        string ToBlock,
        string[]? Address,
        string[]? Topics
    );
    public async Task<string> EthNewFilterAsync(
        TargetBlockNumber fromBlock, TargetBlockNumber toBlock,
        string[]? address, string[]? topics,
        CancellationToken cancellationToken)
    {
        if(!_transport.SupportsFilters)
        {
            throw new InvalidOperationException("The underlying transport does not support filters");
        }

        var filterOptions = new EthNewFilterRequest(fromBlock.ToString(), toBlock.ToString(), address, topics);
        return await _transport.SendRpcRequest<EthNewFilterRequest, string>(
            "eth_newFilter", filterOptions, cancellationToken) switch
        {
            RpcResult<string>.Success result => result.Result,
            RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<string> EthNewBlockFilterAsync(CancellationToken cancellationToken) 
        => await _transport.SendRpcRequest<string>(
            "eth_newBlockFilter", cancellationToken) switch
        {
            RpcResult<string>.Success result => result.Result,
            RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<string> EthNewPendingTransactionFilterAsync(CancellationToken cancellationToken) 
        => await _transport.SendRpcRequest<string>(
            "eth_newPendingTransactionFilter", cancellationToken) switch
        {
            RpcResult<string>.Success result => result.Result,
            RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<bool> EthUninstallFilterAsync(string filterId, CancellationToken cancellationToken) 
        => await _transport.SendRpcRequest<string, bool>(
            "eth_uninstallFilter", filterId, cancellationToken) switch
        {
            RpcResult<bool>.Success result => result.Result,
            RpcResult<bool>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<List<string?>> EthGetPendingTransactionFilterChangesAsync(string filterId, CancellationToken cancellationToken)   => await _transport.SendRpcRequest<string, List<string?>>(
            "eth_getFilterChanges", filterId, cancellationToken) switch
        {
            RpcResult<List<string?>>.Success result => result.Result,
            RpcResult<List<string?>>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<Log[]> EthGetEventFilterChangesAsync(string filterId, CancellationToken cancellationToken) 
        => await _transport.SendRpcRequest<string, Log[]>(
            "eth_getFilterChanges", filterId, cancellationToken) switch
        {
            RpcResult<Log[]>.Success result => result.Result,
            RpcResult<Log[]>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    private record EthGetLogsRequest(
        string FromBlock,
        string ToBlock,
        string[]? Address,
        string[]? Topics,
        byte[]? BlockHash
    );
    public async Task<Log[]> EthGetLogsAsync(
        TargetBlockNumber fromBlock, TargetBlockNumber toBlock,
        string[]? addresses, string[]? topics, byte[]? blockHash,
        CancellationToken cancellationToken)
    {
        var filterOptions = new EthGetLogsRequest(fromBlock.ToString(), toBlock.ToString(), addresses, topics, blockHash);
        return await _transport.SendRpcRequest<EthGetLogsRequest, Log[]>(
            "eth_getLogs", filterOptions, cancellationToken) switch
        {
            RpcResult<Log[]>.Success result => result.Result,
            RpcResult<Log[]>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    private record EthSubscribeLogsRequest(string[]? Address, string[]? Topics);
    public async Task<string> EthSubscribeLogsAsync(string[]? contracts, string[]? topics, CancellationToken cancellationToken)
    {
        var request = new EthSubscribeLogsRequest(contracts, topics);
        return await _transport.SendRpcRequest<string, EthSubscribeLogsRequest, string>(
            "eth_subscribe", "logs", request, cancellationToken) switch
        {
            RpcResult<string>.Success result => result.Result,
            RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<bool> EthUnsubscribeAsync(string subscriptionId, CancellationToken cancellationToken) 
        => await _transport.SendRpcRequest<string, bool>(
            "eth_unsubscribe", subscriptionId, cancellationToken) switch
        {
            RpcResult<bool>.Success result => result.Result,
            RpcResult<bool>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
}