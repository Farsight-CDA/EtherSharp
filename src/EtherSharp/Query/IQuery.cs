using EtherSharp.Contract;
using EtherSharp.Numerics;
using EtherSharp.Query.Operations;
using EtherSharp.Tx;
using EtherSharp.Types;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace EtherSharp.Query;

/// <summary>
/// Represents a low-level query operation that can be encoded into the aggregated query payload.
/// </summary>
public partial interface IQuery
{
    /// <summary>
    /// Gets the number of bytes this operation contributes to the encoded query calldata.
    /// </summary>
    public int CallDataLength { get; }

    /// <summary>
    /// Gets the ETH value that should be sent when executing this operation.
    /// </summary>
    public UInt256 EthValue { get; }

    /// <summary>
    /// Encodes this operation into <paramref name="buffer"/>.
    /// </summary>
    /// <param name="buffer">Destination buffer sized to <see cref="CallDataLength"/>.</param>
    public void Encode(Span<byte> buffer);

    /// <summary>
    /// Reads the expected byte length for this operation's raw result from the beginning of <paramref name="resultData"/>.
    /// </summary>
    /// <param name="resultData">The result payload beginning at this operation's output offset.</param>
    /// <returns>The total number of bytes consumed by this operation's result.</returns>
    public int ParseResultLength(ReadOnlySpan<byte> resultData);

    /// <summary>
    /// Creates a contract-call query that returns either the decoded value or revert data.
    /// </summary>
    /// <typeparam name="T">The decoded return type of the contract call.</typeparam>
    /// <param name="input">The contract call input to execute.</param>
    /// <returns>A query that never throws for EVM reverts or malformed return bytes and returns a <see cref="CallResult{T}"/>.</returns>
    public static IQuery<CallResult<T>> SafeCall<T>(IContractCall<T> input)
        => new CallQueryOperation<T>(input);

    /// <summary>
    /// Creates a contract-call query that unwraps successful results and throws on EVM revert.
    /// </summary>
    /// <typeparam name="T">The decoded return type of the contract call.</typeparam>
    /// <param name="input">The contract call input to execute.</param>
    /// <returns>A query that yields the decoded return value.</returns>
    /// <exception cref="EtherSharp.Common.Exceptions.CallRevertedException">Thrown when the call reverts.</exception>
    public static IQuery<T> Call<T>(IContractCall<T> input)
        => SafeCall(input).Map(x => x.Unwrap());

    /// <summary>
    /// Creates a contract-call query that returns call success/revert information and measured gas usage.
    /// </summary>
    /// <typeparam name="T">The decoded return type of the contract call.</typeparam>
    /// <param name="input">The contract call input to execute.</param>
    /// <returns>A query that returns both <see cref="CallResult{T}"/> and gas used.</returns>
    public static IQuery<(CallResult<T>, ulong)> SafeCallAndMeasureGas<T>(IContractCall<T> input)
        => new CallAndMeasureGasQueryOperation<T>(input);

    /// <summary>
    /// Creates a contract-call query that unwraps successful results, throws on revert, and returns gas usage.
    /// </summary>
    /// <typeparam name="T">The decoded return type of the contract call.</typeparam>
    /// <param name="input">The contract call input to execute.</param>
    /// <returns>A query that yields the decoded return value and gas used.</returns>
    /// <exception cref="EtherSharp.Common.Exceptions.CallRevertedException">Thrown when the call reverts.</exception>
    public static IQuery<(T, ulong)> CallAndMeasureGas<T>(IContractCall<T> input)
        => SafeCallAndMeasureGas(input).Map(x =>
        {
            var (result, gasUsed) = x;
            return (result.Unwrap(), gasUsed);
        });

    /// <summary>
    /// Creates a flash-call query that executes against ephemeral code and returns success/revert information.
    /// </summary>
    /// <typeparam name="T">The decoded return type of the contract call.</typeparam>
    /// <param name="code">The initcode or runtime code to execute for the flash call.</param>
    /// <param name="input">The flash-call input to execute against the code.</param>
    /// <returns>A query that never throws for EVM reverts or malformed return bytes and returns a <see cref="CallResult{T}"/>.</returns>
    public static IQuery<CallResult<T>> SafeFlashCall<T>(IFlashCode code, IFlashCall<T> input)
        => new SafeFlashCallQueryOperation<T>(code, input);

