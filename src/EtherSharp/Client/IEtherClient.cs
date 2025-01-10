using EtherSharp.Client.Services.EtherApi;
using EtherSharp.Client.Services.LogsApi;
using EtherSharp.Client.Services.RPC;
using EtherSharp.Contract;
using EtherSharp.Events;
using EtherSharp.Tx;
using EtherSharp.Tx.EIP1559;
using EtherSharp.Tx.Types;
using EtherSharp.Types;
using System.Numerics;

namespace EtherSharp.Client;
public interface IEtherClient
{
    public ulong ChainId { get; }
    public IEtherApi ETH { get; }

    public ILogsApi<TEvent> Logs<TEvent>() where TEvent : ITxEvent<TEvent>;
    public ILogsApi<Log> Logs() => Logs<Log>();

    public Task InitializeAsync(CancellationToken cancellationToken = default);

    public Task<BlockDataTrasactionAsString> EthGetBlockByNumberAsync(TargetBlockNumber targetBlockNumber, CancellationToken cancellationToken = default);
    public Task<ulong> GetPeakHeightAsync(CancellationToken cancellationToken = default);
    public Task<uint> GetTransactionCount(string address, TargetBlockNumber targetHeight = default, CancellationToken cancellationToken = default);

    public TContract Contract<TContract>(string address)
        where TContract : IEVMContract;
    public TContract Contract<TContract>(Address address)
        where TContract : IEVMContract;

    public Task<T> CallAsync<T>(TxInput<T> call, TargetBlockNumber targetHeight = default, CancellationToken cancellationToken = default);

    public Task<FeeHistory> GetFeeHistoryAsync(int blockCount, TargetBlockNumber newestBlock,
        double[] rewardPercentiles, CancellationToken cancellationToken = default);
    public Task<BigInteger> GetGasPriceAsync(CancellationToken cancellationToken = default);
    public Task<EIP1559GasParams> EstimateTxGasParamsAsync(ITxInput call, EIP1559TxParams? txParams = default, CancellationToken cancellationToken = default)
        => EstimateTxGasParamsAsync<EIP1559TxParams, EIP1559GasParams>(call, txParams, cancellationToken);
    public Task<TTxGasParams> EstimateTxGasParamsAsync<TTxParams, TTxGasParams>(
        ITxInput call, TTxParams? txParams = default, CancellationToken cancellationToken = default
    )
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams;
}
