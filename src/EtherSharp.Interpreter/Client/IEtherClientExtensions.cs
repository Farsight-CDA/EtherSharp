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
    /// Creates an interpreter fork using a data provider compatible with the client's query backend.
    /// </summary>
    /// <param name="client">The client whose query backend will fetch upstream state.</param>
    /// <param name="targetHeight">The numeric or named block target to fork.</param>
    /// <param name="blockHeightQuery">
    /// Optional query for the RPC block height on chains where it differs from EVM NUMBER.
    /// Runs alongside the context query, defaulting to EVM NUMBER. Numeric targets remain authoritative
    /// for subsequent provider reads regardless of the query's result.
    /// </param>
    /// <param name="requestOptions">Options applied to context and subsequent state reads.</param>
    /// <param name="cancellationToken">Token used to cancel fork creation, not subsequent interpreter operations.</param>
    /// <returns>A fork with its own provider and nonce-probe history.</returns>
    /// <exception cref="InvalidOperationException">No provider is available for the configured query backend.</exception>
    /// <remarks>
    /// WithFlashCalls(enableStateOverrides: true) automatically enables the built-in provider.
    /// Named targets use the context's EVM block number unless blockHeightQuery is supplied.
    /// Context and optional height are fetched together; subsequent provider reads use the resolved numeric height.
    /// The client must remain alive while the fork is fetching state. Numeric pinning does not protect
    /// against reorganizations; use a suitably finalized block where consistency is required.
    /// </remarks>
    public static async Task<InterpreterStateFork> ForkAsync(
        this IEtherClient client,
        TargetHeight targetHeight,
        IQuery<ulong>? blockHeightQuery = null,
        RpcRequestOptions requestOptions = default,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(client);
        if(targetHeight == TargetHeight.Pending)
        {
            throw new ArgumentException("A fork requires a mined block, not the pending block.", nameof(targetHeight));
        }

        var (context, height) = await client.QueryAsync(
            IQuery.InterpreterContext(),
            blockHeightQuery ?? IQuery.GetBlockNumber(),
            options: new CallOptions { TargetHeight = targetHeight },
            requestOptions: requestOptions,
            cancellationToken: cancellationToken
        );

        return new InterpreterStateFork(
            InterpreterDataProviderFactory.Create(
                client,
                targetHeight.IsNumeric
                    ? targetHeight
                    : TargetHeight.Height(height),
                requestOptions
            ),
            context
        );
    }

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
