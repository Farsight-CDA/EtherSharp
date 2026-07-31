using EtherSharp.Types;

namespace EtherSharp.Client;

/// <summary>
/// Configures the execution context for an EVM call.
/// </summary>
public readonly record struct CallOptions
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
    /// Creates call options targeting a specific block context.
    /// </summary>
    /// <param name="targetHeight">Target block context.</param>
    public static implicit operator CallOptions(TargetHeight targetHeight)
        => new() { TargetHeight = targetHeight };
}
