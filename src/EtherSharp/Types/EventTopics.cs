using EtherSharp.Common.Json.Converters;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace EtherSharp.Types;

/// <summary>
/// Represents the topic conditions of an EVM log filter.
/// </summary>
/// <remarks>
/// Values within one slot use OR semantics, while configured slots use AND semantics.
/// An unconfigured slot matches any topic value.
/// </remarks>
[JsonConverter(typeof(EventTopicsConverter))]
public readonly struct EventTopics : IEquatable<EventTopics>
{
    /// <summary>
    /// Maximum number of topics supported by EVM logs.
    /// </summary>
    public const int MAX_TOPIC_COUNT = 4;

    /// <summary>
    /// Gets topic conditions that match any topics.
    /// </summary>
    public static EventTopics Any => default;

    /// <summary>
    /// Gets the allowed values for topic slot 0, or empty memory when the slot is unconstrained.
    /// </summary>
    public ReadOnlyMemory<Bytes32> Topic0 { get; }

    /// <summary>
    /// Gets the allowed values for topic slot 1, or empty memory when the slot is unconstrained.
    /// </summary>
    public ReadOnlyMemory<Bytes32> Topic1 { get; }

    /// <summary>
    /// Gets the allowed values for topic slot 2, or empty memory when the slot is unconstrained.
    /// </summary>
    public ReadOnlyMemory<Bytes32> Topic2 { get; }

    /// <summary>
    /// Gets the allowed values for topic slot 3, or empty memory when the slot is unconstrained.
    /// </summary>
    public ReadOnlyMemory<Bytes32> Topic3 { get; }

    /// <summary>
    /// Creates topic conditions for up to four EVM log topic slots.
    /// </summary>
    /// <param name="topic0">Allowed values for topic slot 0, or <see langword="null"/> to match any value.</param>
    /// <param name="topic1">Allowed values for topic slot 1, or <see langword="null"/> to match any value.</param>
    /// <param name="topic2">Allowed values for topic slot 2, or <see langword="null"/> to match any value.</param>
    /// <param name="topic3">Allowed values for topic slot 3, or <see langword="null"/> to match any value.</param>
    public EventTopics(
        Bytes32[]? topic0 = null,
        Bytes32[]? topic1 = null,
        Bytes32[]? topic2 = null,
        Bytes32[]? topic3 = null)
    {
        if(topic0 is [])
        {
            throw new ArgumentException("A configured topic slot must contain at least one value.", nameof(topic0));
        }
        if(topic1 is [])
        {
            throw new ArgumentException("A configured topic slot must contain at least one value.", nameof(topic1));
        }
        if(topic2 is [])
        {
            throw new ArgumentException("A configured topic slot must contain at least one value.", nameof(topic2));
        }
        if(topic3 is [])
        {
            throw new ArgumentException("A configured topic slot must contain at least one value.", nameof(topic3));
        }

        Topic0 = topic0 is null
            ? default
            : FilterNormalization.Normalize([.. topic0]);
        Topic1 = topic1 is null
            ? default
            : FilterNormalization.Normalize([.. topic1]);
        Topic2 = topic2 is null
            ? default
            : FilterNormalization.Normalize([.. topic2]);
        Topic3 = topic3 is null
            ? default
            : FilterNormalization.Normalize([.. topic3]);
    }

    private EventTopics(
        ReadOnlyMemory<Bytes32> topic0,
        ReadOnlyMemory<Bytes32> topic1,
        ReadOnlyMemory<Bytes32> topic2,
        ReadOnlyMemory<Bytes32> topic3)
    {
        Topic0 = topic0;
        Topic1 = topic1;
        Topic2 = topic2;
        Topic3 = topic3;
    }

    internal static EventTopics FromOwned(
        ReadOnlyMemory<Bytes32> topic0,
        ReadOnlyMemory<Bytes32> topic1,
        ReadOnlyMemory<Bytes32> topic2,
        ReadOnlyMemory<Bytes32> topic3
    ) => new(topic0, topic1, topic2, topic3);

    /// <summary>
    /// Gets whether no topic slots are constrained.
    /// </summary>
    public bool IsMatchAll
        => Topic0.IsEmpty && Topic1.IsEmpty && Topic2.IsEmpty && Topic3.IsEmpty;

    /// <summary>
    /// Gets the allowed values for a topic slot, or an empty span when the slot is unconfigured.
    /// </summary>
    /// <param name="index">The zero-based topic slot index.</param>
    public ReadOnlySpan<Bytes32> this[int index]
        => index switch
        {
            0 => Topic0.Span,
            1 => Topic1.Span,
            2 => Topic2.Span,
            3 => Topic3.Span,
            _ => throw new ArgumentOutOfRangeException(nameof(index), index, "EVM topic index must be between 0 and 3."),
        };

    /// <inheritdoc/>
    public bool Equals(EventTopics other)
        => Topic0.Span.SequenceEqual(other.Topic0.Span)
            && Topic1.Span.SequenceEqual(other.Topic1.Span)
            && Topic2.Span.SequenceEqual(other.Topic2.Span)
            && Topic3.Span.SequenceEqual(other.Topic3.Span);

    /// <inheritdoc/>
    public override bool Equals(object? obj)
        => obj is EventTopics other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(Topic0.Length);
        hashCode.AddBytes(MemoryMarshal.AsBytes(Topic0.Span));
        hashCode.Add(Topic1.Length);
        hashCode.AddBytes(MemoryMarshal.AsBytes(Topic1.Span));
        hashCode.Add(Topic2.Length);
        hashCode.AddBytes(MemoryMarshal.AsBytes(Topic2.Span));
        hashCode.Add(Topic3.Length);
        hashCode.AddBytes(MemoryMarshal.AsBytes(Topic3.Span));
        return hashCode.ToHashCode();
    }

    /// <inheritdoc/>
    public static bool operator ==(in EventTopics left, in EventTopics right)
        => left.Equals(right);

    /// <inheritdoc/>
    public static bool operator !=(in EventTopics left, in EventTopics right)
        => !left.Equals(right);
}
