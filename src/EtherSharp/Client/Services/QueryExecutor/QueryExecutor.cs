using EtherSharp.Client.Services.FlashCall;
using EtherSharp.Common.Exceptions;
using EtherSharp.Numerics;
using EtherSharp.Query;
using EtherSharp.RPC.Transport;
using EtherSharp.Tx;
using EtherSharp.Types;
using System.Buffers;

namespace EtherSharp.Client.Services.QueryExecutor;

internal sealed class QueryExecutor(FlashCallExecutor flashCallExecutor)
{
    private readonly FlashCallExecutor _flashCallExecutor = flashCallExecutor;

    public async Task<TQuery> ExecuteQueryAsync<TQuery>(
        IQuery<TQuery> query,
        ulong? gasLimit,
        CallOptions options,
        RpcRequestOptions requestOptions,
        CancellationToken cancellationToken)
    {
        if(query.OperationCount == 0)
        {
            return query.ReadResultFrom([]);
        }

        var plan = new QueryPlan(query.OperationCount, options.StateOverrides);
        plan.Add(query);

        var outputs = new ReadOnlyMemory<byte>[plan.Queries.Count];
        byte[] payloadBytes = IQuerier.Functions.Query.Encode(
            plan.Queries,
            out int payloadSize,
            out var ethValue
        );

        try
        {
            var runtime = QuerierCodeBuilder.CreateRuntime(
                Enum.GetValues<QueryOperationId>()
                    .Where(operationId => operationId != QueryOperationId.PartialRange)
                    .ToArray()
            );

            if(plan.RequiresQuerierStateOverride)
            {
                plan.AddStateOverride(
                    IQuerier.StateOverride.Address,
                    new AccountOverride(balance: UInt256.Zero, nonce: 1, code: runtime.ByteCode)
                );
            }

            var callResult = await _flashCallExecutor.ExecuteFlashCallAsync(
                IFlashCode.FromRuntimeCode(runtime),
                IFlashCall.ForRawFlashCall(ethValue, payloadBytes.AsMemory(0, payloadSize)),
                gasLimit,
                options with { StateOverrides = plan.StateOverrides },
                requestOptions,
                cancellationToken
            );

            if(!callResult.Success)
            {
                throw CallRevertedException.Parse(null, callResult.Data.Span);
            }

            var output = callResult.Data;
            var buffer = output;

            for(int i = 0; i < plan.Queries.Count; i++)
            {
                var operation = plan.Queries[i];
                int sliceLength = operation.ParseResultLength(buffer.Span);
                outputs[i] = buffer[..sliceLength];
                buffer = buffer[sliceLength..];
            }

            return buffer.Length > 0
                ? throw new CallParsingException.RemainingReturnDataException(
                    output,
                    output.Length - buffer.Length
                )
                : query.ReadResultFrom(outputs);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(payloadBytes);
        }
    }
}
