using EtherSharp.Client.Services.TxPublisher;
using EtherSharp.Common.Exceptions;
using EtherSharp.Common.Extensions;
using EtherSharp.Common.Instrumentation;
using EtherSharp.StateOverride;
using EtherSharp.Transport;
using EtherSharp.Types;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Numerics;

namespace EtherSharp.Client.Services.RPC;

internal partial class EvmRpcClient : IRpcClient
{
    private readonly IRPCTransport _transport;
    private readonly IRpcMiddleware[] _middlewares;

    private readonly OTELCounter<long>? _rpcRequestsCounter;

    public event Action? OnConnectionEstablished;
    public event Action<string, ReadOnlySpan<byte>>? OnSubscriptionMessage;

    private record LogParams(LogResponse Params);
    private record LogResponse(Log Result);

    public EvmRpcClient(IRPCTransport transport, IServiceProvider serviceProvider)
    {
        _transport = transport;
        _middlewares = [.. serviceProvider.GetServices<IRpcMiddleware>().Reverse()];

        _rpcRequestsCounter = serviceProvider.CreateOTELCounter<long>("evm_rpc_requests");

        _rpcRequestsCounter?.Add(0, new KeyValuePair<string, object?>("status", "success"));
        _rpcRequestsCounter?.Add(0, new KeyValuePair<string, object?>("status", "failure"));

        if(_transport.SupportsSubscriptions)
        {
            _transport.OnConnectionEstablished += () => OnConnectionEstablished?.Invoke();
            _transport.OnSubscriptionMessage += (subscriptionId, payload) => OnSubscriptionMessage?.Invoke(subscriptionId, payload);
        }
    }

    private Task<RpcResult<TResult>> SendRpcRequest<TResult>(
        string method, CancellationToken cancellationToken = default
    ) => SendRpcRequestAsync<TResult>(method, [], cancellationToken);
    private Task<RpcResult<TResult>> SendRpcRequest<T1, TResult>(
        string method, T1 t1, CancellationToken cancellationToken = default
    ) => SendRpcRequestAsync<TResult>(method, [t1], cancellationToken);
    private Task<RpcResult<TResult>> SendRpcRequest<T1, T2, TResult>(
        string method, T1 t1, T2 t2, CancellationToken cancellationToken = default
    ) => SendRpcRequestAsync<TResult>(method, [t1, t2], cancellationToken);
    private Task<RpcResult<TResult>> SendRpcRequest<T1, T2, T3, TResult>(
        string method, T1 t1, T2 t2, T3 t3, CancellationToken cancellationToken = default
    ) => SendRpcRequestAsync<TResult>(method, [t1, t2, t3], cancellationToken);

    private async Task<RpcResult<TResult>> SendRpcRequestAsync<TResult>(string method, object?[] parameters, CancellationToken cancellationToken)
    {
        Func<CancellationToken, Task<RpcResult<TResult>>> onNext = async (ct) =>
        {
            try
            {
                var result = await _transport.SendRpcRequestAsync<TResult>(method, parameters, ct);
                _rpcRequestsCounter?.Add(1, new KeyValuePair<string, object?>("status", "success"));
                return result;
            }
            catch
            {
                _rpcRequestsCounter?.Add(1, new KeyValuePair<string, object?>("status", "failure"));
                throw;
            }
        };

        foreach(var middleware in _middlewares)
        {
            var next = onNext;
            onNext = (ct) => middleware.HandleAsync(ct, next);
        }

        return await onNext(cancellationToken);
    }

