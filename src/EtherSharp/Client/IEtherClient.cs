using EtherSharp.Client.Modules.Blocks;
using EtherSharp.Client.Modules.Debug;
using EtherSharp.Client.Modules.Ether;
using EtherSharp.Client.Modules.Events;
using EtherSharp.Client.Modules.Trace;
using EtherSharp.Contract;
using EtherSharp.Numerics;
using EtherSharp.Query;
using EtherSharp.Realtime.Events;
using EtherSharp.Tx;
using EtherSharp.Tx.EIP1559;
using EtherSharp.Tx.Types;
using EtherSharp.Types;

namespace EtherSharp.Client;

/// <summary>
/// High-level Ethereum RPC client abstraction for reads, queries, contracts, and execution simulation.
/// </summary>
public interface IEtherClient
{
    internal bool IsInitialized { get; }

    /// <summary>
    /// Gets the chain ID of the connected network.
    /// </summary>
    public ulong ChainId { get; }

    /// <summary>
    /// Gets opcode compatibility details for the connected chain, when available.
    /// </summary>
    public CompatibilityReport? CompatibilityReport { get; }

    /// <summary>
    /// Gets the module for native-currency and account-level RPC operations.
    /// </summary>
    public IEtherModule ETH { get; }

    /// <summary>
    /// Gets the module for block-related RPC operations.
    /// </summary>
    public IBlocksModule Blocks { get; }

    /// <summary>
    /// Gets the module for debug RPC operations.
    /// </summary>
    public IDebugModule Debug { get; }

    /// <summary>
    /// Gets the module for trace RPC operations.
    /// </summary>
    public ITraceModule Trace { get; }