    /// <summary>
    /// Creates a flash-call query that unwraps successful results and throws on EVM revert.
    /// </summary>
    /// <typeparam name="T">The decoded return type of the contract call.</typeparam>
    /// <param name="code">The initcode or runtime code to execute for the flash call.</param>
    /// <param name="input">The flash-call input to execute against the code.</param>
    /// <returns>A query that yields the decoded return value.</returns>
    /// <exception cref="EtherSharp.Common.Exceptions.CallRevertedException">Thrown when the call reverts.</exception>
    public static IQuery<T> FlashCall<T>(IFlashCode code, IFlashCall<T> input)
        => SafeFlashCall(code, input).Map(x => x.Unwrap());

    /// <summary>
    /// Creates a query that returns deployed bytecode for <paramref name="contract"/>.
    /// </summary>
    public static IQuery<EVMByteCode> GetCode(in Address contract)
        => new GetCodeQueryOperation(in contract);

    /// <summary>
    /// Creates a query that returns whether <paramref name="address"/> currently has deployed bytecode.
    /// </summary>
    public static IQuery<bool> HasCode(in Address address)
        => new HasCodeQueryOperation(in address);

    /// <summary>
    /// Creates a query that returns the code hash for <paramref name="contract"/>.
    /// </summary>
    public static IQuery<byte[]> GetCodeHash(in Address contract)
        => new GetCodeHashQueryOperation(in contract);

    /// <summary>
    /// Creates a query that returns the current block number.
    /// </summary>
    public static IQuery<ulong> GetBlockNumber()
        => new GetBlockNumberQueryOperation();

    /// <summary>
    /// Creates a query that calls Arbitrum's <c>ArbSys.arbBlockNumber()</c> precompile and returns the current L2 block number.
    /// </summary>
    /// <remarks>
    /// This query is only supported on Arbitrum-based chains. On other EVM chains the underlying call will revert.
    /// </remarks>
    public static IQuery<ulong> GetArbitrumBlockNumber()
        => Call(IContractCall<UInt256>.ForContractCall(
            "0x0000000000000000000000000000000000000064",
            0,
            Convert.FromHexString("A3B1B31D"),
            new ABI.AbiEncoder(),
            x => x.UInt256())
        ).Map(x => (ulong) x);

    /// <summary>
    /// Creates a query that returns the current block timestamp as a <see cref="DateTimeOffset"/>.
    /// </summary>
    public static IQuery<DateTimeOffset> GetBlockTimestamp()
        => new GetBlockTimestampQueryOperation();

    /// <summary>
    /// Creates a query that returns the current block gas limit.
    /// </summary>
    public static IQuery<ulong> GetBlockGasLimit()
        => new GetBlockGasLimitQueryOperation();

    /// <summary>
    /// Creates a query that returns the block gas price.
    /// </summary>
    public static IQuery<UInt256> GetBlockGasPrice()
        => new GetBlockGasPriceQueryOperation();

    /// <summary>
    /// Creates a query that returns the block base fee.
    /// </summary>
    public static IQuery<UInt256> GetBlockBaseFee()
        => new GetBlockBaseFeeQueryOperation();

    /// <summary>
    /// Creates a query that returns the ETH balance for <paramref name="user"/>.
    /// </summary>
    public static IQuery<UInt256> GetBalance(in Address user)
        => new GetBalanceQueryOperation(in user);

    /// <summary>
    /// Creates a query that reads a raw storage slot by temporarily replacing the target contract's code.
    /// </summary>
    /// <remarks>
    /// The replacement applies to every RPC call used to execute the combined query. Calls to the target's original functions will not work.
    /// </remarks>
    /// <param name="contract">The contract whose storage is read.</param>
    /// <param name="slot">The raw 32-byte storage slot.</param>
    /// <exception cref="InvalidOperationException">Thrown during execution when another query requires a conflicting override for the same contract.</exception>
    public static IQuery<Bytes32> ReadStorageBytes32(in Address contract, in Bytes32 slot)
        => new ReadStorageQueryOperation(in contract, in slot, null);

    /// <summary>
    /// Creates a query that reads a raw storage slot while preserving calls to the target contract's original runtime bytecode.
    /// </summary>
    /// <remarks>
    /// The replacement applies to every RPC call used to execute the combined query. Non-storage-read calls are delegated to a transient
    /// state-override account containing <paramref name="originalByteCode"/>.
    /// </remarks>
    /// <param name="contract">The contract whose storage is read.</param>
    /// <param name="slot">The raw 32-byte storage slot.</param>
    /// <param name="originalByteCode">The target contract's original deployed runtime bytecode.</param>
    /// <exception cref="InvalidOperationException">Thrown during execution when another query requires a conflicting override for the target
    /// or transient contract address.</exception>
    public static IQuery<Bytes32> ReadStorageBytes32(in Address contract, in Bytes32 slot, EVMByteCode originalByteCode)
        => new ReadStorageQueryOperation(in contract, in slot, originalByteCode);

