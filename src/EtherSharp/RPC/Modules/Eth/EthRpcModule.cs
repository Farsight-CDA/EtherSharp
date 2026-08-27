using EtherSharp.Client;
using EtherSharp.Client.Services.TxPublisher;
using EtherSharp.Common.Exceptions;
using EtherSharp.Numerics;
using EtherSharp.RPC.Transport;
using EtherSharp.Tx;
using EtherSharp.Types;
using System.Globalization;

namespace EtherSharp.RPC.Modules.Eth;

internal sealed class EthRpcModule(RpcClient rpcClient, IRPCTransport rpcTransport, CallGasLimitSettings callGasLimitSettings) : IEthRpcModule
{
    private readonly RpcClient _rpcClient = rpcClient;
    private readonly IRPCTransport _rpcTransport = rpcTransport;
    private readonly CallGasLimitSettings _callGasLimitSettings = callGasLimitSettings;

    public async Task<ulong> ChainIdAsync(RpcRequestOptions requestOptions, CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<ulong>("eth_chainId", TargetHeight.Latest, requestOptions, cancellationToken) switch
        {
            RpcResult<ulong>.Success result => result.Result,
            RpcResult<ulong>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<ulong> BlockNumberAsync(RpcRequestOptions requestOptions, CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<string>("eth_blockNumber", TargetHeight.Latest, requestOptions, cancellationToken) switch
        {
            RpcResult<string>.Success result => UInt64.Parse(result.Result.AsSpan()[2..], NumberStyles.HexNumber, CultureInfo.InvariantCulture),
            RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public Task<UInt256> GetBalanceAsync(in Address address, TargetHeight targetHeight, RpcRequestOptions requestOptions, CancellationToken cancellationToken)
    {
        var addressValue = address;
        return GetBalanceCoreAsync(addressValue, targetHeight, requestOptions, cancellationToken);
    }

    private async Task<UInt256> GetBalanceCoreAsync(Address address, TargetHeight targetHeight, RpcRequestOptions requestOptions, CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<Address, TargetHeight, UInt256>(
            "eth_getBalance", address, targetHeight, targetHeight, requestOptions, cancellationToken) switch
        {
            RpcResult<UInt256>.Success result => result.Result,
            RpcResult<UInt256>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public Task<uint> GetTransactionCountAsync(in Address address, TargetHeight targetHeight, RpcRequestOptions requestOptions, CancellationToken cancellationToken)
    {
        var addressValue = address;
        return GetTransactionCountCoreAsync(addressValue, targetHeight, requestOptions, cancellationToken);
    }

    private async Task<uint> GetTransactionCountCoreAsync(Address address, TargetHeight targetHeight, RpcRequestOptions requestOptions, CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<Address, TargetHeight, uint>(
            "eth_getTransactionCount", address, targetHeight, targetHeight, requestOptions, cancellationToken) switch
        {
            RpcResult<uint>.Success result => result.Result,
            RpcResult<uint>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<long> BlockTransactionCountByNumberAsync(TargetHeight targetHeight, RpcRequestOptions requestOptions, CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<TargetHeight, string>(
            "eth_getBlockTransactionCountByNumber", targetHeight, targetHeight, requestOptions, cancellationToken) switch
        {
            RpcResult<string>.Success result => Int64.Parse(result.Result.AsSpan()[2..], NumberStyles.HexNumber, CultureInfo.InvariantCulture),
            RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    // Keep RPC DTOs as record classes: readonly record structs produced identical JSON but no allocation reduction and mixed serialization throughput.
    private sealed record TransactionCall(Address? From, Address? To, ulong? Gas, UInt256? GasPrice, UInt256 Value, ReadOnlyMemory<byte> Data);
    public Task<TxCallResult> CallAsync(
        Address? to, ulong? gas, UInt256? gasPrice, UInt256 value, ReadOnlyMemory<byte> data,
        in CallOptions options, RpcRequestOptions requestOptions, CancellationToken cancellationToken)
    {
        return SendAsync(
            _rpcClient,
            new TransactionCall(
                options.From, to, gas ?? _callGasLimitSettings.GetEthCallGasLimit(), gasPrice, value, data
            ),
            options.TargetHeight,
            options.StateOverrides,
            options.BlockOverrides,
            requestOptions,
            cancellationToken
        );

        // Avoid CallOptions in the async state machine.
        static async Task<TxCallResult> SendAsync(
            RpcClient rpcClient, TransactionCall transaction, TargetHeight targetHeight,
            IReadOnlyDictionary<Address, AccountOverride>? stateOverrides, BlockOverride? blockOverrides,
            RpcRequestOptions requestOptions, CancellationToken cancellationToken)
        {
            var result = (stateOverrides, blockOverrides) switch
            {
                (_, not null) => await rpcClient.SendRpcRequestAsync<TransactionCall, TargetHeight, IReadOnlyDictionary<Address, AccountOverride>?, BlockOverride, byte[]>(
                    "eth_call", transaction, targetHeight, stateOverrides, blockOverrides, targetHeight, requestOptions, cancellationToken),
                (not null, null) => await rpcClient.SendRpcRequestAsync<TransactionCall, TargetHeight, IReadOnlyDictionary<Address, AccountOverride>, byte[]>(
                    "eth_call", transaction, targetHeight, stateOverrides, targetHeight, requestOptions, cancellationToken),
                _ => await rpcClient.SendRpcRequestAsync<TransactionCall, TargetHeight, byte[]>(
                    "eth_call", transaction, targetHeight, targetHeight, requestOptions, cancellationToken),
            };

            return TxCallResult.ParseFrom(result);
        }
    }

    public async Task<TxSubmissionResult> SendRawTransactionAsync(string transaction, RpcRequestOptions requestOptions, CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<string, Bytes32>("eth_sendRawTransaction", transaction, TargetHeight.Latest, requestOptions, cancellationToken) switch
        {
            RpcResult<Bytes32>.Success result => new TxSubmissionResult.Success(result.Result),
            RpcResult<Bytes32>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<UInt256> GasPriceAsync(RpcRequestOptions requestOptions, CancellationToken cancellationToken)
    {
        var response = await _rpcClient.SendRpcRequestAsync<UInt256>("eth_gasPrice", TargetHeight.Latest, requestOptions, cancellationToken);
        return response switch
        {
            RpcResult<UInt256>.Success result => result.Result,
            RpcResult<UInt256>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<UInt256> MaxPriorityFeePerGasAsync(RpcRequestOptions requestOptions, CancellationToken cancellationToken)
    {
        var response = await _rpcClient.SendRpcRequestAsync<UInt256>("eth_maxPriorityFeePerGas", TargetHeight.Latest, requestOptions, cancellationToken);
        return response switch
        {
            RpcResult<UInt256>.Success result => result.Result,
            RpcResult<UInt256>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<FeeHistory> GetFeeHistoryAsync(int blockCount, TargetHeight newestBlock,
        double[] rewardPercentiles, RpcRequestOptions requestOptions, CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<int, TargetHeight, double[], FeeHistory>(
            //ToDo: Calculate proper required block height
            "eth_feeHistory", blockCount, newestBlock, rewardPercentiles, newestBlock, requestOptions, cancellationToken) switch
        {
            RpcResult<FeeHistory>.Success result => result.Result,
            RpcResult<FeeHistory>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    // Keep RPC DTOs as record classes: readonly record structs produced identical JSON but no allocation reduction and mixed serialization throughput.
    internal sealed record EstimateGasRequest(
        Address? From, Address? To, UInt256 Value, ReadOnlyMemory<byte> Data, StateAccess[]? AccessList);

    public Task<ulong> EstimateGasAsync(
        Address? to, UInt256 value, ReadOnlyMemory<byte> data, StateAccess[]? accessList,
        in CallOptions options,
        RpcRequestOptions requestOptions, CancellationToken cancellationToken)
    {
        return SendAsync(
            _rpcClient,
            new EstimateGasRequest(options.From, to, value, data, accessList),
            options.TargetHeight,
            options.StateOverrides,
            options.BlockOverrides,
            requestOptions,
            cancellationToken
        );

        // Avoid CallOptions in the async state machine.
        static async Task<ulong> SendAsync(
            RpcClient rpcClient, EstimateGasRequest transaction, TargetHeight targetHeight,
            IReadOnlyDictionary<Address, AccountOverride>? stateOverrides, BlockOverride? blockOverrides,
            RpcRequestOptions requestOptions, CancellationToken cancellationToken)
        {
            var response = (targetHeight == TargetHeight.Latest, stateOverrides, blockOverrides) switch
            {
                (_, _, not null) => await rpcClient.SendRpcRequestAsync<EstimateGasRequest, TargetHeight, IReadOnlyDictionary<Address, AccountOverride>?, BlockOverride, ulong>(
                    "eth_estimateGas", transaction, targetHeight, stateOverrides, blockOverrides, targetHeight, requestOptions, cancellationToken),
                (_, not null, null) => await rpcClient.SendRpcRequestAsync<EstimateGasRequest, TargetHeight, IReadOnlyDictionary<Address, AccountOverride>, ulong>(
                    "eth_estimateGas", transaction, targetHeight, stateOverrides, targetHeight, requestOptions, cancellationToken),
                (false, null, null) => await rpcClient.SendRpcRequestAsync<EstimateGasRequest, TargetHeight, ulong>(
                    "eth_estimateGas", transaction, targetHeight, targetHeight, requestOptions, cancellationToken),
                _ => await rpcClient.SendRpcRequestAsync<EstimateGasRequest, ulong>(
                    "eth_estimateGas", transaction, targetHeight, requestOptions, cancellationToken),
            };

            return response switch
            {
                RpcResult<ulong>.Success result => result.Result,
                RpcResult<ulong>.Error error => throw RPCException.FromRPCError(error),
                _ => throw new NotImplementedException(),
            };
        }
    }

    public async Task<AccessListResult> CreateAccessListAsync(
        Address? to, UInt256 value, ReadOnlyMemory<byte> data, StateAccess[]? accessList,
        CallOptions options,
        RpcRequestOptions requestOptions, CancellationToken cancellationToken)
    {
        var transaction = new EstimateGasRequest(options.From, to, value, data, accessList);
        var response = (options.TargetHeight == TargetHeight.Latest, options.StateOverrides, options.BlockOverrides) switch
        {
            (_, _, not null) => await _rpcClient.SendRpcRequestAsync<EstimateGasRequest, TargetHeight, IReadOnlyDictionary<Address, AccountOverride>?, BlockOverride, AccessListResult>(
                "eth_createAccessList", transaction, options.TargetHeight, options.StateOverrides, options.BlockOverrides, options.TargetHeight, requestOptions, cancellationToken),
            (_, not null, null) => await _rpcClient.SendRpcRequestAsync<EstimateGasRequest, TargetHeight, IReadOnlyDictionary<Address, AccountOverride>, AccessListResult>(
                "eth_createAccessList", transaction, options.TargetHeight, options.StateOverrides, options.TargetHeight, requestOptions, cancellationToken),
            (false, null, null) => await _rpcClient.SendRpcRequestAsync<EstimateGasRequest, TargetHeight, AccessListResult>(
                "eth_createAccessList", transaction, options.TargetHeight, options.TargetHeight, requestOptions, cancellationToken),
            _ => await _rpcClient.SendRpcRequestAsync<EstimateGasRequest, AccessListResult>(
                "eth_createAccessList", transaction, options.TargetHeight, requestOptions, cancellationToken),
        };

        return response switch
        {
            RpcResult<AccessListResult>.Success result => result.Result,
            RpcResult<AccessListResult>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<DetailedBlockData?> GetFullBlockByNumberAsync(
        TargetHeight targetHeight, RpcRequestOptions requestOptions, CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<TargetHeight, bool, DetailedBlockData>(
            "eth_getBlockByNumber", targetHeight, true, targetHeight, requestOptions, cancellationToken) switch
        {
            RpcResult<DetailedBlockData>.Success result => result.Result,
            RpcResult<DetailedBlockData>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<Block> GetBlockByNumberAsync(
        TargetHeight targetHeight, RpcRequestOptions requestOptions, CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<TargetHeight, bool, Block>(
            "eth_getBlockByNumber", targetHeight, false, targetHeight, requestOptions, cancellationToken) switch
        {
            RpcResult<Block>.Success result => result.Result,
            RpcResult<Block>.Error error => throw RPCException.FromRPCError(error),
            RpcResult<Block>.Null => throw new RPCException(-1, "block not found, rpc returned null", null),
            _ => throw new NotImplementedException(),
        };

    public Task<TxData?> TransactionByHashAsync(in Bytes32 hash, RpcRequestOptions requestOptions, CancellationToken cancellationToken)
    {
        var hashValue = hash;
        return TransactionByHashCoreAsync(hashValue, requestOptions, cancellationToken);
    }

    private async Task<TxData?> TransactionByHashCoreAsync(Bytes32 hash, RpcRequestOptions requestOptions, CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<Bytes32, TxData>(
            //ToDo: Add notion of unspecified required block height
            "eth_getTransactionByHash", hash, TargetHeight.Latest, requestOptions, cancellationToken) switch
        {
            RpcResult<TxData>.Success result => result.Result,
            RpcResult<TxData>.Error error => throw RPCException.FromRPCError(error),
            RpcResult<TxData>.Null => null,
            _ => throw new NotImplementedException(),
        };

    public Task<TxReceipt?> GetTransactionReceiptAsync(in Bytes32 transactionHash, RpcRequestOptions requestOptions, CancellationToken cancellationToken)
    {
        var transactionHashValue = transactionHash;
        return GetTransactionReceiptCoreAsync(transactionHashValue, requestOptions, cancellationToken);
    }

    private async Task<TxReceipt?> GetTransactionReceiptCoreAsync(Bytes32 transactionHash, RpcRequestOptions requestOptions, CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<Bytes32, TxReceipt>(
            "eth_getTransactionReceipt", transactionHash, TargetHeight.Latest, requestOptions, cancellationToken) switch
        {
            RpcResult<TxReceipt>.Success result => result.Result,
            RpcResult<TxReceipt>.Error error => throw RPCException.FromRPCError(error),
            RpcResult<TxReceipt>.Null => null,
            _ => throw new NotImplementedException(),
        };

    public Task<byte[]> GetStorageAtAsync(
        in Address address, byte[] slot, TargetHeight targetHeight = default,
        RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default)
    {
        var addressValue = address;
        return GetStorageAtCoreAsync(addressValue, slot, targetHeight, requestOptions, cancellationToken);
    }

    private async Task<byte[]> GetStorageAtCoreAsync(
        Address address, byte[] slot, TargetHeight targetHeight, RpcRequestOptions requestOptions,
        CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<Address, byte[], TargetHeight, byte[]>(
            "eth_getStorageAt", address, slot, targetHeight, targetHeight, requestOptions, cancellationToken) switch
        {
            RpcResult<byte[]>.Success result => result.Result,
            RpcResult<byte[]>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    // Keep RPC DTOs as record classes: readonly record structs produced identical JSON but no allocation reduction and mixed serialization throughput.
    private sealed record NewFilterRequest(
        TargetHeight FromBlock,
        TargetHeight ToBlock,
        ReadOnlyMemory<Address>? Address,
        EventTopics? Topics
    );
    public async Task<string> NewFilterAsync(
        TargetHeight fromBlock, TargetHeight toBlock, EventFilter eventFilter,
        RpcRequestOptions requestOptions, CancellationToken cancellationToken)
    {
        if(!_rpcTransport.SupportsFilters)
        {
            throw new InvalidOperationException("The underlying transport does not support filters");
        }

        var filterOptions = new NewFilterRequest(
            fromBlock,
            toBlock,
            eventFilter.Addresses.IsEmpty
                ? null
                : eventFilter.Addresses,
            eventFilter.Topics.IsMatchAll
                ? null
                : eventFilter.Topics
            );
        return await _rpcClient.SendRpcRequestAsync<NewFilterRequest, string>(
            "eth_newFilter", filterOptions, fromBlock, requestOptions, cancellationToken) switch
        {
            RpcResult<string>.Success result => result.Result,
            RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<string> NewBlockFilterAsync(RpcRequestOptions requestOptions, CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<string>(
            "eth_newBlockFilter", TargetHeight.Latest, requestOptions, cancellationToken) switch
        {
            RpcResult<string>.Success result => result.Result,
            RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<string> NewPendingTransactionFilterAsync(RpcRequestOptions requestOptions, CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<string>(
            "eth_newPendingTransactionFilter", TargetHeight.Latest, requestOptions, cancellationToken) switch
        {
            RpcResult<string>.Success result => result.Result,
            RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<bool> UninstallFilterAsync(string filterId, RpcRequestOptions requestOptions, CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<string, bool>(
            "eth_uninstallFilter", filterId, TargetHeight.Latest, requestOptions, cancellationToken) switch
        {
            RpcResult<bool>.Success result => result.Result,
            RpcResult<bool>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<List<string?>> GetPendingTransactionFilterChangesAsync(string filterId, RpcRequestOptions requestOptions, CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<string, List<string?>>(
            "eth_getFilterChanges", filterId, TargetHeight.Latest, requestOptions, cancellationToken) switch
        {
            RpcResult<List<string?>>.Success result => result.Result,
            RpcResult<List<string?>>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public async Task<Log[]> GetEventFilterChangesAsync(string filterId, RpcRequestOptions requestOptions, CancellationToken cancellationToken)
        => await _rpcClient.SendRpcRequestAsync<string, Log[]>(
            "eth_getFilterChanges", filterId, TargetHeight.Latest, requestOptions, cancellationToken) switch
        {
            RpcResult<Log[]>.Success result => result.Result,
            RpcResult<Log[]>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    // Keep RPC DTOs as record classes: readonly record structs produced identical JSON but no allocation reduction and mixed serialization throughput.
    private sealed record GetLogsRequest(
        TargetHeight FromBlock,
        TargetHeight ToBlock,
        ReadOnlyMemory<Address>? Address,
        EventTopics? Topics,
        Bytes32? BlockHash
    );
    public async Task<Log[]> GetLogsAsync(
        TargetHeight fromBlock, TargetHeight toBlock,
        EventFilter eventFilter, Bytes32? blockHash,
        RpcRequestOptions requestOptions, CancellationToken cancellationToken)
    {
        var filterOptions = new GetLogsRequest(
            fromBlock,
            toBlock,
            eventFilter.Addresses.IsEmpty
                ? null
                : eventFilter.Addresses,
            eventFilter.Topics.IsMatchAll
                ? null
                : eventFilter.Topics,
            blockHash);
        return await _rpcClient.SendRpcRequestAsync<GetLogsRequest, Log[]>(
            "eth_getLogs", filterOptions, fromBlock, requestOptions, cancellationToken) switch
        {
            RpcResult<Log[]>.Success result => result.Result,
            RpcResult<Log[]>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    // Keep RPC DTOs as record classes: readonly record structs produced identical JSON but no allocation reduction and mixed serialization throughput.
    private sealed record SubscribeLogsRequest(ReadOnlyMemory<Address>? Address, EventTopics? Topics);
    public async Task<string> SubscribeLogsAsync(EventFilter eventFilter, RpcRequestOptions requestOptions, CancellationToken cancellationToken)
    {
        if(!_rpcTransport.SupportsSubscriptions)
        {
            throw new InvalidOperationException("The underlying transport does not support subscriptions");
        }

        var request = new SubscribeLogsRequest(
            eventFilter.Addresses.IsEmpty
                ? null
                : eventFilter.Addresses,
            eventFilter.Topics.IsMatchAll
                ? null
                : eventFilter.Topics
            );
        return await _rpcClient.SendRpcRequestAsync<string, SubscribeLogsRequest, string>(
            "eth_subscribe", "logs", request, TargetHeight.Latest, requestOptions, cancellationToken) switch
        {
            RpcResult<string>.Success result => result.Result,
            RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<string> SubscribeNewHeadsAsync(RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default)
    {
        if(!_rpcTransport.SupportsSubscriptions)
        {
            throw new InvalidOperationException("The underlying transport does not support subscriptions");
        }
        //
        return await _rpcClient.SendRpcRequestAsync<string, string>(
                    "eth_subscribe", "newHeads", TargetHeight.Latest, requestOptions, cancellationToken) switch
        {
            RpcResult<string>.Success result => result.Result,
            RpcResult<string>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<bool> UnsubscribeAsync(string subscriptionId, RpcRequestOptions requestOptions, CancellationToken cancellationToken)
    {
        if(!_rpcTransport.SupportsSubscriptions)
        {
            throw new InvalidOperationException("The underlying transport does not support subscriptions");
        }
        //
        return await _rpcClient.SendRpcRequestAsync<string, bool>(
                "eth_unsubscribe", subscriptionId, TargetHeight.Latest, requestOptions, cancellationToken) switch
        {
            RpcResult<bool>.Success result => result.Result,
            RpcResult<bool>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }
}
