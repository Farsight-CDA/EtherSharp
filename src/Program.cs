using EVM.net.types;
using System.Numerics;
public class Program
{
    public static async Task Main(string[] args)
    {
        string rpcUrl = "https://eth.drpc.org";
        JsonRpcClient jsonRpcClient = new(rpcUrl, new HttpClient());

        var evmRpcClient = new EvmRpcClient(jsonRpcClient);

        _ = new AbiEncoder();

        await TestAsync(evmRpcClient);
    }

    public static async Task TestAsync(EvmRpcClient evmRpcClient)
    {
        long blockNumber = await evmRpcClient.EthBlockNumberAsync();

        string adress = "0x95222290DD7278Aa3Ddd389Cc1E1d165CC4BAfe5";

        Console.WriteLine($"BlockNumber: {blockNumber}");
        Console.WriteLine($"BlockNumber: {adress}");

        //_ = await evmRpcClient.EthGetBalance(address, blockNumber);

        //_ = await evmRpcClient.EthAccountsAsync();

        //_ = await evmRpcClient.EthBlockTransactionCountByHashAsync(blockHash);
        //_ = await evmRpcClient.EthBlockTransactionCountByNumberAsync(blockNumber);

        _ = new BigInteger(20);
        var targetBlockNumber = TargetBlockNumber.Height(blockNumber);

        //_ = await evmRpcClient.EthCallAsync(null, to, null, null, null, null, targetBlockNumber, null, null, null, null, null);

        //_ = await evmRpcClient.EthEstimateGasAsync(from, to, gas, gasPrice, value, input, targetBlockNumber);

        //_ = await evmRpcClient.EthGetFullBlockByHashAsync(blockHash);

        //var transactions = await evmRpcClient.EthGetBlockTransactionsByHashAsync(blockHash);

        //var fullBlock2 = await evmRpcClient.EthGetFullBlockByNumberAsync(targetBlockNumber);

        //var transactions2 = await evmRpcClient.EthGetBlockByNumberAsync(targetBlockNumber);

        //var transaction = await evmRpcClient.EthTransactionByHash(transactionHash);

        //_ = new BigInteger(20);
        _ = targetBlockNumber.ToString();
        //var transaction2 = await evmRpcClient.EthGetTransactionByBlockHashAndIndexAsync(blockHash, 0);

        //var transaction3 = await evmRpcClient.EthGetTransactionByBlockNumberAndIndexAsync(targetBlockNumber.ToString(), 0);

        //var transactionReceipt = await evmRpcClient.EthGetTransactionReceiptAsync(transactionHash);

        //var uncle = await evmRpcClient.EthGetUncleByBlockHashAndIndexAsync(blockHash, 0);

        //var uncle2 = await evmRpcClient.EthGetUncleByBlockNumberAndIndexAsync(targetBlockNumber, 0);

        //string newFilterId = await evmRpcClient.EthNewFilterAsync(targetBlockNumber.ToString(), targetBlockNumber.ToString(), null, null);
        //_ = await evmRpcClient.EthGetEventFilterChangesAsync(newFilterId);

        //string newBlockFilterId = await evmRpcClient.EthNewBlockFilterAsync();
        //_ = await evmRpcClient.GetBlockFilterChangesAsync(newBlockFilterId);

        //string newPendingTransactionFilterId = await evmRpcClient.EthNewPendingTransactionFilterAsync();
        //_ = await evmRpcClient.EthGetPendingTransactionFilterChangesAsync(newPendingTransactionFilterId);

        //_ = await evmRpcClient.EthUninstallFilterAsync(newFilterId);

        //_ = await evmRpcClient.EthNewBlockFilterAsync();

        //_ = await evmRpcClient.EthNewPendingTransactionFilterAsync();

        //_ = await evmRpcClient.EthGetLogsAsync(null, null, adress, [], null);
    }
}

