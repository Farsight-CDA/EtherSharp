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
    /// Creates an interpreter fork using the target block's post-state and execution context.
    /// </summary>
    /// <remarks>
    /// Requires WithFlashCalls(enableStateOverrides: true). Named targets resolve once using
    /// blockHeightQuery or EVM NUMBER; numeric targets take precedence. Pending is unsupported.
    /// Keep the client alive for state reads. Cancellation applies only to creation; numeric pinning is not reorg-safe.
    /// </remarks>
    public static Task<InterpreterStateFork> ForkPostBlockAsync(
        this IEtherClient client,
        TargetHeight targetHeight,
        IQuery<ulong>? blockHeightQuery = null,
        RpcRequestOptions requestOptions = default,
        CancellationToken cancellationToken = default
    ) => ForkBlockAsync(client, targetHeight, false, blockHeightQuery, requestOptions, cancellationToken);

    /// <summary>
    /// Creates an interpreter fork using the parent block's post-state and the target block's execution context.
    /// </summary>
    /// <remarks>
    /// Uses ForkPostBlockAsync's resolution and lifetime rules, but pins state to the parent height.
    /// Rejects genesis and pending. Does not apply block-start transitions or replay transactions.
    /// </remarks>
    public static Task<InterpreterStateFork> ForkPreBlockAsync(
        this IEtherClient client,
        TargetHeight targetHeight,
        IQuery<ulong>? blockHeightQuery = null,
        RpcRequestOptions requestOptions = default,
        CancellationToken cancellationToken = default
    ) => ForkBlockAsync(client, targetHeight, true, blockHeightQuery, requestOptions, cancellationToken);

    private static async Task<InterpreterStateFork> ForkBlockAsync(
        IEtherClient client,
        TargetHeight targetHeight,
        bool useParentState,
        IQuery<ulong>? blockHeightQuery,
        RpcRequestOptions requestOptions,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(client);
        if(targetHeight == TargetHeight.Pending)
        {
            throw new ArgumentException("A fork requires a mined block, not the pending block.", nameof(targetHeight));
        }
        if(useParentState && targetHeight.Value == 0)
        {
            throw new ArgumentException("Genesis has no parent state to fork.", nameof(targetHeight));
        }

        var (context, height) = await client.QueryAsync(
            IQuery.InterpreterContext(),
            blockHeightQuery ?? IQuery.GetBlockNumber(),
            options: new CallOptions { TargetHeight = targetHeight },
            requestOptions: requestOptions,
            cancellationToken: cancellationToken
        );

        ulong stateHeight = targetHeight.Value ?? height;
        if(useParentState)
        {
            if(stateHeight == 0)
            {
                throw new ArgumentException("Genesis has no parent state to fork.", nameof(targetHeight));
            }
            stateHeight--;
        }

        return new InterpreterStateFork(
            InterpreterDataProviderFactory.Create(
                client,
                TargetHeight.Height(stateHeight),
                requestOptions
            ),
            context
        );
    }

    /// <summary>
    /// Creates an interpreter fork at the requested block using the supplied state provider.
    /// </summary>
    /// <remarks>
    /// The target selects context only; the provider controls state pinning. Pending is unsupported.
    /// The fork borrows the client and provider; keep them alive for state reads.
    /// </remarks>
    public static async Task<InterpreterStateFork> ForkWithProviderAsync(
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