    /// <summary>
    /// Creates a query that reads an address from a raw storage slot by temporarily replacing the target contract's code.
    /// </summary>
    /// <remarks>
    /// The address is read from the low 20 bytes of the storage slot. The replacement applies to every RPC call used to execute the
    /// combined query. Calls to the target's original functions will not work.
    /// </remarks>
    /// <param name="contract">The contract whose storage is read.</param>
    /// <param name="slot">The raw 32-byte storage slot.</param>
    /// <exception cref="InvalidOperationException">Thrown during execution when another query requires a conflicting override for the same contract.</exception>
    public static IQuery<Address> ReadStorageAddress(in Address contract, in Bytes32 slot)
        => ReadStorageBytes32(in contract, in slot).Map(static value => Address.FromBytes(value.DangerousGetReadOnlySpan()[^Address.BYTES_LENGTH..]));

    /// <summary>
    /// Creates a query that reads an address from a raw storage slot while preserving calls to the target contract's original runtime bytecode.
    /// </summary>
    /// <remarks>
    /// The address is read from the low 20 bytes of the storage slot. The replacement applies to every RPC call used to execute the combined
    /// query. Non-storage-read calls are delegated to a transient state-override account containing <paramref name="originalByteCode"/>.
    /// </remarks>
    /// <param name="contract">The contract whose storage is read.</param>
    /// <param name="slot">The raw 32-byte storage slot.</param>
    /// <param name="originalByteCode">The target contract's original deployed runtime bytecode.</param>
    /// <exception cref="InvalidOperationException">Thrown during execution when another query requires a conflicting override for the target
    /// or transient contract address.</exception>
    public static IQuery<Address> ReadStorageAddress(in Address contract, in Bytes32 slot, EVMByteCode originalByteCode)
        => ReadStorageBytes32(in contract, in slot, originalByteCode)
            .Map(static value => Address.FromBytes(value.DangerousGetReadOnlySpan()[^Address.BYTES_LENGTH..]));

    /// <summary>
    /// Creates a query that reads an unsigned 256-bit integer from a raw storage slot by temporarily replacing the target contract's code.
    /// </summary>
    /// <param name="contract">The contract whose storage is read.</param>
    /// <param name="slot">The raw 32-byte storage slot.</param>
    /// <exception cref="InvalidOperationException">Thrown during execution when another query requires a conflicting override for the same contract.</exception>
    public static IQuery<UInt256> ReadStorageUInt256(in Address contract, in Bytes32 slot)
        => ReadStorageBytes32(in contract, in slot).Map(static value => BinaryPrimitives.ReadUInt256BigEndian(value.DangerousGetReadOnlySpan()));

    /// <summary>
    /// Creates a query that reads an unsigned 256-bit integer from a raw storage slot while preserving calls to the target contract's original runtime bytecode.
    /// </summary>
    /// <param name="contract">The contract whose storage is read.</param>
    /// <param name="slot">The raw 32-byte storage slot.</param>
    /// <param name="originalByteCode">The target contract's original deployed runtime bytecode.</param>
    /// <exception cref="InvalidOperationException">Thrown during execution when another query requires a conflicting override for the target
    /// or transient contract address.</exception>
    public static IQuery<UInt256> ReadStorageUInt256(in Address contract, in Bytes32 slot, EVMByteCode originalByteCode)
        => ReadStorageBytes32(in contract, in slot, originalByteCode)
            .Map(static value => BinaryPrimitives.ReadUInt256BigEndian(value.DangerousGetReadOnlySpan()));

    /// <summary>
    /// Creates a query that reads a signed 256-bit integer from a raw storage slot by temporarily replacing the target contract's code.
    /// </summary>
    /// <param name="contract">The contract whose storage is read.</param>
    /// <param name="slot">The raw 32-byte storage slot.</param>
    /// <exception cref="InvalidOperationException">Thrown during execution when another query requires a conflicting override for the same contract.</exception>
    public static IQuery<Int256> ReadStorageInt256(in Address contract, in Bytes32 slot)
        => ReadStorageBytes32(in contract, in slot).Map(static value => BinaryPrimitives.ReadInt256BigEndian(value.DangerousGetReadOnlySpan()));

