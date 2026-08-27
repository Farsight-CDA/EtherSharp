namespace EtherSharp.Types;

/// <summary>
/// Represents the emitting-address and topic conditions of an EVM log filter.
/// </summary>
public readonly struct EventFilter : IEquatable<EventFilter>
{
    /// <summary>
    /// Gets a filter that matches every EVM log.
    /// </summary>
    public static EventFilter Any => default;

    /// <summary>
    /// Gets the allowed emitting addresses, or empty memory when addresses are unconstrained.
    /// </summary>
    public ReadOnlyMemory<Address> Addresses { get; }

    /// <summary>
    /// Gets the topic conditions.
    /// </summary>
    public EventTopics Topics { get; }

    /// <summary>
    /// Creates an EVM log filter.
    /// </summary>
    /// <param name="addresses">Allowed emitting addresses, or <see langword="null"/> to match any address.</param>
    /// <param name="topics">Topic conditions.</param>
    public EventFilter(Address[]? addresses = null, EventTopics topics = default)
    {
        if(addresses is [])
        {
            throw new ArgumentException("An address filter must contain at least one address.", nameof(addresses));
        }

        Addresses = addresses is null
            ? default
            : FilterNormalization.Normalize([.. addresses]);
        Topics = topics;
    }

    private EventFilter(ReadOnlyMemory<Address> addresses, EventTopics topics)
    {
        Addresses = addresses;
        Topics = topics;
    }

    internal static EventFilter FromOwned(ReadOnlyMemory<Address> addresses, EventTopics topics)
        => new(addresses, topics);

    /// <inheritdoc/>
    public bool Equals(EventFilter other)
        => Topics == other.Topics
            && Addresses.Span.SequenceEqual(other.Addresses.Span);

    /// <inheritdoc/>
    public override bool Equals(object? obj)
        => obj is EventFilter other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(Topics);
        foreach(var address in Addresses.Span)
        {
            hashCode.Add(address);
        }

        return hashCode.ToHashCode();
    }

    /// <inheritdoc/>
    public static bool operator ==(in EventFilter left, in EventFilter right)
        => left.Equals(right);

    /// <inheritdoc/>
    public static bool operator !=(in EventFilter left, in EventFilter right)
        => !left.Equals(right);
}
