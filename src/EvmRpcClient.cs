﻿using EtherSharp.Types;
using System.Globalization;
using System.Numerics;

namespace EtherSharp;

public class EvmRpcClient(JsonRpcClient jsonRpcClient)
{
    private readonly JsonRpcClient _jsonRpcClient = jsonRpcClient;

    public async Task<long> EthBlockNumberAsync()
    {
        var response = await _jsonRpcClient.SendRpcRequest<string>("eth_blockNumber");

        return response switch
        {
            RpcResult<string>.Success result => long.Parse(result.Result.AsSpan()[2..], NumberStyles.HexNumber),
            RpcResult<string>.Error error => throw new Exception($"RPC Error: {error.Code} - {error.Message}"),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<BigInteger> EthGetBalance(string address, TargetBlockNumber blockNumber)
    {
        var response = await _jsonRpcClient.SendRpcRequest<string, string, BigInteger>("eth_getBalance", address, blockNumber.ToString());

        return response switch
        {
            RpcResult<BigInteger>.Success result => result.Result,
            RpcResult<BigInteger>.Error error => throw new Exception($"RPC Error: {error.Code} - {error.Message}"),
            _ => throw new NotImplementedException(),
        };
    }

    public Task<BigInteger> EthGetBalance(string address, long blockNumber)
        => EthGetBalance(address, TargetBlockNumber.Height(blockNumber));

    public async Task<string[]> EthAccountsAsync()
    {
        var response = await _jsonRpcClient.SendRpcRequest<string[]>("eth_accounts");
        return response switch
        {
            RpcResult<string[]>.Success result => result.Result,
            RpcResult<string[]>.Error error => throw new Exception($"RPC Error: {error.Code} - {error.Message}"),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<long> EthBlockTransactionCountByHashAsync(string blockHash)
    {
        if(blockHash.Length != 66)
        {
            throw new InvalidOperationException("Blockhash string must be 32 Bytes");
        }

        var response = await _jsonRpcClient.SendRpcRequest<string, string>("eth_getBlockTransactionCountByHash", blockHash);
        return response switch
        {
            RpcResult<string>.Success result => long.Parse(result.Result.AsSpan()[2..], NumberStyles.HexNumber),
            RpcResult<string>.Error error => throw new Exception($"RPC Error: {error.Code} - {error.Message}"),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<long> EthBlockTransactionCountByNumberAsync(TargetBlockNumber targetBlockNumber)
    {
        var response = await _jsonRpcClient.SendRpcRequest<string, string>("eth_getBlockTransactionCountByNumber", targetBlockNumber.ToString());

        return response switch
        {
            RpcResult<string>.Success result => long.Parse(result.Result.AsSpan()[2..], NumberStyles.HexNumber),
            RpcResult<string>.Error error => throw new Exception($"RPC Error: {error.Code} - {error.Message}"),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<long> EthBlockTransactionCountByNumberAsync(long blockNumber)
        => await EthBlockTransactionCountByNumberAsync(TargetBlockNumber.Height(blockNumber));

    private record TransactionEthCall(string? From, string To, uint? Gas, BigInteger? GasPrice, int? Value, string? Data);
    private record FakeAccountData(string? Balance, string? Nonce, string? Code, object? State, int? StateDiff);

    public record ContractReturn
    {
        public record Reverted : ContractReturn;
        public record Success(byte[] Data) : ContractReturn;
    }

    public async Task<ContractReturn> EthCallAsync(string? from, string to, uint? gas, BigInteger? gasPrice, int? value, string? contractMethode, TargetBlockNumber blockNumber, string? balance, string? nonce, string? code, object? state, int? stateDiff)
    {
        TransactionEthCall transaction = new(from, to, gas, gasPrice, value, contractMethode);

        FakeAccountData fakeAccountData = new(balance, nonce, code, state, stateDiff);

        var response = await _jsonRpcClient.SendRpcRequest<TransactionEthCall, string, FakeAccountData, byte[]>("eth_call", transaction, blockNumber.ToString(), fakeAccountData);

        switch(response)
        {
            case RpcResult<byte[]>.Success result:
                return new ContractReturn.Success(result.Result);
            case RpcResult<byte[]>.Error error:
                if(error.Message == "execution reverted" && error.Code == -32000)
                {
                    return new ContractReturn.Reverted();
                }
                throw new Exception($"RPC Error: {error.Code} - {error.Message}");
            default:
                throw new NotImplementedException();
        }
    }

    private record TransactionEstimateGas(string? From, string? To, uint? Gas, BigInteger? GasPrice, int? Value, string? Input);
    public async Task<uint> EthEstimateGasAsync(string? from, string? to, uint? gas, BigInteger? gasPrice, int? value, string? input, TargetBlockNumber? blockNumber)
    {
        TransactionEstimateGas transaction = new(from, to, gas, gasPrice, value, input);
        var response = await _jsonRpcClient.SendRpcRequest<TransactionEstimateGas, string?, uint>("eth_estimateGas", transaction, blockNumber?.ToString());

        return response switch
        {
            RpcResult<uint>.Success result => result.Result,
            RpcResult<uint>.Error error => throw new Exception($"RPC Error: {error.Code} - {error.Message}"),
            _ => throw new NotImplementedException(),
        };
    }

    public Task<uint> EthEstimateGasAsync(string? from, string? to, uint? gas, BigInteger? gasPrice, int? value, string? input, long blockNumber)
        => EthEstimateGasAsync(from, to, gas, gasPrice, value, input, TargetBlockNumber.Height(blockNumber));

    public async Task<BlockData?> EthGetFullBlockByHashAsync(string blockHash)
    {
        if(blockHash.Length != 66)
        {
            throw new InvalidOperationException("Blockhash string must be 32 Bytes");
        }
        var response = await _jsonRpcClient.SendRpcRequest<string, bool, BlockData?>("eth_getBlockByHash", blockHash, true);

        return response switch
        {
            RpcResult<BlockData>.Success result => result.Result,
            RpcResult<BlockData>.Error error => throw new Exception($"RPC Error: {error.Code} - {error.Message}"),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<BlockDataTrasactionAsString?> EthGetBlockTransactionsByHashAsync(string blockHash)
    {
        if(blockHash.Length != 66)
        {
            throw new InvalidOperationException("Blockhash string must be 32 Bytes");
        }
        var response = await _jsonRpcClient.SendRpcRequest<string, bool, BlockDataTrasactionAsString>("eth_getBlockByHash", blockHash, false);
        return response switch
        {
            RpcResult<BlockDataTrasactionAsString>.Success result => result.Result,
            RpcResult<BlockDataTrasactionAsString>.Error error => throw new Exception($"RPC Error: {error.Code} - {error.Message}"),
            _ => throw new NotImplementedException(),
        };
    }

    public Task<BlockData?> EthGetFullBlockByNumberAsync(long targetBlockNumber)
    => EthGetFullBlockByNumberAsync(TargetBlockNumber.Height(targetBlockNumber));

    public async Task<BlockData?> EthGetFullBlockByNumberAsync(TargetBlockNumber targetBlockNumber)
    {

        var response = await _jsonRpcClient.SendRpcRequest<string, bool, BlockData>("eth_getBlockByNumber", targetBlockNumber.ToString(), true);
        return response switch
        {
            RpcResult<BlockData>.Success result => result.Result,
            RpcResult<BlockData>.Error error => throw new Exception($"RPC Error: {error.Code} - {error.Message}"),
            _ => throw new NotImplementedException(),
        };
    }

    public Task<BlockDataTrasactionAsString?> EthGetBlockByNumberAsync(long targetBlockNumber)
        => EthGetBlockByNumberAsync(TargetBlockNumber.Height(targetBlockNumber));

    public async Task<BlockDataTrasactionAsString?> EthGetBlockByNumberAsync(TargetBlockNumber targetBlockNumber)
    {

        var response = await _jsonRpcClient.SendRpcRequest<string, bool, BlockDataTrasactionAsString>("eth_getBlockByNumber", targetBlockNumber.ToString(), false);
        return response switch
        {
            RpcResult<BlockDataTrasactionAsString>.Success result => result.Result,
            RpcResult<BlockDataTrasactionAsString>.Error error => throw new Exception($"RPC Error: {error.Code} - {error.Message}"),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<Transaction> EthTransactionByHash(string hash)
    {
        var response = await _jsonRpcClient.SendRpcRequest<string, Transaction>("eth_getTransactionByHash", hash);
        return response switch
        {
            RpcResult<Transaction>.Success result => result.Result,
            RpcResult<Transaction>.Error error => throw new Exception($"RPC Error: {error.Code} - {error.Message}"),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<Transaction> EthGetTransactionByBlockHashAndIndexAsync(string blockHash, int index)
    {
        if(blockHash.Length != 66)
        {
            throw new InvalidOperationException("Blockhash string must be 32 Bytes");
        }
        var response = await _jsonRpcClient.SendRpcRequest<string, int, Transaction>("eth_getTransactionByBlockHashAndIndex", blockHash, index);
        return response switch
        {
            RpcResult<Transaction>.Success result => result.Result,
            RpcResult<Transaction>.Error error => throw new Exception($"RPC Error: {error.Code} - {error.Message}"),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<Transaction> EthGetTransactionByBlockNumberAndIndexAsync(string blockNumber, int index)
    {
        var response = await _jsonRpcClient.SendRpcRequest<string, int, Transaction>("eth_getTransactionByBlockNumberAndIndex", blockNumber, index);
        return response switch
        {
            RpcResult<Transaction>.Success result => result.Result,
            RpcResult<Transaction>.Error error => throw new Exception($"RPC Error: {error.Code} - {error.Message}"),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<TransactionReceipt> EthGetTransactionReceiptAsync(string transactionHash)
    {
        var response = await _jsonRpcClient.SendRpcRequest<string, TransactionReceipt>("eth_getTransactionReceipt", transactionHash);
        return response switch
        {
            RpcResult<TransactionReceipt>.Success result => result.Result,
            RpcResult<TransactionReceipt>.Error error => throw new Exception($"RPC Error: {error.Code} - {error.Message}"),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<Uncle?> EthGetUncleByBlockHashAndIndexAsync(string blockHash, int uncleIndex)
    {
        var response = await _jsonRpcClient.SendRpcRequest<string, int, Uncle>("eth_getUncleByBlockHashAndIndex", blockHash, uncleIndex);
        return response switch
        {
            RpcResult<Uncle>.Success result => result.Result,
            RpcResult<Uncle>.Error error => throw new Exception($"RPC Error: {error.Code} - {error.Message}"),
            RpcResult<Uncle>.Null _ => null,
            _ => throw new NotImplementedException(),
        };
    }

    public Task<Uncle?> EthGetUncleByBlockNumberAndIndexAsync(BigInteger height, uint uncleIndex)
     => EthGetUncleByBlockNumberAndIndexAsync(TargetBlockNumber.Height(height), uncleIndex);

    public async Task<Uncle?> EthGetUncleByBlockNumberAndIndexAsync(TargetBlockNumber targetBlockNumber, uint uncleIndex)
    {
        var response = await _jsonRpcClient.SendRpcRequest<string, uint, Uncle>("eth_getUncleByBlockNumberAndIndex", targetBlockNumber.ToString(), uncleIndex);
        return response switch
        {
            RpcResult<Uncle>.Success result => result.Result,
            RpcResult<Uncle>.Error error => throw new Exception($"RPC Error: {error.Code} - {error.Message}"),
            RpcResult<Uncle>.Null _ => null,
            _ => throw new NotImplementedException(),
        };
    }

    private record NewFilterOption(string? FromBlock, string? ToBlock, string? Address, string[]? Topics);

    public async Task<string> EthNewFilterAsync(string? fromBlock, string? toBlock, string? address, string[]? topics)
    {

        NewFilterOption filterOptions = new(fromBlock, toBlock, address, topics);

        var response = await _jsonRpcClient.SendRpcRequest<NewFilterOption, string>("eth_newFilter", filterOptions);
        return response switch
        {
            RpcResult<string>.Success result => result.Result,
            RpcResult<string>.Error error => throw new Exception($"RPC Error: {error.Code} - {error.Message}"),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<string> EthNewBlockFilterAsync()
    {
        var response = await _jsonRpcClient.SendRpcRequest<string>("eth_newBlockFilter");
        return response switch
        {
            RpcResult<string>.Success result => result.Result,
            RpcResult<string>.Error error => throw new Exception($"RPC Error: {error.Code} - {error.Message}"),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<string> EthNewPendingTransactionFilterAsync()
    {
        var response = await _jsonRpcClient.SendRpcRequest<string>("eth_newPendingTransactionFilter");
        return response switch
        {
            RpcResult<string>.Success result => result.Result,
            RpcResult<string>.Error error => throw new Exception($"RPC Error: {error.Code} - {error.Message}"),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<bool> EthUninstallFilterAsync(string filterIDString)
    {
        var response = await _jsonRpcClient.SendRpcRequest<string, bool>("eth_uninstallFilter", filterIDString);
        return response switch
        {
            RpcResult<bool>.Success result => result.Result,
            RpcResult<bool>.Error error => throw new Exception($"RPC Error: {error.Code} - {error.Message}"),
            _ => throw new NotImplementedException(),
        };
    }
    public Task<List<string?>> GetBlockFilterChangesAsync(string filterID)
        => EthGetPendingTransactionFilterChangesAsync(filterID);
    public async Task<List<string?>> EthGetPendingTransactionFilterChangesAsync(string filterID)
    {
        var response = await _jsonRpcClient.SendRpcRequest<string, List<string?>>("eth_getFilterChanges", filterID);
        return response switch
        {
            RpcResult<List<string?>>.Success result => result.Result,
            RpcResult<List<string?>>.Error error => throw new Exception($"RPC Error: {error.Code} - {error.Message}"),
            _ => throw new NotImplementedException(),
        };
    }
    public async Task<EventFilterChangesResult[]> EthGetEventFilterChangesAsync(string filterID)
    {
        var response = await _jsonRpcClient.SendRpcRequest<string, EventFilterChangesResult[]>("eth_getFilterChanges", filterID);
        return response switch
        {
            RpcResult<EventFilterChangesResult[]>.Success result => result.Result,
            RpcResult<EventFilterChangesResult[]>.Error error => throw new Exception($"RPC Error: {error.Code} - {error.Message}"),
            _ => throw new NotImplementedException(),
        };
    }

    private record FilterOptions(TargetBlockNumber? FromBlock, TargetBlockNumber? ToBlock, string Address, Topics[] Topics, byte[]? BlockHash);

    public async Task<Log[]> EthGetLogsAsync(TargetBlockNumber? fromBlock, TargetBlockNumber? toBlock, string address, Topics[] topics, byte[]? blockHash)
    {
        FilterOptions filterOptions = new(fromBlock, toBlock, address, topics, blockHash);
        var response = await _jsonRpcClient.SendRpcRequest<FilterOptions, Log[]>("eth_getLogs", filterOptions);
        return response switch
        {
            RpcResult<Log[]>.Success result => result.Result,
            RpcResult<Log[]>.Error error => throw new Exception($"RPC Error: {error.Code} - {error.Message}"),
            _ => throw new NotImplementedException(),
        };
    }
}