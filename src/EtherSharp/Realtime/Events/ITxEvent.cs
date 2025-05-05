using EtherSharp.Types;

namespace EtherSharp.Realtime.Events;
public interface ITxEvent<TSelf>
{
    public abstract static TSelf Decode(Log data);
}
