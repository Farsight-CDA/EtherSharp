using EtherSharp.Types;

namespace EtherSharp.Realtime.Events;

public interface ITxLog<TSelf> : ITxLog
{
    public abstract static TSelf Decode(Log log);
}

public interface ITxLog
{
    public Log Event { get; }
}