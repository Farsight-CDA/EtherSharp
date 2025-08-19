using EtherSharp.Types;

namespace EtherSharp.Realtime.Events;
public interface ITxEvent<TSelf> : ITxEvent
{
    public abstract static TSelf Decode(Log data);
}

public interface ITxEvent
{
    public Log Log { get; }
}