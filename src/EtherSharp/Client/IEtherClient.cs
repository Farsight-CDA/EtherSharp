using EtherSharp.Client.Modules.Blocks;
using EtherSharp.Client.Modules.Ether;
using EtherSharp.Client.Modules.Events;
using EtherSharp.Client.Modules.Query;
using EtherSharp.Contract;
using EtherSharp.Realtime.Events;
using EtherSharp.Tx;
using EtherSharp.Tx.EIP1559;
using EtherSharp.Tx.Types;
using EtherSharp.Types;
using System.Numerics;

namespace EtherSharp.Client;

public interface IEtherClient
{
    /// <summary>
    /// The chainId of the chain you are connected to.
    /// </summary>
    public ulong ChainId { get; }
    /// <summary>
    /// Module used to interact with the native currency.
    /// </summary>
    public IEtherModule ETH { get; }
    /// <summary>
    /// Module used to interact with blocks.
    /// </summary>
    public IBlocksModule Blocks { get; }

    public Task<T1> QueryAsync<T1>(
        IQuery<T1> c1,
        TargetBlockNumber targetBlockNumber = default, CancellationToken cancellationToken = default
    );
    public Task<(T1, T2)> QueryAsync<T1, T2>(
        IQuery<T1> c1, IQuery<T2> c2,
        TargetBlockNumber targetBlockNumber = default, CancellationToken cancellationToken = default
    );
    public Task<(T1, T2, T3)> QueryAsync<T1, T2, T3>(
        IQuery<T1> c1, IQuery<T2> c2, IQuery<T3> c3,
        TargetBlockNumber targetBlockNumber = default, CancellationToken cancellationToken = default
    );
    public Task<(T1, T2, T3, T4)> QueryAsync<T1, T2, T3, T4>(
        IQuery<T1> c1, IQuery<T2> c2, IQuery<T3> c3, IQuery<T4> c4,
        TargetBlockNumber targetBlockNumber = default, CancellationToken cancellationToken = default
    );
    public Task<(T1, T2, T3, T4, T5)> QueryAsync<T1, T2, T3, T4, T5>(
        IQuery<T1> c1, IQuery<T2> c2, IQuery<T3> c3, IQuery<T4> c4, IQuery<T5> c5,
        TargetBlockNumber targetBlockNumber = default, CancellationToken cancellationToken = default
    );

    public IEventsModule<TEvent> Events<TEvent>() where TEvent : ITxLog<TEvent>;
    public IEventsModule<Log> Events() => Events<Log>();

    public IInternalEtherClient AsInternal();

    public Task InitializeAsync(CancellationToken cancellationToken = default);
    public Task<T> InitializeAsync<T>(IQuery<T> initQuery, CancellationToken cancellationToken = default);

    public Task<Transaction?> GetTransactionAsync(string hash, CancellationToken cancellationToken = default);
    public Task<TransactionReceipt?> GetTransactionReceiptAsync(string hash, CancellationToken cancellationToken = default);

    public Task<uint> GetTransactionCount(Address address, TargetBlockNumber targetHeight = default, CancellationToken cancellationToken = default);

    public Task<byte[]> GetStorageAtAsync(Address address, byte[] slot, TargetBlockNumber targetBlockNumber = default, CancellationToken cancellationToken = default);
    public Task<byte[]> GetStorageAtAsync(IEVMContract contract, byte[] slot, TargetBlockNumber targetBlockNumber = default, CancellationToken cancellationToken = default);

    public TContract Contract<TContract>(Address address)
        where TContract : IEVMContract;

    public Task<T> CallAsync<T>(ITxInput<T> call, TargetBlockNumber targetHeight = default, CancellationToken cancellationToken = default);
    public Task<T> FlashCallAsync<T>(IContractDeployment deployment, IContractCall<T> call, TargetBlockNumber targetHeight = default, CancellationToken cancellationToken = default);

    public Task<FeeHistory> GetFeeHistoryAsync(int blockCount, TargetBlockNumber newestBlock,
        double[] rewardPercentiles, CancellationToken cancellationToken = default);

    public Task<BigInteger> GetGasPriceAsync(CancellationToken cancellationToken = default);
    public Task<BigInteger> GetMaxPriorityFeePerGasAsync(CancellationToken cancellationToken = default);

    public Task<ulong> EstimateGasLimitAsync(ITxInput call, Address? from = null, CancellationToken cancellationToken = default);

    public Task<EIP1559GasParams> EstimateTxGasParamsAsync(ITxInput call, EIP1559TxParams? txParams = default, CancellationToken cancellationToken = default)
        => EstimateTxGasParamsAsync<EIP1559TxParams, EIP1559GasParams>(call, txParams, cancellationToken);
    public Task<TTxGasParams> EstimateTxGasParamsAsync<TTxParams, TTxGasParams>(
        ITxInput call, TTxParams? txParams = default, CancellationToken cancellationToken = default
    )
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams;
}
