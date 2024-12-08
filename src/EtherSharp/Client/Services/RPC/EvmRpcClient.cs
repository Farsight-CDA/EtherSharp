﻿using EtherSharp.Common;
using EtherSharp.Common.Exceptions;
using EtherSharp.Transport;
using EtherSharp.Types;
using System.Globalization;
using System.Numerics;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace EtherSharp.Client.Services.RPC;

internal partial class EvmRpcClient(IRPCTransport transport) : IRpcClient
{
    private readonly IRPCTransport _transport = transport;

    private readonly Lock _subscriptionsLock = new Lock();
    private List<(string SubscriptionId, ISubscriptionHandler<Log> Callback)>? _eventSubscriptions;

    private record LogParams(LogResponse Params);
    private record LogResponse(Log Result);
    private void SetupSubscriptions()
    {
        if(_eventSubscriptions is not null)
        {
            return;
        }

        lock(_subscriptionsLock)
        {
            if(_eventSubscriptions is not null)
            {
                return;
            }

            _eventSubscriptions = [];
            _transport.OnSubscriptionMessage += (subscriptionId, payload) =>
            {
                var (_, eventCallback) = _eventSubscriptions.FirstOrDefault(x => x.SubscriptionId == subscriptionId);

                if(eventCallback is not null)
                {
                    var p = JsonSerializer.Deserialize<LogParams>(payload, ParsingUtils.EvmSerializerOptions)!;
                    eventCallback.HandlePayload(p.Params.Result);
                }
            };
        }
    }

