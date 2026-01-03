using EtherSharp.Client.Services.FlashCallExecutor;
using EtherSharp.Query;
using EtherSharp.Tx;
using EtherSharp.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EtherSharp.Client.Services.QueryExecutor;

public class FlashCallQueryExecutor(IFlashCallExecutor flashCallExecutor, IServiceProvider provider) : IQueryExecutor
{
    private readonly IFlashCallExecutor _flashCallExecutor = flashCallExecutor;
    private readonly ILogger? _logger = provider.GetService<ILoggerFactory>()?.CreateLogger<FlashCallQueryExecutor>();

    public async Task<TQuery> ExecuteQueryAsync<TQuery>(IQuery<TQuery> query, TargetBlockNumber targetHeight, CancellationToken cancellationToken)
    {
        ReadOnlySpan<byte> buffer = [];
        byte[][] outputs = new byte[query.Queries.Count][];
        int requestCount = 0;

        for(int i = 0; i < query.Queries.Count; i++)
        {
            var q = query.Queries[i];
            if(buffer.Length == 0)
            {
                requestCount++;
                byte[] payloadBytes = QuerierUtils.EncodeCalls(
                    query.Queries.Skip(i),
                    _flashCallExecutor.GetMaxPayloadSize(targetHeight) - QuerierUtils.QuerierCode.Length,
                    out int callCount,
                    out var ethValue
                );

                if(callCount == 0)
                {
                    throw new InvalidOperationException("Call is too large to be executed within batch");
                }

                var callResult = await _flashCallExecutor.ExecuteFlashCallAsync(
                    IContractDeployment.Create(QuerierUtils.QuerierCode, 0),
                    IContractCall.ForRawContractCall(null!, ethValue, payloadBytes),
                    targetHeight,
                    cancellationToken
                );

                var output = callResult.Unwrap(Address.Zero);

                if(output.Length == 0)
                {
                    throw new InvalidOperationException("Call is too expensive to be executed within batch");
                }

                buffer = output.Span;
            }

            int sliceLength = q.ParseResultLength(buffer);
            byte[] sliceData = buffer[0..sliceLength].ToArray();
            outputs[i] = sliceData;
            buffer = buffer[sliceLength..];
        }

        _logger?.LogTrace("Batch query processing completed using {requests} request(s)", requestCount);

        if(requestCount > 1)
        {
            _logger?.LogDebug("Batch query processing too expensive, required {requests} requests", requestCount);
        }

        return query.ReadResultFrom(outputs);
    }
}
