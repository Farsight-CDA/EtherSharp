using EtherSharp.RPC.Modules.Trace;
using EtherSharp.RPC.Modules.Trace.Types;
using EtherSharp.Types;

namespace EtherSharp.Client.Modules.Trace;

internal class TraceModule(ITraceRpcModule traceRpcModule) : ITraceModule
{
    private readonly ITraceRpcModule _traceRpcModule = traceRpcModule;

    public async Task<CallTrace> TraceTransactionCallsAsync(Hash32 transactionHash, CancellationToken cancellationToken = default)
    {
        var result = await _traceRpcModule.ReplayTransactionAsync(transactionHash, ["trace"], cancellationToken);

        if(result.Trace is null || result.Trace.Length == 0)
        {
            throw new InvalidOperationException("No traces returned for transaction");
        }

        var root = result.Trace.FirstOrDefault(x => x.TraceAddress.Length == 0) ?? result.Trace[0];
        return ConvertToCallTrace(root, result.Trace);
    }

    private static CallTrace ConvertToCallTrace(TransactionTrace trace, TransactionTrace[] allTraces)
    {
        var children = allTraces
            .Where(x => x.TraceAddress.Length == trace.TraceAddress.Length + 1 &&
                        x.TraceAddress.AsSpan(0, trace.TraceAddress.Length).SequenceEqual(trace.TraceAddress))
            .OrderBy(x => x.TraceAddress[^1])
            .Select(x => ConvertToCallTrace(x, allTraces))
            .ToArray();

        return new CallTrace(
            From: trace.Action.From,
            To: trace.Action.To,
            Gas: trace.Action.Gas,
            GasUsed: trace.Result?.GasUsed ?? 0,
            Value: trace.Action.Value,
            Input: trace.Action.Input,
            Output: trace.Result?.Output,
            Type: trace.Action.CallType,
            Calls: children.Length > 0 ? children : null,
            Error: trace.Error
        );
    }

    public Task<TransactionTraceResult> ReplayTransactionAsync(Hash32 transactionHash, string[] traceTypes, CancellationToken cancellationToken = default)
        => _traceRpcModule.ReplayTransactionAsync(transactionHash, traceTypes, cancellationToken);
}
