using EtherSharp.Client.Services.TxPublisher;
using EtherSharp.Common.Exceptions;
using EtherSharp.Numerics;
using EtherSharp.Types;
using System.Globalization;

namespace EtherSharp.RPC.Modules.Eth;

internal class EthRpcModule(IRpcClient rpcClient) : IEthRpcModule
{
    private readonly IRpcClient _rpcClient = rpcClient;

    public async Task<ulong> ChainIdAsync(CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<ulong>("eth_chainId", TargetBlockNumber.Latest, cancellationToken) switch
        {
            RpcResult<ulong>.Success result => result.Result,
            RpcResult<ulong>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<ulong> BlockNumberAsync(CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<string>("eth_blockNumber", TargetBlockNumber.Latest, cancellationToken) switch
        {
            RpcResult<string>.Success result => UInt64.Parse(result.Result.AsSpan()[2..], NumberStyles.HexNumber, CultureInfo.InvariantCulture),
            RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<UInt256> GetBalanceAsync(Address address, TargetBlockNumber targetHeight, CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<Address, TargetBlockNumber, UInt256>(
            "eth_getBalance", address, targetHeight, targetHeight, cancellationToken) switch
        {
            RpcResult<UInt256>.Success result => result.Result,
            RpcResult<UInt256>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<uint> GetTransactionCountAsync(Address address, TargetBlockNumber targetHeight, CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<Address, TargetBlockNumber, uint>(
            "eth_getTransactionCount", address, targetHeight, targetHeight, cancellationToken) switch
        {
            RpcResult<uint>.Success result => result.Result,
            RpcResult<uint>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<long> BlockTransactionCountByNumberAsync(TargetBlockNumber targetHeight, CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<TargetBlockNumber, string>(
            "eth_getBlockTransactionCountByNumber", targetHeight, targetHeight, cancellationToken) switch
        {
            RpcResult<string>.Success result => Int64.Parse(result.Result.AsSpan()[2..], NumberStyles.HexNumber, CultureInfo.InvariantCulture),
            RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    private record TransactionCall(Address? From, Address? To, uint? Gas, UInt256? GasPrice, UInt256 Value, string? Data);
    public async Task<TxCallResult> CallAsync(
        Address? from, Address? to, uint? gas, UInt256? gasPrice, UInt256 value, string? data,
        TargetBlockNumber targetHeight, CancellationToken cancellationToken)
    {
        var transaction = new TransactionCall(from, to, gas, gasPrice, value, data);

        return TxCallResult.ParseFrom(
            await _rpcClient.SendRpcRequestAsync<TransactionCall, TargetBlockNumber, byte[]>(
                "eth_call", transaction, targetHeight, targetHeight, cancellationToken
            )
        );
    }

    public async Task<TxSubmissionResult> SendRawTransactionAsync(string transaction, CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<string, string>("eth_sendRawTransaction", transaction, TargetBlockNumber.Latest, cancellationToken) switch
        {
            RpcResult<string>.Success result => new TxSubmissionResult.Success(result.Result),
            RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<UInt256> GasPriceAsync(CancellationToken cancellationToken)
    {
        var response = await _rpcClient.SendRpcRequestAsync<UInt256>("eth_gasPrice", TargetBlockNumber.Latest, cancellationToken);
        return response switch
        {
            RpcResult<UInt256>.Success result => result.Result,
            RpcResult<UInt256>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<UInt256> MaxPriorityFeePerGasAsync(CancellationToken cancellationToken)
    {
        var response = await _rpcClient.SendRpcRequestAsync<UInt256>("eth_maxPriorityFeePerGas", TargetBlockNumber.Latest, cancellationToken);
        return response switch
        {
            RpcResult<UInt256>.Success result => result.Result,
            RpcResult<UInt256>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<FeeHistory> GetFeeHistoryAsync(int blockCount, TargetBlockNumber newestBlock,
        double[] rewardPercentiles, CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<int, string, double[], FeeHistory>(
            //ToDo: Calculate proper required block height
            "eth_feeHistory", blockCount, newestBlock.ToString(), rewardPercentiles, newestBlock, cancellationToken) switch
        {
            RpcResult<FeeHistory>.Success result => result.Result,
            RpcResult<FeeHistory>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    private record EstimateGasRequest(Address? From, Address? To, UInt256 Value, string Data);
    public async Task<ulong> EstimateGasAsync(
        Address? from, Address? to, UInt256 value, string data, CancellationToken cancellationToken)
    {
        var transaction = new EstimateGasRequest(from, to, value, data);
        return await _rpcClient.SendRpcRequestAsync<EstimateGasRequest, ulong>(
            "eth_estimateGas", transaction, TargetBlockNumber.Latest, cancellationToken) switch
        {
            RpcResult<ulong>.Success result => result.Result,
            RpcResult<ulong>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<BlockData?> GetFullBlockByNumberAsync(
        TargetBlockNumber targetHeight, CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<TargetBlockNumber, bool, BlockData>(
            "eth_getBlockByNumber", targetHeight, true, targetHeight, cancellationToken) switch
        {
            RpcResult<BlockData>.Success result => result.Result,
            RpcResult<BlockData>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<BlockDataTrasactionAsString> GetBlockByNumberAsync(
        TargetBlockNumber targetHeight, CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<TargetBlockNumber, bool, BlockDataTrasactionAsString>(
            "eth_getBlockByNumber", targetHeight, false, targetHeight, cancellationToken) switch
        {
            RpcResult<BlockDataTrasactionAsString>.Success result => result.Result,
            RpcResult<BlockDataTrasactionAsString>.Error error => throw RPCException.FromRPCError(error),
            RpcResult<BlockDataTrasactionAsString>.Null => throw new RPCException(-1, "block not found, rpc returned null", null),
            _ => throw new NotImplementedException(),
        };

    public async Task<Transaction?> TransactionByHashAsync(string hash, CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<string, Transaction>(
            //ToDo: Add notion of unspecified required block height
            "eth_getTransactionByHash", hash, TargetBlockNumber.Latest, cancellationToken) switch
        {
            RpcResult<Transaction>.Success result => result.Result,
            RpcResult<Transaction>.Error error => throw RPCException.FromRPCError(error),
            RpcResult<Transaction>.Null => null,
            _ => throw new NotImplementedException(),
        };

    public async Task<TransactionReceipt?> GetTransactionReceiptAsync(string transactionHash, CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<string, TransactionReceipt>(
            "eth_getTransactionReceipt", transactionHash, TargetBlockNumber.Latest, cancellationToken) switch
        {
            RpcResult<TransactionReceipt>.Success result => result.Result,
            RpcResult<TransactionReceipt>.Error error => throw RPCException.FromRPCError(error),
            RpcResult<TransactionReceipt>.Null => null,
            _ => throw new NotImplementedException(),
        };

    public async Task<byte[]> GetStorageAtAsync(
        Address address, byte[] slot, TargetBlockNumber targetHeight = default, CancellationToken cancellationToken = default)
        => await _rpcClient.SendRpcRequestAsync<string, byte[], TargetBlockNumber, byte[]>(
            "eth_getStorageAt", address.String, slot, targetHeight, targetHeight, cancellationToken) switch
        {
            RpcResult<byte[]>.Success result => result.Result,
            RpcResult<byte[]>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    private record NewFilterRequest(
        TargetBlockNumber FromBlock,
        TargetBlockNumber ToBlock,
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

        var filterOptions = new NewFilterRequest(fromBlock, toBlock, address, topics);
        return await _rpcClient.SendRpcRequestAsync<NewFilterRequest, string>(
            "eth_newFilter", filterOptions, fromBlock, cancellationToken) switch
        {
            RpcResult<string>.Success result => result.Result,
            RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<string> NewBlockFilterAsync(CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<string>(
            "eth_newBlockFilter", TargetBlockNumber.Latest, cancellationToken) switch
        {
            RpcResult<string>.Success result => result.Result,
            RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<string> NewPendingTransactionFilterAsync(CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<string>(
            "eth_newPendingTransactionFilter", TargetBlockNumber.Latest, cancellationToken) switch
        {
            RpcResult<string>.Success result => result.Result,
            RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<bool> UninstallFilterAsync(string filterId, CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<string, bool>(
            "eth_uninstallFilter", filterId, TargetBlockNumber.Latest, cancellationToken) switch
        {
            RpcResult<bool>.Success result => result.Result,
            RpcResult<bool>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<List<string?>> GetPendingTransactionFilterChangesAsync(string filterId, CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<string, List<string?>>(
            "eth_getFilterChanges", filterId, TargetBlockNumber.Latest, cancellationToken) switch
        {
            RpcResult<List<string?>>.Success result => result.Result,
            RpcResult<List<string?>>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<Log[]> GetEventFilterChangesAsync(string filterId, CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<string, Log[]>(
            "eth_getFilterChanges", filterId, TargetBlockNumber.Latest, cancellationToken) switch
        {
            RpcResult<Log[]>.Success result => result.Result,
            RpcResult<Log[]>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    private record GetLogsRequest(
        TargetBlockNumber FromBlock,
        TargetBlockNumber ToBlock,
        Address[]? Address,
        string[]?[]? Topics,
        string? BlockHash
    );
    public async Task<Log[]> GetLogsAsync(
        TargetBlockNumber fromBlock, TargetBlockNumber toBlock,
        Address[]? addresses, string[]?[]? topics, string? blockHash,
        CancellationToken cancellationToken)
    {
        var filterOptions = new GetLogsRequest(fromBlock, toBlock, addresses, topics, blockHash);
        return await _rpcClient.SendRpcRequestAsync<GetLogsRequest, Log[]>(
            "eth_getLogs", filterOptions, fromBlock, cancellationToken) switch
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
            "eth_subscribe", "logs", request, TargetBlockNumber.Latest, cancellationToken) switch
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
                    "eth_subscribe", "newHeads", TargetBlockNumber.Latest, cancellationToken) switch
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
                "eth_unsubscribe", subscriptionId, TargetBlockNumber.Latest, cancellationToken) switch
        {
            RpcResult<bool>.Success result => result.Result,
            RpcResult<bool>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }
}
