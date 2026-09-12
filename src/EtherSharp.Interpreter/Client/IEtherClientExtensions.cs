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
    /// <param name="client">The client supplying the upstream block context and state backend.</param>
    /// <param name="targetHeight">The target block. Pending is unsupported.</param>
    /// <param name="options">Fork configuration. Initial state must describe the target block's post-state.</param>
    /// <param name="dataProviderOptions">Configuration for creating the upstream data provider.</param>
    /// <param name="requestOptions">Options applied to upstream requests.</param>
    /// <param name="cancellationToken">Cancellation for fork creation only.</param>
    /// <remarks>
    /// Requires WithFlashCalls(enableStateOverrides: true). Named targets resolve once using
    /// <see cref="InterpreterForkOptions.BlockHeightQuery"/> or EVM NUMBER; numeric targets take precedence. Pending is unsupported.
    /// Keep the client alive for state reads. Cancellation applies only to creation; numeric pinning is not reorg-safe.
    /// </remarks>
    public static Task<InterpreterStateFork> ForkPostBlockAsync(
        this IEtherClient client,
        TargetHeight targetHeight,
        InterpreterForkOptions options = default,
        InterpreterDataProviderOptions dataProviderOptions = default,
        RpcRequestOptions requestOptions = default,
        CancellationToken cancellationToken = default
    ) => ForkBlockAsync(
        client, targetHeight, useParentState: false, options, dataProviderOptions, requestOptions, cancellationToken
    );

    /// <summary>
    /// Creates an interpreter fork using the parent block's post-state and the target block's execution context.
    /// </summary>
    /// <param name="client">The client supplying the upstream block context and state backend.</param>
    /// <param name="targetHeight">The target block. Rejects genesis and pending.</param>
    /// <param name="options">Fork configuration. Initial state must describe the parent block's post-state.</param>
    /// <param name="dataProviderOptions">Configuration for creating the upstream data provider.</param>
    /// <param name="requestOptions">Options applied to upstream requests.</param>
    /// <param name="cancellationToken">Cancellation for fork creation only.</param>
    /// <remarks>
    /// Uses <see cref="ForkPostBlockAsync"/>'s resolution and lifetime rules.
    /// Rejects genesis and pending. Does not apply block-start transitions or replay transactions.
    /// </remarks>
    public static Task<InterpreterStateFork> ForkPreBlockAsync(
        this IEtherClient client,
        TargetHeight targetHeight,
        InterpreterForkOptions options = default,
        InterpreterDataProviderOptions dataProviderOptions = default,
        RpcRequestOptions requestOptions = default,
        CancellationToken cancellationToken = default
    ) => ForkBlockAsync(
        client, targetHeight, useParentState: true, options, dataProviderOptions, requestOptions, cancellationToken
    );

    private static async Task<InterpreterStateFork> ForkBlockAsync(
        IEtherClient client,
        TargetHeight targetHeight,
        bool useParentState,
        InterpreterForkOptions options,
        InterpreterDataProviderOptions dataProviderOptions,
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
            IQuery.InterpreterContext(disableBlobBaseFee: options.DisableBlobBaseFee),
            options.BlockHeightQuery ?? IQuery.GetBlockNumber(),
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
                dataProviderOptions,
                requestOptions
            ),
            context,
            options
        );
    }

    /// <summary>
    /// Creates an interpreter fork at the requested block using the supplied state provider.
    /// </summary>
    /// <param name="client">The client supplying the execution context.</param>
    /// <param name="targetHeight">The target block. Pending is unsupported.</param>
    /// <param name="dataProvider">The provider that controls state pinning.</param>
    /// <param name="options">Fork configuration. Initial state must describe the provider's state snapshot.</param>
    /// <param name="requestOptions">Options applied to upstream requests.</param>
    /// <param name="cancellationToken">Cancellation for context creation only.</param>
    /// <remarks>
    /// The target selects context only; the provider controls state pinning. Pending is unsupported.
    /// The fork borrows the client and provider; keep them alive for state reads.
    /// </remarks>
    public static async Task<InterpreterStateFork> ForkWithProviderAsync(
        this IEtherClient client,
        TargetHeight targetHeight,
        IInterpreterDataProvider dataProvider,
        InterpreterForkOptions options = default,
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
            IQuery.InterpreterContext(disableBlobBaseFee: options.DisableBlobBaseFee),
            options: new CallOptions { TargetHeight = targetHeight },
            requestOptions: requestOptions,
            cancellationToken: cancellationToken
        );
        return new InterpreterStateFork(dataProvider, context, options);
    }
}
