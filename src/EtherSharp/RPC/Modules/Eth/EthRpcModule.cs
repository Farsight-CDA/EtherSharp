using EtherSharp.Client.Services.TxPublisher;
using EtherSharp.Common.Exceptions;
using EtherSharp.Types;
using System.Globalization;
using System.Numerics;

namespace EtherSharp.RPC.Modules.Eth;
internal class EthRpcModule(IRpcClient rpcClient) : IEthRpcModule
{
    private readonly IRpcClient _rpcClient = rpcClient;

    public async Task<ulong> ChainIdAsync(CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<ulong>("eth_chainId", cancellationToken) switch
        {
            RpcResult<ulong>.Success result => result.Result,
            RpcResult<ulong>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<ulong> BlockNumberAsync(CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<string>("eth_blockNumber", cancellationToken) switch
        {
            RpcResult<string>.Success result => ulong.Parse(result.Result.AsSpan()[2..], NumberStyles.HexNumber, CultureInfo.InvariantCulture),
            RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<BigInteger> GetBalanceAsync(Address address, TargetBlockNumber blockNumber, CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<Address, string, BigInteger>("eth_getBalance", address, blockNumber.ToString(), cancellationToken) switch
        {
            RpcResult<BigInteger>.Success result => result.Result,
            RpcResult<BigInteger>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<uint> GetTransactionCountAsync(Address address, TargetBlockNumber blockNumber, CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<Address, string, uint>(
            "eth_getTransactionCount", address, blockNumber.ToString(), cancellationToken) switch
        {
            RpcResult<uint>.Success result => result.Result,
            RpcResult<uint>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<string[]> AccountsAsync(CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<string[]>("eth_accounts", cancellationToken) switch
        {
            RpcResult<string[]>.Success result => result.Result,
            RpcResult<string[]>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<long> BlockTransactionCountByHashAsync(string blockHash, CancellationToken cancellationToken)
    {
        if(blockHash.Length != 66)
        {
            throw new InvalidOperationException("Blockhash string must be 32 Bytes");
        }
        //
        return await _rpcClient.SendRpcRequestAsync<string, string>(
            "eth_getBlockTransactionCountByHash", blockHash, cancellationToken
        ) switch
        {
            RpcResult<string>.Success result => long.Parse(result.Result.AsSpan()[2..], NumberStyles.HexNumber, CultureInfo.InvariantCulture),
            RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<long> BlockTransactionCountByNumberAsync(TargetBlockNumber targetBlockNumber, CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<string, string>(
            "eth_getBlockTransactionCountByNumber", targetBlockNumber.ToString(), cancellationToken) switch
        {
            RpcResult<string>.Success result => long.Parse(result.Result.AsSpan()[2..], NumberStyles.HexNumber, CultureInfo.InvariantCulture),
            RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<long> BlockTransactionCountByNumberAsync(ulong blockNumber, CancellationToken cancellationToken)
        => await BlockTransactionCountByNumberAsync(TargetBlockNumber.Height(blockNumber), cancellationToken);

    private record TransactionCall(Address? From, Address To, uint? Gas, BigInteger? GasPrice, BigInteger Value, string? Data);
    private record FakeAccountData(string? Balance, string? Nonce, string? Code, object? State, int? StateDiff);

    public async Task<TxCallResult> CallAsync(
        Address? from, Address to, uint? gas, BigInteger? gasPrice, BigInteger value, string? data,
        TargetBlockNumber blockNumber, CancellationToken cancellationToken)
    {
        var transaction = new TransactionCall(from, to, gas, gasPrice, value, data);

        return TxCallResult.ParseFrom(
            await _rpcClient.SendRpcRequestAsync<TransactionCall, string, byte[]>(
                "eth_call", transaction, blockNumber.ToString(), cancellationToken
            )
        );
    }

    public async Task<TxSubmissionResult> SendRawTransactionAsync(string transaction, CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<string, string>("eth_sendRawTransaction", transaction, cancellationToken) switch
        {
            RpcResult<string>.Success result => new TxSubmissionResult.Success(result.Result),
            RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<BigInteger> GasPriceAsync(CancellationToken cancellationToken)
    {
        var response = await _rpcClient.SendRpcRequestAsync<string>("eth_gasPrice", cancellationToken);
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

    public async Task<BigInteger> MaxPriorityFeePerGasAsync(CancellationToken cancellationToken)
    {
        var response = await _rpcClient.SendRpcRequestAsync<string>("eth_maxPriorityFeePerGas", cancellationToken);
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

    public async Task<FeeHistory> GetFeeHistoryAsync(int blockCount, TargetBlockNumber newestBlock,
        double[] rewardPercentiles, CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<int, string, double[], FeeHistory>(
            "eth_feeHistory", blockCount, newestBlock.ToString(), rewardPercentiles, cancellationToken) switch
        {
            RpcResult<FeeHistory>.Success result => result.Result,
            RpcResult<FeeHistory>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    private record EstimateGasRequest(Address? From, Address To, BigInteger Value, string Data);
    public async Task<ulong> EstimateGasAsync(
        Address? from, Address to, BigInteger value, string data, CancellationToken cancellationToken)
    {
        var transaction = new EstimateGasRequest(from, to, value, data);
        return await _rpcClient.SendRpcRequestAsync<EstimateGasRequest, ulong>(
            "eth_estimateGas", transaction, cancellationToken) switch
        {
            RpcResult<ulong>.Success result => result.Result,
            RpcResult<ulong>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<BlockData?> GetFullBlockByHashAsync(string blockHash, CancellationToken cancellationToken)
    {
        if(blockHash.Length != 66)
        {
            throw new InvalidOperationException("Blockhash string must be 32 Bytes");
        }
        //
        return await _rpcClient.SendRpcRequestAsync<string, bool, BlockData?>(
            "eth_getBlockByHash", blockHash, true, cancellationToken) switch
        {
            RpcResult<BlockData>.Success result => result.Result,
            RpcResult<BlockData>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<BlockDataTrasactionAsString?> GetBlockTransactionsByHashAsync(
        string blockHash, CancellationToken cancellationToken)
    {
        if(blockHash.Length != 66)
        {
            throw new InvalidOperationException("Blockhash string must be 32 Bytes");
        }
        //
        return await _rpcClient.SendRpcRequestAsync<string, bool, BlockDataTrasactionAsString>(
            "eth_getBlockByHash", blockHash, false, cancellationToken) switch
        {
            RpcResult<BlockDataTrasactionAsString>.Success result => result.Result,
            RpcResult<BlockDataTrasactionAsString>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<BlockData?> GetFullBlockByNumberAsync(
        TargetBlockNumber targetBlockNumber, CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<string, bool, BlockData>(
            "eth_getBlockByNumber", targetBlockNumber.ToString(), false, cancellationToken) switch
        {
            RpcResult<BlockData>.Success result => result.Result,
            RpcResult<BlockData>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<BlockDataTrasactionAsString> GetBlockByNumberAsync(
        TargetBlockNumber targetBlockNumber, CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<string, bool, BlockDataTrasactionAsString>(
            "eth_getBlockByNumber", targetBlockNumber.ToString(), false, cancellationToken) switch
        {
            RpcResult<BlockDataTrasactionAsString>.Success result => result.Result,
            RpcResult<BlockDataTrasactionAsString>.Error error => throw RPCException.FromRPCError(error),
            RpcResult<BlockDataTrasactionAsString>.Null => throw new RPCException(-1, "block not found, rpc returned null", null),
            _ => throw new NotImplementedException(),
        };

    public async Task<Transaction?> TransactionByHashAsync(string hash, CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<string, Transaction>(
            "eth_getTransactionByHash", hash, cancellationToken) switch
        {
            RpcResult<Transaction>.Success result => result.Result,
            RpcResult<Transaction>.Error error => throw RPCException.FromRPCError(error),
            RpcResult<Transaction>.Null => null,
            _ => throw new NotImplementedException(),
        };

    public async Task<Transaction> GetTransactionByBlockHashAndIndexAsync(
        string blockHash, int index, CancellationToken cancellationToken)
    {
        if(blockHash.Length != 66)
        {
            throw new InvalidOperationException("Blockhash string must be 32 Bytes");
        }
        //
        return await _rpcClient.SendRpcRequestAsync<string, int, Transaction>(
            "eth_getTransactionByBlockHashAndIndex", blockHash, index, cancellationToken) switch
        {
            RpcResult<Transaction>.Success result => result.Result,
            RpcResult<Transaction>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<Transaction> GetTransactionByBlockNumberAndIndexAsync(
        string blockNumber, int index, CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<string, int, Transaction>(
            "eth_getTransactionByBlockNumberAndIndex", blockNumber, index, cancellationToken) switch
        {
            RpcResult<Transaction>.Success result => result.Result,
            RpcResult<Transaction>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<TransactionReceipt?> GetTransactionReceiptAsync(string transactionHash, CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<string, TransactionReceipt>(
            "eth_getTransactionReceipt", transactionHash, cancellationToken) switch
        {
            RpcResult<TransactionReceipt>.Success result => result.Result,
            RpcResult<TransactionReceipt>.Error error => throw RPCException.FromRPCError(error),
            RpcResult<TransactionReceipt>.Null => null,
            _ => throw new NotImplementedException(),
        };

    public async Task<Uncle?> GetUncleByBlockHashAndIndexAsync(
        string blockHash, int uncleIndex, CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<string, int, Uncle>(
            "eth_getUncleByBlockHashAndIndex", blockHash, uncleIndex, cancellationToken) switch
        {
            RpcResult<Uncle>.Success result => result.Result,
            RpcResult<Uncle>.Error error => throw RPCException.FromRPCError(error),
            RpcResult<Uncle>.Null _ => null,
            _ => throw new NotImplementedException(),
        };

    public async Task<Uncle?> GetUncleByBlockNumberAndIndexAsync(
        TargetBlockNumber targetBlockNumber, uint uncleIndex, CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<string, uint, Uncle>(
            "eth_getUncleByBlockNumberAndIndex", targetBlockNumber.ToString(), uncleIndex, cancellationToken) switch
        {
            RpcResult<Uncle>.Success result => result.Result,
            RpcResult<Uncle>.Error error => throw RPCException.FromRPCError(error),
            RpcResult<Uncle>.Null _ => null,
            _ => throw new NotImplementedException(),
        };

    private record NewFilterRequest(
        string FromBlock,
        string ToBlock,
        Address[]? Address,
        string[]?[]? Topics
    );
    public async Task<string> NewFilterAsync(
        TargetBlockNumber fromBlock, TargetBlockNumber toBlock,
        Address[]? address, string[]?[]? topics,
        CancellationToken cancellationToken)
    {
        if(!_rpcClient.SupportsFilters)
        {
            throw new InvalidOperationException("The underlying transport does not support filters");
        }

        var filterOptions = new NewFilterRequest(fromBlock.ToString(), toBlock.ToString(), address, topics);
        return await _rpcClient.SendRpcRequestAsync<NewFilterRequest, string>(
            "eth_newFilter", filterOptions, cancellationToken) switch
        {
            RpcResult<string>.Success result => result.Result,
            RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<string> NewBlockFilterAsync(CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<string>(
            "eth_newBlockFilter", cancellationToken) switch
        {
            RpcResult<string>.Success result => result.Result,
            RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<string> NewPendingTransactionFilterAsync(CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<string>(
            "eth_newPendingTransactionFilter", cancellationToken) switch
        {
            RpcResult<string>.Success result => result.Result,
            RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<bool> UninstallFilterAsync(string filterId, CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<string, bool>(
            "eth_uninstallFilter", filterId, cancellationToken) switch
        {
            RpcResult<bool>.Success result => result.Result,
            RpcResult<bool>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<List<string?>> GetPendingTransactionFilterChangesAsync(string filterId, CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<string, List<string?>>(
            "eth_getFilterChanges", filterId, cancellationToken) switch
        {
            RpcResult<List<string?>>.Success result => result.Result,
            RpcResult<List<string?>>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<Log[]> GetEventFilterChangesAsync(string filterId, CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<string, Log[]>(
            "eth_getFilterChanges", filterId, cancellationToken) switch
        {
            RpcResult<Log[]>.Success result => result.Result,
            RpcResult<Log[]>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    private record GetLogsRequest(
        string FromBlock,
        string ToBlock,
        Address[]? Address,
        string[]?[]? Topics,
        string? BlockHash
    );
    public async Task<Log[]> GetLogsAsync(
        TargetBlockNumber fromBlock, TargetBlockNumber toBlock,
        Address[]? addresses, string[]?[]? topics, string? blockHash,
        CancellationToken cancellationToken)
    {
        var filterOptions = new GetLogsRequest(fromBlock.ToString(), toBlock.ToString(), addresses, topics, blockHash);
        return await _rpcClient.SendRpcRequestAsync<GetLogsRequest, Log[]>(
            "eth_getLogs", filterOptions, cancellationToken) switch
        {
            RpcResult<Log[]>.Success result => result.Result,
            RpcResult<Log[]>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    private record SubscribeLogsRequest(Address[]? Address, string[]?[]? Topics);
    public async Task<string> SubscribeLogsAsync(Address[]? contracts, string[]?[]? topics, CancellationToken cancellationToken)
    {
        if(!_rpcClient.SupportsSubscriptions)
        {
            throw new InvalidOperationException("The underlying transport does not support subscriptions");
        }

        var request = new SubscribeLogsRequest(contracts, topics);
        return await _rpcClient.SendRpcRequestAsync<string, SubscribeLogsRequest, string>(
            "eth_subscribe", "logs", request, cancellationToken) switch
        {
            RpcResult<string>.Success result => result.Result,
            RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<string> SubscribeNewHeadsAsync(CancellationToken cancellationToken = default)
    {
        if(!_rpcClient.SupportsSubscriptions)
        {
            throw new InvalidOperationException("The underlying transport does not support subscriptions");
        }
        //
        return await _rpcClient.SendRpcRequestAsync<string, string>(
                    "eth_subscribe", "newHeads", cancellationToken) switch
        {
            RpcResult<string>.Success result => result.Result,
            RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<bool> UnsubscribeAsync(string subscriptionId, CancellationToken cancellationToken)
    {
        if(!_rpcClient.SupportsSubscriptions)
        {
            throw new InvalidOperationException("The underlying transport does not support subscriptions");
        }
        //
        return await _rpcClient.SendRpcRequestAsync<string, bool>(
                "eth_unsubscribe", subscriptionId, cancellationToken) switch
        {
            RpcResult<bool>.Success result => result.Result,
            RpcResult<bool>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }
}