    public async Task<ulong> EthChainId()
        => await _transport.SendRpcRequest<ulong>("eth_chainId") switch
        {
            RpcResult<ulong>.Success result => result.Result,
            RpcResult<ulong>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<long> EthBlockNumberAsync()
        => await _transport.SendRpcRequest<string>("eth_blockNumber") switch
        {
            RpcResult<string>.Success result => long.Parse(result.Result.AsSpan()[2..], NumberStyles.HexNumber),
            RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<BigInteger> EthGetBalance(string address, TargetBlockNumber blockNumber)
        => await _transport.SendRpcRequest<string, string, BigInteger>("eth_getBalance", address, blockNumber.ToString()) switch
        {
            RpcResult<BigInteger>.Success result => result.Result,
            RpcResult<BigInteger>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<uint> EthGetTransactionCount(string address, TargetBlockNumber blockNumber)
        => await _transport.SendRpcRequest<string, string, uint>("eth_getTransactionCount", address, blockNumber.ToString()) switch
        {
            RpcResult<uint>.Success result => result.Result,
            RpcResult<uint>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<string[]> EthAccountsAsync()
        => await _transport.SendRpcRequest<string[]>("eth_accounts") switch
        {
            RpcResult<string[]>.Success result => result.Result,
            RpcResult<string[]>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<long> EthBlockTransactionCountByHashAsync(string blockHash)
    {
        if(blockHash.Length != 66)
        {
            throw new InvalidOperationException("Blockhash string must be 32 Bytes");
        }

        var response = await _transport.SendRpcRequest<string, string>("eth_getBlockTransactionCountByHash", blockHash);
        return response switch
        {
            RpcResult<string>.Success result => long.Parse(result.Result.AsSpan()[2..], NumberStyles.HexNumber),
            RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<long> EthBlockTransactionCountByNumberAsync(TargetBlockNumber targetBlockNumber)
    {
        var response = await _transport.SendRpcRequest<string, string>("eth_getBlockTransactionCountByNumber", targetBlockNumber.ToString());

        return response switch
        {
            RpcResult<string>.Success result => long.Parse(result.Result.AsSpan()[2..], NumberStyles.HexNumber),
            RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<long> EthBlockTransactionCountByNumberAsync(long blockNumber)
        => await EthBlockTransactionCountByNumberAsync(TargetBlockNumber.Height(blockNumber));

    private record TransactionEthCall(string? From, string To, uint? Gas, BigInteger? GasPrice, int? Value, string? Data);
    private record FakeAccountData(string? Balance, string? Nonce, string? Code, object? State, int? StateDiff);

    public async Task<TxCallResult> EthCallAsync(
        string? from, string to, uint? gas, BigInteger? gasPrice, int? value, string? data, TargetBlockNumber blockNumber)
    {
        TransactionEthCall transaction = new(from, to, gas, gasPrice, value, data);

        var response = await _transport.SendRpcRequest<TransactionEthCall, string, byte[]>("eth_call", transaction, blockNumber.ToString());

        return response switch
        {
            RpcResult<byte[]>.Success result => new TxCallResult.Success(result.Result),
            RpcResult<byte[]>.Error error => error.Message == "execution reverted" && error.Code == -32000
                ? new TxCallResult.Reverted()
                : throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<TxSubmissionResult> EthSendRawTransactionAsync(string transaction)
    {
        var response = await _transport.SendRpcRequest<string, string>("eth_sendRawTransaction", transaction);
        return response switch
        {
            RpcResult<string>.Success result => new TxSubmissionResult.Success(result.Result),
            RpcResult<string>.Error error => error.Code != -32000
                ? throw RPCException.FromRPCError(error)
                : ParseTxSubmissionError(error.Message),
            _ => throw new NotImplementedException(),
        };
    }
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

    public async Task<BigInteger> EthGasPriceAsync()
    {
        var response = await _transport.SendRpcRequest<string>("eth_gasPrice");
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

    public async Task<BigInteger> EthMaxPriorityFeePerGas()
    {
        var response = await _transport.SendRpcRequest<string>("eth_maxPriorityFeePerGas");
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

    private record EthEstimateGasRequest(string From, string To, BigInteger Value, string Data);
    public async Task<ulong> EthEstimateGasAsync(string from, string to, BigInteger value, string data)
    {
        var transaction = new EthEstimateGasRequest(from, to, value, data);
        var response = await _transport.SendRpcRequest<EthEstimateGasRequest, ulong>("eth_estimateGas", transaction);

        return response switch
        {
            RpcResult<ulong>.Success result => result.Result,
            RpcResult<ulong>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<BlockData?> EthGetFullBlockByHashAsync(string blockHash)
    {
        if(blockHash.Length != 66)
        {
            throw new InvalidOperationException("Blockhash string must be 32 Bytes");
        }
        var response = await _transport.SendRpcRequest<string, bool, BlockData?>("eth_getBlockByHash", blockHash, true);

        return response switch
        {
            RpcResult<BlockData>.Success result => result.Result,
            RpcResult<BlockData>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<BlockDataTrasactionAsString?> EthGetBlockTransactionsByHashAsync(string blockHash)
    {
        if(blockHash.Length != 66)
        {
            throw new InvalidOperationException("Blockhash string must be 32 Bytes");
        }
        var response = await _transport.SendRpcRequest<string, bool, BlockDataTrasactionAsString>("eth_getBlockByHash", blockHash, false);
        return response switch
        {
            RpcResult<BlockDataTrasactionAsString>.Success result => result.Result,
            RpcResult<BlockDataTrasactionAsString>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<BlockData?> EthGetFullBlockByNumberAsync(TargetBlockNumber targetBlockNumber)
    {

        var response = await _transport.SendRpcRequest<string, bool, BlockData>("eth_getBlockByNumber", targetBlockNumber.ToString(), true);
        return response switch
        {
            RpcResult<BlockData>.Success result => result.Result,
            RpcResult<BlockData>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<BlockDataTrasactionAsString?> EthGetBlockByNumberAsync(TargetBlockNumber targetBlockNumber)
    {

        var response = await _transport.SendRpcRequest<string, bool, BlockDataTrasactionAsString>("eth_getBlockByNumber", targetBlockNumber.ToString(), false);
        return response switch
        {
            RpcResult<BlockDataTrasactionAsString>.Success result => result.Result,
            RpcResult<BlockDataTrasactionAsString>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<Transaction> EthTransactionByHash(string hash)
    {
        var response = await _transport.SendRpcRequest<string, Transaction>("eth_getTransactionByHash", hash);
        return response switch
        {
            RpcResult<Transaction>.Success result => result.Result,
            RpcResult<Transaction>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<Transaction> EthGetTransactionByBlockHashAndIndexAsync(string blockHash, int index)
    {
        if(blockHash.Length != 66)
        {
            throw new InvalidOperationException("Blockhash string must be 32 Bytes");
        }
        var response = await _transport.SendRpcRequest<string, int, Transaction>("eth_getTransactionByBlockHashAndIndex", blockHash, index);
        return response switch
        {
            RpcResult<Transaction>.Success result => result.Result,
            RpcResult<Transaction>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<Transaction> EthGetTransactionByBlockNumberAndIndexAsync(string blockNumber, int index)
    {
        var response = await _transport.SendRpcRequest<string, int, Transaction>("eth_getTransactionByBlockNumberAndIndex", blockNumber, index);
        return response switch
        {
            RpcResult<Transaction>.Success result => result.Result,
            RpcResult<Transaction>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<TransactionReceipt?> EthGetTransactionReceiptAsync(string transactionHash)
    {
        var response = await _transport.SendRpcRequest<string, TransactionReceipt>("eth_getTransactionReceipt", transactionHash);
        return response switch
        {
            RpcResult<TransactionReceipt>.Success result => result.Result,
            RpcResult<TransactionReceipt>.Error error => throw RPCException.FromRPCError(error),
            RpcResult<TransactionReceipt>.Null => null,
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<Uncle?> EthGetUncleByBlockHashAndIndexAsync(string blockHash, int uncleIndex)
    {
        var response = await _transport.SendRpcRequest<string, int, Uncle>("eth_getUncleByBlockHashAndIndex", blockHash, uncleIndex);
        return response switch
        {
            RpcResult<Uncle>.Success result => result.Result,
            RpcResult<Uncle>.Error error => throw RPCException.FromRPCError(error),
            RpcResult<Uncle>.Null _ => null,
            _ => throw new NotImplementedException(),
        };
    }
    public async Task<Uncle?> EthGetUncleByBlockNumberAndIndexAsync(TargetBlockNumber targetBlockNumber, uint uncleIndex)
    {
        var response = await _transport.SendRpcRequest<string, uint, Uncle>("eth_getUncleByBlockNumberAndIndex", targetBlockNumber.ToString(), uncleIndex);
        return response switch
        {
            RpcResult<Uncle>.Success result => result.Result,
            RpcResult<Uncle>.Error error => throw RPCException.FromRPCError(error),
            RpcResult<Uncle>.Null _ => null,
            _ => throw new NotImplementedException(),
        };
    }

    private record EthNewFilterRequest(
        string FromBlock,
        string ToBlock,
        string[]? Address,
        string[]? Topics
    );
    public async Task<string> EthNewFilterAsync(
        TargetBlockNumber fromBlock, TargetBlockNumber toBlock,
        string[]? address, string[]? topics)
    {
        if(!_transport.SupportsFilters)
        {
            throw new InvalidOperationException("The underlying transport does not support filters");
        }

        var filterOptions = new EthNewFilterRequest(fromBlock.ToString(), toBlock.ToString(), address, topics);
        var response = await _transport.SendRpcRequest<EthNewFilterRequest, string>("eth_newFilter", filterOptions);
        return response switch
        {
            RpcResult<string>.Success result => result.Result,
            RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<string> EthNewBlockFilterAsync()
    {
        var response = await _transport.SendRpcRequest<string>("eth_newBlockFilter");
        return response switch
        {
            RpcResult<string>.Success result => result.Result,
            RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<string> EthNewPendingTransactionFilterAsync()
    {
        var response = await _transport.SendRpcRequest<string>("eth_newPendingTransactionFilter");
        return response switch
        {
            RpcResult<string>.Success result => result.Result,
            RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<bool> EthUninstallFilterAsync(string filterId)
    {
        var response = await _transport.SendRpcRequest<string, bool>("eth_uninstallFilter", filterId);
        return response switch
        {
            RpcResult<bool>.Success result => result.Result,
            RpcResult<bool>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }
    public async Task<List<string?>> EthGetPendingTransactionFilterChangesAsync(string filterId)
    {
        var response = await _transport.SendRpcRequest<string, List<string?>>("eth_getFilterChanges", filterId);
        return response switch
        {
            RpcResult<List<string?>>.Success result => result.Result,
            RpcResult<List<string?>>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }
    public async Task<Log[]> EthGetEventFilterChangesAsync(string filterId)
    {
        var response = await _transport.SendRpcRequest<string, Log[]>("eth_getFilterChanges", filterId);
        return response switch
        {
            RpcResult<Log[]>.Success result => result.Result,
            RpcResult<Log[]>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    private record EthGetLogsRequest(
        string FromBlock,
        string ToBlock,
        string[]? Address,
        string[]? Topics,
        byte[]? BlockHash
    );
    public async Task<Log[]> EthGetLogsAsync(
        TargetBlockNumber fromBlock, TargetBlockNumber toBlock,
        string[]? addresses, string[]? topics, byte[]? blockHash)
    {
        var filterOptions = new EthGetLogsRequest(fromBlock.ToString(), toBlock.ToString(), addresses, topics, blockHash);
        var response = await _transport.SendRpcRequest<EthGetLogsRequest, Log[]>("eth_getLogs", filterOptions);
        return response switch
        {
            RpcResult<Log[]>.Success result => result.Result,
            RpcResult<Log[]>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task RegisterSubscriptionAsync(ISubscriptionHandler<Log> handler)
    {
        if(!_transport.SupportsSubscriptions)
        {
            throw new InvalidOperationException("The underlying transport does not support subscriptions");
        }

        SetupSubscriptions();

        string subscriptionId = await handler.InstallAsync();
        _eventSubscriptions!.Add((subscriptionId, handler));
    }

    private record EthSubscribeLogsRequest(string[]? Address, string[]? Topics);
    public async Task<string> EthSubscribeLogsAsync(string[]? contracts, string[]? topics)
    {
        var request = new EthSubscribeLogsRequest(contracts, topics);
        var response = await _transport.SendRpcRequest<string, EthSubscribeLogsRequest, string>(
            "eth_subscribe", "logs", request);
        return response switch
        {
            RpcResult<string>.Success result => result.Result,
            RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<bool> EthUnsubscribeAsync(string subscriptionId)
    {
        var response = await _transport.SendRpcRequest<string, bool>("eth_unsubscribe", subscriptionId);
        return response switch
        {
            RpcResult<bool>.Success result => result.Result,
            RpcResult<bool>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }
}