    /// <summary>
    /// Creates a query that reads a signed 256-bit integer from a raw storage slot while preserving calls to the target contract's original runtime bytecode.
    /// </summary>
    /// <param name="contract">The contract whose storage is read.</param>
    /// <param name="slot">The raw 32-byte storage slot.</param>
    /// <param name="originalByteCode">The target contract's original deployed runtime bytecode.</param>
    /// <exception cref="InvalidOperationException">Thrown during execution when another query requires a conflicting override for the target
    /// or transient contract address.</exception>
    public static IQuery<Int256> ReadStorageInt256(in Address contract, in Bytes32 slot, EVMByteCode originalByteCode)
        => ReadStorageBytes32(in contract, in slot, originalByteCode)
            .Map(static value => BinaryPrimitives.ReadInt256BigEndian(value.DangerousGetReadOnlySpan()));

    /// <summary>
    /// Creates a query that returns the current chain id.
    /// </summary>
    public static IQuery<ulong> GetChainId()
        => new GetChainIdQueryOperation();

    /// <summary>
    /// Creates a query that probes EVM feature support and returns a compatibility report.
    /// </summary>
    public static IQuery<CompatibilityReport> GetCompatibilityReport()
        => new GetCompatibilityQueryOperation();

    /// <summary>
    /// Creates a query that returns the remaining gas inside the query execution context.
    /// </summary>
    public static IQuery<UInt256> GetRemainingGas()
        => new RemainingGasOperation();

    /// <summary>
    /// Creates a query with no underlying operations that always returns <paramref name="value"/>.
    /// </summary>
    public static IQuery<T> Noop<T>(T value)
        => new NoopQueryOperation<T>(value);

    /// <summary>
    /// Maps the result of <paramref name="query"/> to a new type while preserving its underlying operations.
    /// </summary>
    public static IQuery<TTo> Map<TFrom, TTo>(IQuery<TFrom> query, Func<TFrom, TTo> mapping)
        => new MappedQuery<TFrom, TTo>(query, mapping);

    /// <summary>
    /// Combines an arbitrary number of queries into a single query that returns an ordered result list.
    /// </summary>
    public static IQuery<T[]> Range<T>(params IEnumerable<IQuery<T>> queries)
        => Range<T>(queries.ToArray().AsMemory());

    /// <summary>
    /// Combines an arbitrary number of queries into a single query that returns an ordered result list.
    /// </summary>
    [OverloadResolutionPriority(1)]
    public static IQuery<T[]> Range<T>(ReadOnlyMemory<IQuery<T>> queries)
        => new RangeQuery<T>(queries);
}

/// <summary>
/// Represents a call payload that returns a result of type <typeparamref name="TQuery"/> when eth_call'ed.
/// </summary>
/// <typeparam name="TQuery">The parsed result type returned by this query.</typeparam>
public partial interface IQuery<TQuery>
{
    /// <summary>
    /// Gets the number of low-level operations contributed to an execution plan by this query.
    /// </summary>
    public int OperationCount
        => this is IQuery
            ? 1
            : throw new InvalidOperationException("Composite queries must declare their operation count.");

    /// <summary>
    /// Adds this query's low-level operations and state overrides to <paramref name="plan"/>.
    /// </summary>
    /// <remarks>
    /// Implementations must add exactly <see cref="OperationCount"/> operations, in the order expected by
    /// <see cref="ReadResultFrom"/>.
    /// </remarks>
    public void AddTo(IQueryPlan plan)
    {
        if(this is not IQuery query)
        {
            throw new InvalidOperationException("Composite queries must implement query collection.");
        }

        plan.AddOperation(query);
    }

    /// <summary>
    /// Decodes this query's result from its low-level operation results.
    /// </summary>
    /// <param name="queryResults">The results in the same order that the operations were added to the plan.</param>
    public TQuery ReadResultFrom(params ReadOnlySpan<ReadOnlyMemory<byte>> queryResults);

    /// <summary>
    /// Creates a query by mapping the output of <paramref name="query"/> to <typeparamref name="TQuery"/>.
    /// </summary>
    public static IQuery<TQuery> From<TFrom>(IQuery<TFrom> query, Func<TFrom, TQuery> mapping)
        => IQuery.Map(query, mapping);

    /// <summary>
    /// Maps this query result to a new type while preserving its underlying operations.
    /// </summary>
    public IQuery<TTo> Map<TTo>(Func<TQuery, TTo> mapping)
        => IQuery.Map(this, mapping);
}
