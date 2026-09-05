using EtherSharp.Client;
using EtherSharp.Interpreter.Forking;
using EtherSharp.Interpreter.Query;
using EtherSharp.Query;
using EtherSharp.RPC.Transport;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Client;

/// <summary>
/// Creates interpreter forks using a client's upstream block context.
/// </summary>
public static class IEtherClientExtensions
{
    /// <summary>
    /// Creates an interpreter fork at the requested block using the supplied state provider.
    /// </summary>
    /// <param name="client">The client used to fetch the block context.</param>
    /// <param name="targetHeight">The numeric or named block target passed to the context query.</param>
    /// <param name="dataProvider">The provider responsible for resolving state at the fork's context.</param>
    /// <param name="requestOptions">Options for the context-fetching RPC requests.</param>
    /// <param name="cancellationToken">Token used to cancel fork creation.</param>
    /// <returns>An independent upstream state fork.</returns>
    /// <remarks>
    /// Pending blocks are not supported. The provider is responsible for pinning subsequent state reads
    /// to the captured context. The fork does not own the client or provider;
    /// any client used by the provider must remain alive while the fork is fetching state.
    /// </remarks>
    public static async Task<InterpreterStateFork> ForkAsync(
        this IEtherClient client,
        TargetHeight targetHeight,
        IInterpreterDataProvider dataProvider,
        RpcRequestOptions requestOptions = default,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(dataProvider);
        if(targetHeight == TargetHeight.Pending)
        {
            throw new ArgumentException("A fork requires a mined block, not the pending block.", nameof(targetHeight));
        }

        var context = await client.QueryAsync(
            IQuery.InterpreterContext(),
            options: new CallOptions { TargetHeight = targetHeight },
            requestOptions: requestOptions,
            cancellationToken: cancellationToken
        );
        return new InterpreterStateFork(dataProvider, context);
    }
}
