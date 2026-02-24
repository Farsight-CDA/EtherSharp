using EtherSharp.Common.Exceptions;
using EtherSharp.Contract;
using EtherSharp.Numerics;
using EtherSharp.Query.Operations;
using EtherSharp.Tx;
using EtherSharp.Types;

namespace EtherSharp.Query;

/// <summary>
/// Represents a low-level query operation that can be encoded into the aggregated query payload.
/// </summary>
public interface IQuery
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
    /// <returns>A query that never throws for EVM reverts and returns a <see cref="QueryResult{T}"/>.</returns>
    public static IQuery<QueryResult<T>> SafeCall<T>(IContractCall<T> input)
        => new CallQueryOperation<T>(input);

    /// <summary>
    /// Creates a contract-call query that unwraps successful results and throws on EVM revert.
    /// </summary>
    /// <typeparam name="T">The decoded return type of the contract call.</typeparam>
    /// <param name="input">The contract call input to execute.</param>
    /// <returns>A query that yields the decoded return value.</returns>
    /// <exception cref="CallRevertedException">Thrown when the call reverts.</exception>
    public static IQuery<T> Call<T>(IContractCall<T> input)
        => SafeCall(input).Map(x => x switch
        {
            QueryResult<T>.Success s => s.Value,
            QueryResult<T>.Reverted r => throw CallRevertedException.Parse(input.To, r.Data.Span),
            _ => throw new ImpossibleException()
        });

    /// <summary>
    /// Creates a contract-call query that returns call success/revert information and measured gas usage.
    /// </summary>
    /// <typeparam name="T">The decoded return type of the contract call.</typeparam>
    /// <param name="input">The contract call input to execute.</param>
    /// <returns>A query that returns both <see cref="QueryResult{T}"/> and gas used.</returns>
    public static IQuery<(QueryResult<T>, ulong)> SafeCallAndMeasureGas<T>(IContractCall<T> input)
        => new CallAndMeasureGasQueryOperation<T>(input);

    /// <summary>
    /// Creates a contract-call query that unwraps successful results, throws on revert, and returns gas usage.
    /// </summary>
    /// <typeparam name="T">The decoded return type of the contract call.</typeparam>
    /// <param name="input">The contract call input to execute.</param>
    /// <returns>A query that yields the decoded return value and gas used.</returns>
    /// <exception cref="CallRevertedException">Thrown when the call reverts.</exception>
    public static IQuery<(T, ulong)> CallAndMeasureGas<T>(IContractCall<T> input)
        => SafeCallAndMeasureGas(input).Map(x =>
        {
            var (result, gasUsed) = x;
            var unwrapped = result switch
            {
                QueryResult<T>.Success s => s.Value,
                QueryResult<T>.Reverted r => throw CallRevertedException.Parse(input.To, r.Data.Span),
                _ => throw new ImpossibleException()
            };
            return (unwrapped, gasUsed);
        });

    /// <summary>
    /// Creates a flash-call query that deploys ephemeral bytecode and then executes a call against it, returning success/revert information.
    /// </summary>
    /// <typeparam name="T">The decoded return type of the contract call.</typeparam>
    /// <param name="deployment">The contract deployment to execute for the flash call.</param>
    /// <param name="input">The contract call input to execute against the deployed code.</param>
    /// <returns>A query that never throws for EVM reverts and returns a <see cref="QueryResult{T}"/>.</returns>
    public static IQuery<QueryResult<T>> SafeFlashCall<T>(IContractDeployment deployment, IContractCall<T> input)
        => new SafeFlashCallQueryOperation<T>(deployment, input);

    /// <summary>
    /// Creates a flash-call query that unwraps successful results and throws on EVM revert.
    /// </summary>
    /// <typeparam name="T">The decoded return type of the contract call.</typeparam>
    /// <param name="deployment">The contract deployment to execute for the flash call.</param>
    /// <param name="input">The contract call input to execute against the deployed code.</param>
    /// <returns>A query that yields the decoded return value.</returns>
    /// <exception cref="CallRevertedException">Thrown when the call reverts.</exception>
    public static IQuery<T> FlashCall<T>(IContractDeployment deployment, IContractCall<T> input)
        => SafeFlashCall(deployment, input).Map(x => x switch
        {
            QueryResult<T>.Success s => s.Value,
            QueryResult<T>.Reverted r => throw CallRevertedException.Parse(input.To, r.Data.Span),
            _ => throw new ImpossibleException()
        });

    /// <summary>
    /// Creates a query that returns deployed bytecode for <paramref name="contract"/>.
    /// </summary>
    public static IQuery<EVMByteCode> GetCode(Address contract)
        => new GetCodeQueryOperation(contract);

    /// <summary>
    /// Creates a query that returns the code hash for <paramref name="contract"/>.
    /// </summary>
    public static IQuery<byte[]> GetCodeHash(Address contract)
        => new GetCodeHashQueryOperation(contract);

    /// <summary>
    /// Creates a query that returns the current block number.
    /// </summary>
    public static IQuery<ulong> GetBlockNumber()
        => new GetBlockNumberQueryOperation();

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
    public static IQuery<UInt256> GetBalance(Address user)
        => new GetBalanceQueryOperation(user);

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
        => new Query<TTo>(query.Queries, results => mapping(query.ReadResultFrom(results)));

    /// <summary>
    /// Combines two queries into a single query that returns a tuple of both results.
    /// </summary>
    public static IQuery<(T1, T2)> Combine<T1, T2>(IQuery<T1> q1, IQuery<T2> q2)
        => new Query<(T1, T2)>(
            [.. q1.Queries, .. q2.Queries],
            results => (
                q1.ReadResultFrom(results[..]),
                q2.ReadResultFrom(results[q1.Queries.Count..])
            )
        );

    /// <summary>
    /// Combines three queries into a single query that returns a tuple of all results.
    /// </summary>
    public static IQuery<(T1, T2, T3)> Combine<T1, T2, T3>(IQuery<T1> q1, IQuery<T2> q2, IQuery<T3> q3)
        => new Query<(T1, T2, T3)>(
            [.. q1.Queries, .. q2.Queries, .. q3.Queries],
            results => (
                q1.ReadResultFrom(results[..]),
                q2.ReadResultFrom(results[q1.Queries.Count..]),
                q3.ReadResultFrom(results[(q1.Queries.Count + q2.Queries.Count)..])
            )
        );

    /// <summary>
    /// Combines four queries into a single query that returns a tuple of all results.
    /// </summary>
    public static IQuery<(T1, T2, T3, T4)> Combine<T1, T2, T3, T4>(IQuery<T1> q1, IQuery<T2> q2, IQuery<T3> q3, IQuery<T4> q4)
        => new Query<(T1, T2, T3, T4)>(
            [.. q1.Queries, .. q2.Queries, .. q3.Queries, .. q4.Queries],
            results =>
            {
                int offset = 0;
                int count1 = q1.Queries.Count;
                int count2 = q2.Queries.Count;
                int count3 = q3.Queries.Count;
                int count4 = q4.Queries.Count;

                return (
                    q1.ReadResultFrom(results[offset..(offset += count1)]),
                    q2.ReadResultFrom(results[offset..(offset += count2)]),
                    q3.ReadResultFrom(results[offset..(offset += count3)]),
                    q4.ReadResultFrom(results[offset..(offset += count4)])
                );
            }
        );

    /// <summary>
    /// Combines five queries into a single query that returns a tuple of all results.
    /// </summary>
    public static IQuery<(T1, T2, T3, T4, T5)> Combine<T1, T2, T3, T4, T5>(IQuery<T1> q1, IQuery<T2> q2, IQuery<T3> q3, IQuery<T4> q4, IQuery<T5> q5)
        => new Query<(T1, T2, T3, T4, T5)>(
            [.. q1.Queries, .. q2.Queries, .. q3.Queries, .. q4.Queries, .. q5.Queries],
            results =>
            {
                int offset = 0;
                int count1 = q1.Queries.Count;
                int count2 = q2.Queries.Count;
                int count3 = q3.Queries.Count;
                int count4 = q4.Queries.Count;
                int count5 = q5.Queries.Count;

                return (
                    q1.ReadResultFrom(results[offset..(offset += count1)]),
                    q2.ReadResultFrom(results[offset..(offset += count2)]),
                    q3.ReadResultFrom(results[offset..(offset += count3)]),
                    q4.ReadResultFrom(results[offset..(offset += count4)]),
                    q5.ReadResultFrom(results[offset..(offset += count5)])
                );
            }
        );

    /// <summary>
    /// Combines six queries into a single query that returns a tuple of all results.
    /// </summary>
    public static IQuery<(T1, T2, T3, T4, T5, T6)> Combine<T1, T2, T3, T4, T5, T6>(IQuery<T1> q1, IQuery<T2> q2, IQuery<T3> q3, IQuery<T4> q4, IQuery<T5> q5, IQuery<T6> q6)
        => new Query<(T1, T2, T3, T4, T5, T6)>(
            [.. q1.Queries, .. q2.Queries, .. q3.Queries, .. q4.Queries, .. q5.Queries, .. q6.Queries],
            results =>
            {
                int offset = 0;
                int count1 = q1.Queries.Count;
                int count2 = q2.Queries.Count;
                int count3 = q3.Queries.Count;
                int count4 = q4.Queries.Count;
                int count5 = q5.Queries.Count;
                int count6 = q6.Queries.Count;

                return (
                    q1.ReadResultFrom(results[offset..(offset += count1)]),
                    q2.ReadResultFrom(results[offset..(offset += count2)]),
                    q3.ReadResultFrom(results[offset..(offset += count3)]),
                    q4.ReadResultFrom(results[offset..(offset += count4)]),
                    q5.ReadResultFrom(results[offset..(offset += count5)]),
                    q6.ReadResultFrom(results[offset..(offset += count6)])
                );
            }
        );

    /// <summary>
    /// Combines seven queries into a single query that returns a tuple of all results.
    /// </summary>
    public static IQuery<(T1, T2, T3, T4, T5, T6, T7)> Combine<T1, T2, T3, T4, T5, T6, T7>(IQuery<T1> q1, IQuery<T2> q2, IQuery<T3> q3, IQuery<T4> q4, IQuery<T5> q5, IQuery<T6> q6, IQuery<T7> q7)
        => new Query<(T1, T2, T3, T4, T5, T6, T7)>(
            [.. q1.Queries, .. q2.Queries, .. q3.Queries, .. q4.Queries, .. q5.Queries, .. q6.Queries, .. q7.Queries],
            results =>
            {
                int offset = 0;
                int count1 = q1.Queries.Count;
                int count2 = q2.Queries.Count;
                int count3 = q3.Queries.Count;
                int count4 = q4.Queries.Count;
                int count5 = q5.Queries.Count;
                int count6 = q6.Queries.Count;
                int count7 = q7.Queries.Count;

                return (
                    q1.ReadResultFrom(results[offset..(offset += count1)]),
                    q2.ReadResultFrom(results[offset..(offset += count2)]),
                    q3.ReadResultFrom(results[offset..(offset += count3)]),
                    q4.ReadResultFrom(results[offset..(offset += count4)]),
                    q5.ReadResultFrom(results[offset..(offset += count5)]),
                    q6.ReadResultFrom(results[offset..(offset += count6)]),
                    q7.ReadResultFrom(results[offset..(offset += count7)])
                );
            }
        );

    /// <summary>
    /// Combines eight queries into a single query that returns a tuple of all results.
    /// </summary>
    public static IQuery<(T1, T2, T3, T4, T5, T6, T7, T8)> Combine<T1, T2, T3, T4, T5, T6, T7, T8>(IQuery<T1> q1, IQuery<T2> q2, IQuery<T3> q3, IQuery<T4> q4, IQuery<T5> q5, IQuery<T6> q6, IQuery<T7> q7, IQuery<T8> q8)
        => new Query<(T1, T2, T3, T4, T5, T6, T7, T8)>(
            [.. q1.Queries, .. q2.Queries, .. q3.Queries, .. q4.Queries, .. q5.Queries, .. q6.Queries, .. q7.Queries, .. q8.Queries],
            results =>
            {
                int offset = 0;
                int count1 = q1.Queries.Count;
                int count2 = q2.Queries.Count;
                int count3 = q3.Queries.Count;
                int count4 = q4.Queries.Count;
                int count5 = q5.Queries.Count;
                int count6 = q6.Queries.Count;
                int count7 = q7.Queries.Count;
                int count8 = q8.Queries.Count;

                return (
                    q1.ReadResultFrom(results[offset..(offset += count1)]),
                    q2.ReadResultFrom(results[offset..(offset += count2)]),
                    q3.ReadResultFrom(results[offset..(offset += count3)]),
                    q4.ReadResultFrom(results[offset..(offset += count4)]),
                    q5.ReadResultFrom(results[offset..(offset += count5)]),
                    q6.ReadResultFrom(results[offset..(offset += count6)]),
                    q7.ReadResultFrom(results[offset..(offset += count7)]),
                    q8.ReadResultFrom(results[offset..(offset += count8)])
                );
            }
        );

    /// <summary>
    /// Combines nine queries into a single query that returns a tuple of all results.
    /// </summary>
    public static IQuery<(T1, T2, T3, T4, T5, T6, T7, T8, T9)> Combine<T1, T2, T3, T4, T5, T6, T7, T8, T9>(IQuery<T1> q1, IQuery<T2> q2, IQuery<T3> q3, IQuery<T4> q4, IQuery<T5> q5, IQuery<T6> q6, IQuery<T7> q7, IQuery<T8> q8, IQuery<T9> q9)
        => new Query<(T1, T2, T3, T4, T5, T6, T7, T8, T9)>(
            [.. q1.Queries, .. q2.Queries, .. q3.Queries, .. q4.Queries, .. q5.Queries, .. q6.Queries, .. q7.Queries, .. q8.Queries, .. q9.Queries],
            results =>
            {
                int offset = 0;
                int count1 = q1.Queries.Count;
                int count2 = q2.Queries.Count;
                int count3 = q3.Queries.Count;
                int count4 = q4.Queries.Count;
                int count5 = q5.Queries.Count;
                int count6 = q6.Queries.Count;
                int count7 = q7.Queries.Count;
                int count8 = q8.Queries.Count;
                int count9 = q9.Queries.Count;

                return (
                    q1.ReadResultFrom(results[offset..(offset += count1)]),
                    q2.ReadResultFrom(results[offset..(offset += count2)]),
                    q3.ReadResultFrom(results[offset..(offset += count3)]),
                    q4.ReadResultFrom(results[offset..(offset += count4)]),
                    q5.ReadResultFrom(results[offset..(offset += count5)]),
                    q6.ReadResultFrom(results[offset..(offset += count6)]),
                    q7.ReadResultFrom(results[offset..(offset += count7)]),
                    q8.ReadResultFrom(results[offset..(offset += count8)]),
                    q9.ReadResultFrom(results[offset..(offset += count9)])
                );
            }
        );

    /// <summary>
    /// Combines an arbitrary number of queries into a single query that returns an ordered result list.
    /// </summary>
    public static IQuery<IReadOnlyList<T>> Range<T>(params IEnumerable<IQuery<T>> queries)
    {
        var queryList = queries.ToList();
        return new Query<IReadOnlyList<T>>(
            [.. queryList.SelectMany(q => q.Queries)],
            results =>
            {
                var resultsList = new List<T>(queryList.Count);
                int offset = 0;
                foreach(var q in queryList)
                {
                    int count = q.Queries.Count;
                    resultsList.Add(q.ReadResultFrom(results[offset..(offset + count)]));
                    offset += count;
                }
                return resultsList;
            }
        );
    }
}

/// <summary>
/// Represents a call payload that returns a result of type <typeparamref name="TQuery"/> when eth_call'ed.
/// </summary>
/// <typeparam name="TQuery">The parsed result type returned by this query.</typeparam>
public interface IQuery<TQuery>
{
    internal IReadOnlyList<IQuery> Queries { get; }
    internal TQuery ReadResultFrom(params ReadOnlySpan<ReadOnlyMemory<byte>> queryResults);

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
