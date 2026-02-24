using EtherSharp.Types;

namespace EtherSharp.Realtime.Events;

/// <summary>
/// Defines a strongly-typed transaction log that can decode itself from a raw log payload.
/// </summary>
/// <typeparam name="TSelf">The concrete log type.</typeparam>
public interface ITxLog<TSelf> : ITxLog
{
    /// <summary>
    /// Decodes a typed log instance from a raw transaction log payload.
    /// </summary>
    /// <param name="log">The raw log payload.</param>
    /// <returns>The decoded typed log instance.</returns>
    public abstract static TSelf Decode(Log log);
}

/// <summary>
/// Defines the common contract for decoded transaction logs.
/// </summary>
public interface ITxLog
{
    /// <summary>
    /// Gets the original raw log payload backing this decoded event.
    /// </summary>
    public Log Event { get; }
}
