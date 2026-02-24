using EtherSharp.Client.Services.FlashCallExecutor;
using EtherSharp.Query;
using EtherSharp.Tx;
using EtherSharp.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Buffers;

namespace EtherSharp.Client.Services.QueryExecutor;

internal class FlashCallQueryExecutor(IFlashCallExecutor flashCallExecutor, IServiceProvider provider) : IQueryExecutor
{
    private readonly IFlashCallExecutor _flashCallExecutor = flashCallExecutor;
    private readonly ILogger? _logger = provider.GetService<ILoggerFactory>()?.CreateLogger<FlashCallQueryExecutor>();
    private readonly IEtherClient _client = provider.GetRequiredService<IEtherClient>();

    private readonly IContractDeployment _londonDeployment = IContractDeployment.Create(QuerierUtils.LondonQuerierCode, 0);
    private readonly IContractDeployment _cancunDeployment = IContractDeployment.Create(QuerierUtils.CancunQuerierCode, 0);

    public async Task<TQuery> ExecuteQueryAsync<TQuery>(IQuery<TQuery> query, TargetHeight targetHeight, CancellationToken cancellationToken)
    {
        var buffer = ReadOnlyMemory<byte>.Empty;
        var outputs = new ReadOnlyMemory<byte>[query.Queries.Count];
        int requestCount = 0;

        bool supportsCancun = _client.IsInitialized && _client.CompatibilityReport is not null && _client.CompatibilityReport.SupportsPush0;
        var querierDeployment = !supportsCancun || targetHeight.Value != 0 || targetHeight == TargetHeight.Earliest
            ? _londonDeployment
            : _cancunDeployment;

        for(int i = 0; i < query.Queries.Count; i++)
        {
            var q = query.Queries[i];
            if(buffer.Length == 0)
            {
                requestCount++;

                byte[] payloadBytes = QuerierUtils.EncodeCalls(
                    querierDeployment.ByteCode,
                    query.Queries.Skip(i),
                    _flashCallExecutor.GetMaxPayloadSize(targetHeight) - querierDeployment.ByteCode.Length,
                    _flashCallExecutor.GetMaxResultSize(targetHeight),
                    out int payloadSize,
                    out int callCount,
                    out var ethValue
                );

                if(callCount == 0)
                {
                    throw new InvalidOperationException("Call is too large to be executed within batch");
                }

                try
                {
                    var callResult = await _flashCallExecutor.ExecuteFlashCallAsync(
                        querierDeployment,
                        IContractCall.ForRawContractCall(null!, ethValue, payloadBytes.AsMemory(0, payloadSize)),
                        targetHeight,
                        cancellationToken
                    );

                    var output = callResult.Unwrap(Address.Zero);

                    if(output.Length == 0)
                    {
                        throw new InvalidOperationException("Call is too expensive to be executed within batch");
                    }

                    buffer = output;
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

        if(_logger?.IsEnabled(LogLevel.Trace) == true)
        {
            _logger.LogTrace("Batch query processing completed using {requests} request(s)", requestCount);
        }

        if(requestCount > 1 && _logger?.IsEnabled(LogLevel.Debug) == true)
        {
            _logger.LogDebug("Batch query processing too expensive, required {requests} requests", requestCount);
        }

        return query.ReadResultFrom(outputs);
    }
}
