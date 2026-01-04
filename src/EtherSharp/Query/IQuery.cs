using EtherSharp.Common.Exceptions;
using EtherSharp.Query.Operations;
using EtherSharp.Tx;
using EtherSharp.Types;
using System.Numerics;

namespace EtherSharp.Query;

public interface IQuery
{
    public int CallDataLength { get; }
    public BigInteger EthValue { get; }

    public void Encode(Span<byte> buffer);
    public int ParseResultLength(ReadOnlySpan<byte> resultData);

    public static IQuery<QueryResult<T>> SafeCall<T>(IContractCall<T> input)
        => new CallQueryOperation<T>(input);
    public static IQuery<T> Call<T>(IContractCall<T> input)
        => SafeCall(input).Map(x => x switch
        {
            QueryResult<T>.Success s => s.Value,
            QueryResult<T>.Reverted r => throw CallRevertedException.Parse(input.To, r.Data),
            _ => throw new ImpossibleException()
        });

    public static IQuery<(QueryResult<T>, ulong)> SafeCallAndMeasureGas<T>(IContractCall<T> input)
        => new CallAndMeasureGasQueryOperation<T>(input);
    public static IQuery<(T, ulong)> CallAndMeasureGas<T>(IContractCall<T> input)
        => SafeCallAndMeasureGas(input).Map(x =>
        {
            var (result, gasUsed) = x;
            var unwrapped = result switch
            {
                QueryResult<T>.Success s => s.Value,
                QueryResult<T>.Reverted r => throw CallRevertedException.Parse(input.To, r.Data),
                _ => throw new ImpossibleException()
            };
            return (unwrapped, gasUsed);
        });

    public static IQuery<QueryResult<T>> SafeFlashCall<T>(IContractDeployment deployment, IContractCall<T> input)
        => new SafeFlashCallQueryOperation<T>(deployment, input);
    public static IQuery<T> FlashCall<T>(IContractDeployment deployment, IContractCall<T> input)
        => SafeFlashCall(deployment, input).Map(x => x switch
        {
            QueryResult<T>.Success s => s.Value,
            QueryResult<T>.Reverted r => throw CallRevertedException.Parse(input.To, r.Data),
            _ => throw new ImpossibleException()
        });

    public static IQuery<EVMByteCode> GetCode(Address contract)
        => new GetCodeQueryOperation(contract);

    public static IQuery<byte[]> GetCodeHash(Address contract)
        => new GetCodeHashQueryOperation(contract);

    public static IQuery<ulong> GetBlockNumber()
        => new GetBlockNumberQueryOperation();

    public static IQuery<DateTimeOffset> GetBlockTimestamp()
        => new GetBlockTimestampQueryOperation();

    public static IQuery<ulong> GetBlockGasLimit()
        => new GetBlockGasLimitQueryOperation();

    public static IQuery<BigInteger> GetBlockGasPrice()
        => new GetBlockGasPriceQueryOperation();

    public static IQuery<BigInteger> GetBlockBaseFee()
        => new GetBlockBaseFeeQueryOperation();

    public static IQuery<BigInteger> GetBalance(Address user)
        => new GetBalanceQueryOperation(user);

    public static IQuery<ulong> GetChainId()
        => new GetChainIdQueryOperation();

    public static IQuery<TTo> Map<TFrom, TTo>(IQuery<TFrom> query, Func<TFrom, TTo> mapping)
        => new Query<TTo>(query.Queries, results => mapping(query.ReadResultFrom(results)));

    public static IQuery<(T1, T2)> Combine<T1, T2>(IQuery<T1> q1, IQuery<T2> q2)
        => new Query<(T1, T2)>(
            [.. q1.Queries, .. q2.Queries],
            results => (
                q1.ReadResultFrom(results[..]),
                q2.ReadResultFrom(results[q1.Queries.Count..])
            )
        );

    public static IQuery<(T1, T2, T3)> Combine<T1, T2, T3>(IQuery<T1> q1, IQuery<T2> q2, IQuery<T3> q3)
        => new Query<(T1, T2, T3)>(
            [.. q1.Queries, .. q2.Queries, .. q3.Queries],
            results => (
                q1.ReadResultFrom(results[..]),
                q2.ReadResultFrom(results[q1.Queries.Count..]),
                q3.ReadResultFrom(results[(q1.Queries.Count + q2.Queries.Count)..])
            )
        );

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
}

/// <summary>
/// Represents a call payload that returns a result of type <typeparamref name="TQuery"/> when eth_call'ed.
/// </summary>
/// <typeparam name="TQuery"></typeparam>
public interface IQuery<TQuery>
{
    internal IReadOnlyList<IQuery> Queries { get; }
    internal TQuery ReadResultFrom(params ReadOnlySpan<byte[]> queryResults);

    public static IQuery<TQuery> From<TFrom>(IQuery<TFrom> query, Func<TFrom, TQuery> mapping)
        => IQuery.Map(query, mapping);

    public IQuery<TTo> Map<TTo>(Func<TQuery, TTo> mapping)
        => IQuery.Map(this, mapping);
}
