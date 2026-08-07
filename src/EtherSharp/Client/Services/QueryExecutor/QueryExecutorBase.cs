using EtherSharp.Common.Exceptions;
using EtherSharp.Numerics;
using EtherSharp.Query;
using EtherSharp.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Buffers;

namespace EtherSharp.Client.Services.QueryExecutor;

internal abstract class QueryExecutorBase : IQueryExecutor
{
    private readonly ILogger? _logger;
    private readonly IEtherClient _client;

    protected QueryExecutorBase(IServiceProvider provider)
    {
        _logger = provider.GetService<ILoggerFactory>()?.CreateLogger(GetType());
        _client = provider.GetRequiredService<IEtherClient>();
    }

    protected abstract Address? CallAddress { get; }
    protected abstract void PreparePlan(QueryPlan plan, QuerierByteCode querier);
    protected abstract int GetMaxPayloadSize(QuerierByteCode querier, ulong? gasLimit, TargetHeight targetHeight);
    protected abstract int GetMaxResultSize(TargetHeight targetHeight);

    protected abstract Task<TxCallResult> ExecuteBatchAsync(
        QuerierByteCode querier,
        UInt256 value,
        ReadOnlyMemory<byte> payload,
        ulong? gasLimit,
        CallOptions options,
        CancellationToken cancellationToken
    );

    public async Task<TQuery> ExecuteQueryAsync<TQuery>(
        IQuery<TQuery> query,
        ulong? gasLimit,
        CallOptions options,
        CancellationToken cancellationToken)
    {
        var plan = new QueryPlan(query.OperationCount, options.StateOverrides);
        plan.Add(query);

        var outputs = new ReadOnlyMemory<byte>[plan.Queries.Count];

        if(plan.Count == 0)
        {
            return query.ReadResultFrom(outputs);
        }

        bool supportsCancun = _client.IsInitialized && _client.CompatibilityReport is not null && _client.CompatibilityReport.SupportsPush0;
        var querier = supportsCancun && (options.TargetHeight == TargetHeight.Latest || options.TargetHeight == TargetHeight.Pending)
            ? QuerierUtils.CancunQuerier
            : QuerierUtils.LondonQuerier;

        PreparePlan(plan, querier);

        int maxPayloadSize = GetMaxPayloadSize(querier, gasLimit, options.TargetHeight);
        int maxResultSize = GetMaxResultSize(options.TargetHeight);
        var buffer = ReadOnlyMemory<byte>.Empty;
        int requestCount = 0;

        for(int i = 0; i < plan.Queries.Count; i++)
        {
            var q = plan.Queries[i];
            if(buffer.Length == 0)
            {
                requestCount++;

                byte[] payloadBytes = QuerierUtils.EncodeCalls(
                    plan.Queries,
                    i,
                    maxPayloadSize,
                    maxResultSize,
                    out int payloadSize,
                    out int callCount,
                    out var ethValue
                );

                try
                {
                    if(callCount == 0)
                    {
                        throw new InvalidOperationException("Call is too large to be executed within batch");
                    }

                    var callResult = await ExecuteBatchAsync(
                        querier,
                        ethValue,
                        payloadBytes.AsMemory(0, payloadSize),
                        gasLimit,
                        options with { StateOverrides = plan.StateOverrides },
                        cancellationToken
                    );

                    if(!callResult.Success)
                    {
                        throw CallRevertedException.Parse(CallAddress, callResult.Data.Span);
                    }

                    var output = callResult.Data;

                    if(output.Length == 0)
                    {
                        throw new InvalidOperationException("Call is too expensive to be executed within batch");
                    }

                    buffer = output;

                    if(_logger?.IsEnabled(LogLevel.Trace) == true)
                    {
                        _logger.LogTrace(
                            "Query request {request} completed with {operations} operation(s)",
                            requestCount,
                            callCount);
                    }
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(payloadBytes);
                }
            }

            int sliceLength = q.ParseResultLength(buffer.Span);
            outputs[i] = buffer[0..sliceLength];
            buffer = buffer[sliceLength..];
        }

        if(requestCount > 1 && _logger?.IsEnabled(LogLevel.Debug) == true)
        {
            _logger.LogDebug("Batch query processing too expensive, required {requests} requests", requestCount);
        }

        return query.ReadResultFrom(outputs);
    }
}
