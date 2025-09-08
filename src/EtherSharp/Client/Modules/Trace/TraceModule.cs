using EtherSharp.Tx;
using EtherSharp.Types;

namespace EtherSharp.Client.Modules.Trace;
internal class TraceModule : ITraceModule
{
    public Task TraceCallStateDiffAsync(ITxInput call, TargetBlockNumber targetBlockNumber = default, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task TraceCallTracesAsync(ITxInput call, TargetBlockNumber targetBlockNumber = default, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task TraceCallVmTraceAsync(ITxInput call, TargetBlockNumber targetBlockNumber = default, CancellationToken cancellationToken = default) => throw new NotImplementedException();
}
