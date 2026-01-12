using EtherSharp.Client.Services.FlashCallExecutor;
using EtherSharp.Query;
using EtherSharp.Tx;
using EtherSharp.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Buffers;

namespace EtherSharp.Client.Services.QueryExecutor;

internal class FlashCallQueryExecutor(IFlashCallExecutor flashCallExecutor, IServiceProvider provider) : IQueryExecutor, IInitializableService
{
    private readonly IFlashCallExecutor _flashCallExecutor = flashCallExecutor;
    private readonly ILogger? _logger = provider.GetService<ILoggerFactory>()?.CreateLogger<FlashCallQueryExecutor>();

    private bool _supportsCancun = false;
    private readonly IContractDeployment _londonDeployment = IContractDeployment.Create(QuerierUtils.LondonQuerierCode, 0);
    private readonly IContractDeployment _cancunDeployment = IContractDeployment.Create(QuerierUtils.CancunQuerierCode, 0);

    public ValueTask InitializeAsync(ulong chainId, CompatibilityReport compatibilityReport, CancellationToken cancellationToken = default)
    {
        if(compatibilityReport.SupportsPush0)
        {
            _supportsCancun = true;
        }

        return ValueTask.CompletedTask;
    }

    public async Task<TQuery> ExecuteQueryAsync<TQuery>(IQuery<TQuery> query, TargetBlockNumber targetHeight, CancellationToken cancellationToken)
    {
        ReadOnlySpan<byte> buffer = [];
        byte[][] outputs = new byte[query.Queries.Count][];
        int requestCount = 0;

        var querierDeployment = !_supportsCancun || targetHeight.Value != 0 || targetHeight == TargetBlockNumber.Earliest
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

                    buffer = output.Span;
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(payloadBytes);
                }
            }

            int sliceLength = q.ParseResultLength(buffer);
            //ToDo: use array pool or figure out how we can use Spans here
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
