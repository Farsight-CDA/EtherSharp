using EtherSharp.Types;

namespace EtherSharp.Client;

/// <summary>
/// Configures the execution context and tracing behavior for a simulated call.
/// </summary>
public readonly record struct TraceCallOptions
{
    /// <summary>
    /// Gets the block context against which the call executes.
    /// </summary>
    public TargetHeight TargetHeight { get; init; }

    /// <summary>
    /// Gets the optional sender address.
    /// </summary>
    public Address? From { get; init; }

    /// <summary>
    /// Gets the optional account state overrides.
    /// </summary>
    public IReadOnlyDictionary<Address, AccountOverride>? StateOverrides { get; init; }

    /// <summary>
    /// Gets the optional block context overrides.
    /// </summary>
    public BlockOverride? BlockOverrides { get; init; }

    /// <summary>
    /// Gets the maximum number of milliseconds the node may spend tracing the call.
    /// </summary>
    public uint? TimeoutMilliseconds { get; init; }

    /// <summary>
    /// Gets the transaction index whose resulting intra-block state is used.
    /// </summary>
    public uint? TransactionIndex { get; init; }

    /// <summary>
    /// Creates trace-call options from normal call options.
    /// </summary>
    public static implicit operator TraceCallOptions(CallOptions options)
        => new()
        {
            TargetHeight = options.TargetHeight,
            From = options.From,
            StateOverrides = options.StateOverrides,
            BlockOverrides = options.BlockOverrides
        };

    /// <summary>
    /// Creates trace-call options targeting a specific block context.
    /// </summary>
    public static implicit operator TraceCallOptions(TargetHeight targetHeight)
        => new() { TargetHeight = targetHeight };
}
