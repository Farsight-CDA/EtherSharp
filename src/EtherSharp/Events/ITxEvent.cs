using EtherSharp.Types;

namespace EtherSharp.Events;
public interface ITxEvent<TSelf>
{
    public abstract static TSelf Decode(Log data);
}