    /// <summary>
    /// Executes a single query at the specified target block.
    /// </summary>
    /// <typeparam name="T1">Result type of the query.</typeparam>
    /// <param name="c1">Query to execute.</param>
    /// <param name="targetHeight">Target block context for query execution.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The query result.</returns>
    public Task<T1> QueryAsync<T1>(
        IQuery<T1> c1,
        TargetHeight targetHeight = default, CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Executes two queries in a single coordinated query context.
    /// </summary>
    /// <typeparam name="T1">Result type of the first query.</typeparam>
    /// <typeparam name="T2">Result type of the second query.</typeparam>
    /// <param name="c1">First query.</param>
    /// <param name="c2">Second query.</param>
    /// <param name="targetHeight">Target block context for query execution.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A tuple containing both query results.</returns>
    public Task<(T1, T2)> QueryAsync<T1, T2>(
        IQuery<T1> c1, IQuery<T2> c2,
        TargetHeight targetHeight = default, CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Executes three queries in a single coordinated query context.
    /// </summary>
    public Task<(T1, T2, T3)> QueryAsync<T1, T2, T3>(
        IQuery<T1> c1, IQuery<T2> c2, IQuery<T3> c3,
        TargetHeight targetHeight = default, CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Executes four queries in a single coordinated query context.
    /// </summary>
    public Task<(T1, T2, T3, T4)> QueryAsync<T1, T2, T3, T4>(
        IQuery<T1> c1, IQuery<T2> c2, IQuery<T3> c3, IQuery<T4> c4,
        TargetHeight targetHeight = default, CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Executes five queries in a single coordinated query context.
    /// </summary>
    public Task<(T1, T2, T3, T4, T5)> QueryAsync<T1, T2, T3, T4, T5>(
        IQuery<T1> c1, IQuery<T2> c2, IQuery<T3> c3, IQuery<T4> c4, IQuery<T5> c5,
        TargetHeight targetHeight = default, CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets a typed event-stream module for decoding contract logs into <typeparamref name="TEvent"/>.
    /// </summary>
    /// <typeparam name="TEvent">Event DTO implementing <see cref="ITxLog{TEvent}"/>.</typeparam>
    /// <returns>An events module specialized for <typeparamref name="TEvent"/>.</returns>
    public IEventsModule<TEvent> Events<TEvent>() where TEvent : ITxLog<TEvent>;

    /// <summary>
    /// Gets an untyped event-stream module for raw <see cref="Log"/> entries.
    /// </summary>
    /// <returns>An events module for raw logs.</returns>
    public IEventsModule<Log> Events() => Events<Log>();

    /// <summary>
    /// Exposes internal client capabilities used by advanced infrastructure components.
    /// </summary>
    /// <returns>The internal client view.</returns>
    public IInternalEtherClient AsInternal();

    /// <summary>
    /// Initializes the client and loads chain capabilities.
    /// </summary>
    /// <param name="forceNoQuery">If <see langword="true"/>, skips initial query-based warmup when supported.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task that completes when initialization finishes.</returns>
    public Task InitializeAsync(bool forceNoQuery = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Initializes the client and executes an initialization query.
    /// </summary>
    /// <typeparam name="T">Result type returned by the initialization query.</typeparam>
    /// <param name="initQuery">Query to execute as part of initialization.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The query result.</returns>
    public Task<T> InitializeAsync<T>(IQuery<T> initQuery, CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches a transaction by hash.
    /// </summary>
    /// <param name="hash">Transaction hash.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The transaction if found; otherwise <see langword="null"/>.</returns>
    public Task<TxData?> GetTransactionAsync(Hash32 hash, CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches a transaction receipt by hash.
    /// </summary>
    /// <param name="hash">Transaction hash.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The receipt if available; otherwise <see langword="null"/>.</returns>
    public Task<TxReceipt?> GetTransactionReceiptAsync(Hash32 hash, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the account nonce (transaction count) for an address at a specific block context.
    /// </summary>
    /// <param name="address">Account address.</param>
    /// <param name="targetHeight">Target block context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The account nonce.</returns>
    public Task<uint> GetTransactionCount(Address address, TargetHeight targetHeight = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reads a storage slot from contract storage.
    /// </summary>
    /// <param name="address">Contract address.</param>
    /// <param name="slot">32-byte storage slot key.</param>
    /// <param name="targetHeight">Target block context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The slot value as bytes.</returns>
    public Task<byte[]> GetStorageAtAsync(Address address, byte[] slot, TargetHeight targetHeight = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reads a storage slot from a contract instance.
    /// </summary>
    /// <param name="contract">Contract instance.</param>
    /// <param name="slot">32-byte storage slot key.</param>
    /// <param name="targetHeight">Target block context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The slot value as bytes.</returns>
    public Task<byte[]> GetStorageAtAsync(IEVMContract contract, byte[] slot, TargetHeight targetHeight = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Binds a contract interface to a specific on-chain address.
    /// </summary>
    /// <typeparam name="TContract">Contract interface/type implementing <see cref="IEVMContract"/>.</typeparam>
    /// <param name="address">Contract address.</param>
    /// <returns>A contract proxy bound to <paramref name="address"/>.</returns>
    public TContract Contract<TContract>(Address address)
        where TContract : IEVMContract;

    /// <summary>
    /// Executes a safe call that captures success/failure status and return data.
    /// </summary>
    /// <typeparam name="T">Expected decoded return type.</typeparam>
    /// <param name="call">Call input definition.</param>
    /// <param name="targetHeight">Target block context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A structured call result including revert information when applicable.</returns>
    public Task<TxCallResult> SafeCallAsync<T>(ITxInput<T> call, TargetHeight targetHeight = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a read-only call and returns the decoded result.
    /// </summary>
    /// <typeparam name="T">Expected decoded return type.</typeparam>
    /// <param name="call">Call input definition.</param>
    /// <param name="targetHeight">Target block context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The decoded call result.</returns>
    public Task<T> CallAsync<T>(ITxInput<T> call, TargetHeight targetHeight = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a safe flash-call against a temporary deployment and captures success/failure details.
    /// </summary>
    /// <typeparam name="T">Expected decoded return type.</typeparam>
    /// <param name="deployment">Deployment definition used for the temporary execution context.</param>
    /// <param name="call">Call to execute against the temporary deployment.</param>
    /// <param name="targetHeight">Target block context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A structured call result including revert information when applicable.</returns>
    public Task<TxCallResult> SafeFlashCallAsync<T>(
        IContractDeployment deployment, IContractCall<T> call, TargetHeight targetHeight = default, CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Executes a flash-call against a temporary deployment and returns the decoded result.
    /// </summary>
    /// <typeparam name="T">Expected decoded return type.</typeparam>
    /// <param name="deployment">Deployment definition used for the temporary execution context.</param>
    /// <param name="call">Call to execute against the temporary deployment.</param>
    /// <param name="targetHeight">Target block context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The decoded call result.</returns>
    public Task<T> FlashCallAsync<T>(
        IContractDeployment deployment, IContractCall<T> call, TargetHeight targetHeight = default, CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves fee history data used for gas estimation strategies.
    /// </summary>
    /// <param name="blockCount">Number of historical blocks to inspect.</param>
    /// <param name="newestBlock">Newest block included in the history window.</param>
    /// <param name="rewardPercentiles">Reward percentiles to calculate per block.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Fee history snapshot from the node.</returns>
    public Task<FeeHistory> GetFeeHistoryAsync(int blockCount, TargetHeight newestBlock,
        double[] rewardPercentiles, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current network gas price.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The current gas price.</returns>
    public Task<UInt256> GetGasPriceAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current recommended max priority fee per gas.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The suggested priority fee per gas.</returns>
    public Task<UInt256> GetMaxPriorityFeePerGasAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Estimates the gas limit required to execute a transaction call.
    /// </summary>
    /// <param name="call">Transaction call definition.</param>
    /// <param name="from">Optional sender address override.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Estimated gas limit.</returns>
    public Task<ulong> EstimateGasLimitAsync(ITxInput call, Address? from = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Estimates EIP-1559 gas parameters for a transaction call.
    /// </summary>
    /// <param name="call">Transaction call definition.</param>
    /// <param name="txParams">Optional transaction parameters that influence estimation.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Estimated EIP-1559 gas parameters.</returns>
    public Task<EIP1559GasParams> EstimateTxGasParamsAsync(ITxInput call, EIP1559TxParams? txParams = default, CancellationToken cancellationToken = default)
        => EstimateTxGasParamsAsync<EIP1559TxParams, EIP1559GasParams>(call, txParams, cancellationToken);

    /// <summary>
    /// Estimates gas parameters for a transaction call using custom transaction and gas parameter types.
    /// </summary>
    /// <typeparam name="TTxParams">Transaction parameter type.</typeparam>
    /// <typeparam name="TTxGasParams">Gas parameter result type.</typeparam>
    /// <param name="call">Transaction call definition.</param>
    /// <param name="txParams">Optional transaction parameters that influence estimation.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Estimated gas parameters.</returns>
    public Task<TTxGasParams> EstimateTxGasParamsAsync<TTxParams, TTxGasParams>(
        ITxInput call, TTxParams? txParams = default, CancellationToken cancellationToken = default
    )
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams;
}
