using EtherSharp.Client.Services.FlashCall;
using EtherSharp.Common.Exceptions;
using EtherSharp.Query;
using EtherSharp.Tx;
using EtherSharp.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Buffers;

namespace EtherSharp.Client.Services.QueryExecutor;

internal sealed class QueryExecutor(
    FlashCallExecutor flashCallExecutor,
    IServiceProvider provider)
{
    private readonly FlashCallExecutor _flashCallExecutor = flashCallExecutor;
    private readonly ILogger? _logger = provider.GetService<ILoggerFactory>()?.CreateLogger<QueryExecutor>();
    private readonly IEtherClient _client = provider.GetRequiredService<IEtherClient>();

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

        int maxPayloadSize = _flashCallExecutor.GetMaxPayloadSize(querier.Code, gasLimit, options.TargetHeight);
        int maxResultSize = _flashCallExecutor.GetMaxResultSize(querier.Code, options.TargetHeight);
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

                    var callResult = await _flashCallExecutor.ExecuteFlashCallAsync(
                        querier.Code,
                        IFlashCall.ForRawFlashCall(ethValue, payloadBytes.AsMemory(0, payloadSize)),
                        gasLimit,
                        options with { StateOverrides = plan.StateOverrides },
                        cancellationToken
                    );

                    if(!callResult.Success)
                    {
                        throw CallRevertedException.Parse(null, callResult.Data.Span);
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
