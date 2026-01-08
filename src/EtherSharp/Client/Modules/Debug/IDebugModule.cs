using EtherSharp.Types;

namespace EtherSharp.Client.Modules.Debug;

public interface IDebugModule
{
    public Task<CallTrace> TraceTransactionCallsAsync(string transactionHash, CancellationToken cancellationToken = default);
}
