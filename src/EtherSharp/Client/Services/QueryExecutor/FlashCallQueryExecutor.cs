using EtherSharp.Client.Services.FlashCallExecutor;
using EtherSharp.Numerics;
using EtherSharp.Query;
using EtherSharp.Tx;
using EtherSharp.Types;

namespace EtherSharp.Client.Services.QueryExecutor;

internal sealed class FlashCallQueryExecutor(IFlashCallExecutor flashCallExecutor, IServiceProvider provider) : QueryExecutorBase(provider)
{
    private readonly IFlashCallExecutor _flashCallExecutor = flashCallExecutor;

    protected override Address? CallAddress => null;
    protected override void PreparePlan(QueryPlan plan, QuerierByteCode querier) { }

    protected override int GetMaxPayloadSize(QuerierByteCode querier, ulong? gasLimit, TargetHeight targetHeight)
        => _flashCallExecutor.GetMaxPayloadSize(gasLimit, targetHeight) - querier.Deployment.ByteCode.Length;

    protected override int GetMaxResultSize(TargetHeight targetHeight)
        => _flashCallExecutor.GetMaxResultSize(targetHeight);

    protected override Task<TxCallResult> ExecuteBatchAsync(
        QuerierByteCode querier,
        UInt256 value,
        ReadOnlyMemory<byte> payload,
        ulong? gasLimit,
        CallOptions options,
        CancellationToken cancellationToken
    )
        => _flashCallExecutor.ExecuteFlashCallAsync(
            querier.Deployment,
            IFlashCall.ForRawFlashCall(value, payload),
            gasLimit,
            options,
            cancellationToken);
}