    public async Task<ulong> EthChainIdAsync(CancellationToken cancellationToken)
        => await SendRpcRequest<ulong>("eth_chainId", cancellationToken) switch
        {
            RpcResult<ulong>.Success result => result.Result,
            RpcResult<ulong>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<ulong> EthBlockNumberAsync(CancellationToken cancellationToken)
        => await SendRpcRequest<string>("eth_blockNumber", cancellationToken) switch
        {
            RpcResult<string>.Success result => ulong.Parse(result.Result.AsSpan()[2..], NumberStyles.HexNumber, CultureInfo.InvariantCulture),
            RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<BigInteger> EthGetBalance(Address address, TargetBlockNumber blockNumber, CancellationToken cancellationToken)
        => await SendRpcRequest<Address, string, BigInteger>("eth_getBalance", address, blockNumber.ToString(), cancellationToken) switch
        {
            RpcResult<BigInteger>.Success result => result.Result,
            RpcResult<BigInteger>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<uint> EthGetTransactionCount(Address address, TargetBlockNumber blockNumber, CancellationToken cancellationToken)
        => await SendRpcRequest<Address, string, uint>(
            "eth_getTransactionCount", address, blockNumber.ToString(), cancellationToken) switch
        {
            RpcResult<uint>.Success result => result.Result,
            RpcResult<uint>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<string[]> EthAccountsAsync(CancellationToken cancellationToken)
        => await SendRpcRequest<string[]>("eth_accounts", cancellationToken) switch
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
        return await SendRpcRequest<string, string>(
            "eth_getBlockTransactionCountByHash", blockHash, cancellationToken
        ) switch
        {
            RpcResult<string>.Success result => long.Parse(result.Result.AsSpan()[2..], NumberStyles.HexNumber, CultureInfo.InvariantCulture),
            RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<long> EthBlockTransactionCountByNumberAsync(TargetBlockNumber targetBlockNumber, CancellationToken cancellationToken)
        => await SendRpcRequest<string, string>(
            "eth_getBlockTransactionCountByNumber", targetBlockNumber.ToString(), cancellationToken) switch
        {
            RpcResult<string>.Success result => long.Parse(result.Result.AsSpan()[2..], NumberStyles.HexNumber, CultureInfo.InvariantCulture),
            RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<long> EthBlockTransactionCountByNumberAsync(ulong blockNumber, CancellationToken cancellationToken)
        => await EthBlockTransactionCountByNumberAsync(TargetBlockNumber.Height(blockNumber), cancellationToken);

    private record TransactionEthCall(Address? From, Address To, uint? Gas, BigInteger? GasPrice, int? Value, string? Data);
    private record FakeAccountData(string? Balance, string? Nonce, string? Code, object? State, int? StateDiff);

    public async Task<TxCallResult> EthCallAsync(
        Address? from, Address to, uint? gas, BigInteger? gasPrice, int? value, string? data,
        TargetBlockNumber blockNumber, TxStateOverride? stateOverride, CancellationToken cancellationToken)
    {
        var transaction = new TransactionEthCall(from, to, gas, gasPrice, value, data);

        return TxCallResult.ParseFrom(
            await SendRpcRequest<TransactionEthCall, string, Dictionary<Address, OverrideAccount>?, byte[]>(
                "eth_call", transaction, blockNumber.ToString(), stateOverride?._accountOverrides, cancellationToken
            )
        );
    }

    public async Task<TxSubmissionResult> EthSendRawTransactionAsync(string transaction, CancellationToken cancellationToken)
        => await SendRpcRequest<string, string>("eth_sendRawTransaction", transaction, cancellationToken) switch
        {
            RpcResult<string>.Success result => new TxSubmissionResult.Success(result.Result),
            RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<BigInteger> EthGasPriceAsync(CancellationToken cancellationToken)
    {
        var response = await SendRpcRequest<string>("eth_gasPrice", cancellationToken);
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
                if(res != System.Buffers.OperationStatus.Done)
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
        var response = await SendRpcRequest<string>("eth_maxPriorityFeePerGas", cancellationToken);
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
                if(res != System.Buffers.OperationStatus.Done)
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

    public async Task<FeeHistory> EthGetFeeHistory(int blockCount, TargetBlockNumber newestBlock,
        double[] rewardPercentiles, CancellationToken cancellationToken)
        => await SendRpcRequest<int, string, double[], FeeHistory>(
            "eth_feeHistory", blockCount, newestBlock.ToString(), rewardPercentiles, cancellationToken) switch
        {
            RpcResult<FeeHistory>.Success result => result.Result,
            RpcResult<FeeHistory>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    private record EthEstimateGasRequest(Address? From, Address To, BigInteger Value, string Data);
    public async Task<ulong> EthEstimateGasAsync(
        Address? from, Address to, BigInteger value, string data, CancellationToken cancellationToken)
    {
        var transaction = new EthEstimateGasRequest(from, to, value, data);
        return await SendRpcRequest<EthEstimateGasRequest, ulong>(
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
        return await SendRpcRequest<string, bool, BlockData?>(
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
        return await SendRpcRequest<string, bool, BlockDataTrasactionAsString>(
            "eth_getBlockByHash", blockHash, false, cancellationToken) switch
        {
            RpcResult<BlockDataTrasactionAsString>.Success result => result.Result,
            RpcResult<BlockDataTrasactionAsString>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<BlockData?> EthGetFullBlockByNumberAsync(
        TargetBlockNumber targetBlockNumber, CancellationToken cancellationToken)
        => await SendRpcRequest<string, bool, BlockData>(
            "eth_getBlockByNumber", targetBlockNumber.ToString(), false, cancellationToken) switch
        {
            RpcResult<BlockData>.Success result => result.Result,
            RpcResult<BlockData>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<BlockDataTrasactionAsString> EthGetBlockByNumberAsync(
        TargetBlockNumber targetBlockNumber, CancellationToken cancellationToken)
        => await SendRpcRequest<string, bool, BlockDataTrasactionAsString>(
            "eth_getBlockByNumber", targetBlockNumber.ToString(), false, cancellationToken) switch
        {
            RpcResult<BlockDataTrasactionAsString>.Success result => result.Result,
            RpcResult<BlockDataTrasactionAsString>.Error error => throw RPCException.FromRPCError(error),
            RpcResult<BlockDataTrasactionAsString>.Null => throw new RPCException(-1, "block not found, rpc returned null"),
            _ => throw new NotImplementedException(),
        };

    public async Task<Transaction?> EthTransactionByHash(string hash, CancellationToken cancellationToken)
        => await SendRpcRequest<string, Transaction>(
            "eth_getTransactionByHash", hash, cancellationToken) switch
        {
            RpcResult<Transaction>.Success result => result.Result,
            RpcResult<Transaction>.Error error => throw RPCException.FromRPCError(error),
            RpcResult<Transaction>.Null => null,
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
        return await SendRpcRequest<string, int, Transaction>(
            "eth_getTransactionByBlockHashAndIndex", blockHash, index, cancellationToken) switch
        {
            RpcResult<Transaction>.Success result => result.Result,
            RpcResult<Transaction>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<Transaction> EthGetTransactionByBlockNumberAndIndexAsync(
        string blockNumber, int index, CancellationToken cancellationToken)
        => await SendRpcRequest<string, int, Transaction>(
            "eth_getTransactionByBlockNumberAndIndex", blockNumber, index, cancellationToken) switch
        {
            RpcResult<Transaction>.Success result => result.Result,
            RpcResult<Transaction>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<TransactionReceipt?> EthGetTransactionReceiptAsync(string transactionHash, CancellationToken cancellationToken)
        => await SendRpcRequest<string, TransactionReceipt>(
            "eth_getTransactionReceipt", transactionHash, cancellationToken) switch
        {
            RpcResult<TransactionReceipt>.Success result => result.Result,
            RpcResult<TransactionReceipt>.Error error => throw RPCException.FromRPCError(error),
            RpcResult<TransactionReceipt>.Null => null,
            _ => throw new NotImplementedException(),
        };

    public async Task<Uncle?> EthGetUncleByBlockHashAndIndexAsync(
        string blockHash, int uncleIndex, CancellationToken cancellationToken)
        => await SendRpcRequest<string, int, Uncle>(
            "eth_getUncleByBlockHashAndIndex", blockHash, uncleIndex, cancellationToken) switch
        {
            RpcResult<Uncle>.Success result => result.Result,
            RpcResult<Uncle>.Error error => throw RPCException.FromRPCError(error),
            RpcResult<Uncle>.Null _ => null,
            _ => throw new NotImplementedException(),
        };

    public async Task<Uncle?> EthGetUncleByBlockNumberAndIndexAsync(
        TargetBlockNumber targetBlockNumber, uint uncleIndex, CancellationToken cancellationToken)
        => await SendRpcRequest<string, uint, Uncle>(
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
        Address[]? Address,
        string[]?[]? Topics
    );
    public async Task<string> EthNewFilterAsync(
        TargetBlockNumber fromBlock, TargetBlockNumber toBlock,
        Address[]? address, string[]?[]? topics,
        CancellationToken cancellationToken)
    {
        if(!_transport.SupportsFilters)
        {
            throw new InvalidOperationException("The underlying transport does not support filters");
        }

        var filterOptions = new EthNewFilterRequest(fromBlock.ToString(), toBlock.ToString(), address, topics);
        return await SendRpcRequest<EthNewFilterRequest, string>(
            "eth_newFilter", filterOptions, cancellationToken) switch
        {
            RpcResult<string>.Success result => result.Result,
            RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<string> EthNewBlockFilterAsync(CancellationToken cancellationToken)
        => await SendRpcRequest<string>(
            "eth_newBlockFilter", cancellationToken) switch
        {
            RpcResult<string>.Success result => result.Result,
            RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<string> EthNewPendingTransactionFilterAsync(CancellationToken cancellationToken)
        => await SendRpcRequest<string>(
            "eth_newPendingTransactionFilter", cancellationToken) switch
        {
            RpcResult<string>.Success result => result.Result,
            RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<bool> EthUninstallFilterAsync(string filterId, CancellationToken cancellationToken)
        => await SendRpcRequest<string, bool>(
            "eth_uninstallFilter", filterId, cancellationToken) switch
        {
            RpcResult<bool>.Success result => result.Result,
            RpcResult<bool>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<List<string?>> EthGetPendingTransactionFilterChangesAsync(string filterId, CancellationToken cancellationToken)
        => await SendRpcRequest<string, List<string?>>(
            "eth_getFilterChanges", filterId, cancellationToken) switch
        {
            RpcResult<List<string?>>.Success result => result.Result,
            RpcResult<List<string?>>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<Log[]> EthGetEventFilterChangesAsync(string filterId, CancellationToken cancellationToken)
        => await SendRpcRequest<string, Log[]>(
            "eth_getFilterChanges", filterId, cancellationToken) switch
        {
            RpcResult<Log[]>.Success result => result.Result,
            RpcResult<Log[]>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    private record EthGetLogsRequest(
        string FromBlock,
        string ToBlock,
        Address[]? Address,
        string[]?[]? Topics,
        string? BlockHash
    );
    public async Task<Log[]> EthGetLogsAsync(
        TargetBlockNumber fromBlock, TargetBlockNumber toBlock,
        Address[]? addresses, string[]?[]? topics, string? blockHash,
        CancellationToken cancellationToken)
    {
        var filterOptions = new EthGetLogsRequest(fromBlock.ToString(), toBlock.ToString(), addresses, topics, blockHash);
        return await SendRpcRequest<EthGetLogsRequest, Log[]>(
            "eth_getLogs", filterOptions, cancellationToken) switch
        {
            RpcResult<Log[]>.Success result => result.Result,
            RpcResult<Log[]>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    private record EthSubscribeLogsRequest(Address[]? Address, string[]?[]? Topics);
    public async Task<string> EthSubscribeLogsAsync(Address[]? contracts, string[]?[]? topics, CancellationToken cancellationToken)
    {
        if(!_transport.SupportsSubscriptions)
        {
            throw new InvalidOperationException("The underlying transport does not support subscriptions");
        }

        var request = new EthSubscribeLogsRequest(contracts, topics);
        return await SendRpcRequest<string, EthSubscribeLogsRequest, string>(
            "eth_subscribe", "logs", request, cancellationToken) switch
        {
            RpcResult<string>.Success result => result.Result,
            RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<string> EthSubscribeNewHeadsAsync(CancellationToken cancellationToken = default)
    {
        if(!_transport.SupportsSubscriptions)
        {
            throw new InvalidOperationException("The underlying transport does not support subscriptions");
        }
        //
        return await SendRpcRequest<string, string>(
                    "eth_subscribe", "newHeads", cancellationToken) switch
        {
            RpcResult<string>.Success result => result.Result,
            RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<bool> EthUnsubscribeAsync(string subscriptionId, CancellationToken cancellationToken)
    {
        if(!_transport.SupportsSubscriptions)
        {
            throw new InvalidOperationException("The underlying transport does not support subscriptions");
        }
        //
        return await SendRpcRequest<string, bool>(
                "eth_unsubscribe", subscriptionId, cancellationToken) switch
        {
            RpcResult<bool>.Success result => result.Result,
            RpcResult<bool>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }
}