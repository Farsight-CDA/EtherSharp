using EtherSharp.Types;

namespace EtherSharp.Realtime.Events;

/// <summary>
/// Defines a strongly-typed transaction log that can decode itself from a raw log payload.
/// </summary>
/// <typeparam name="TSelf">The concrete log type.</typeparam>
public interface ITxLog<TSelf> : ITxLog
{
    public abstract static TSelf Decode(Log log);
}

/// <summary>
/// Defines the common contract for decoded transaction logs.
/// </summary>
public interface ITxLog
{
    public Log Event { get; }
}
