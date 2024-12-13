using EtherSharp.Client.Services.RPC;
using EtherSharp.Tx;
using EtherSharp.Tx.EIP1559;
using EtherSharp.Tx.Types;
using EtherSharp.Wallet;
using System.Numerics;

namespace EtherSharp.Client.Services.GasFeeProvider;
public class CachingRpcGasFeeProvider(IRpcClient rpcClient, IEtherSigner signer) : IGasFeeProvider, IInitializableService
{
    private readonly IRpcClient _rpcClient = rpcClient;
    private readonly IEtherSigner _signer = signer;

    private BigInteger _lastGasPrice;
    private BigInteger _lastPriorityFee;

    public TimeSpan RefreshInterval { get; set; } = TimeSpan.FromSeconds(30);

    public async ValueTask InitializeAsync(ulong chainId, CancellationToken cancellationToken)
    {
        var gasPriceTask = _rpcClient.EthGasPriceAsync(cancellationToken);
        var priorityFeeTask = _rpcClient.EthMaxPriorityFeePerGas(cancellationToken);

        _lastGasPrice = await gasPriceTask;
        _lastPriorityFee = await priorityFeeTask;

        _ = RefreshWorker();
    }

    private async Task RefreshWorker()
    {
        using var timer = new PeriodicTimer(RefreshInterval);

        while(await timer.WaitForNextTickAsync())
        {
            try
            {
                var gasPriceTask = _rpcClient.EthGasPriceAsync();
                var priorityFeeTask = _rpcClient.EthMaxPriorityFeePerGas();

                _lastGasPrice = await gasPriceTask;
                _lastPriorityFee = await priorityFeeTask;
            }
            catch
            {
            }
        }
    }

    public Task<ulong> EstimateGasAsync(
        ITxInput txInput, ReadOnlySpan<byte> data, CancellationToken cancellationToken = default)
        => _rpcClient.EthEstimateGasAsync(
                _signer.Address.String,
                txInput.To.String,
                txInput.Value,
                $"0x{Convert.ToHexString(data)}",
                cancellationToken
        );

    public Task<ITxGasParams> CalculateGasParamsAsync<TTxParams>(
        ITxInput txInput, TTxParams txParams, ulong gas, CancellationToken cancellationToken = default)
        where TTxParams : ITxParams 
        => txParams switch
        {
            EIP1559TxParams => Task.FromResult<ITxGasParams>(new EIP1559GasParams(_lastGasPrice, _lastPriorityFee)),
            _ => throw new NotSupportedException(),
        };
}
