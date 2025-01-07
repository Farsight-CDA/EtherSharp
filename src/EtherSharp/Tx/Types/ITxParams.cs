namespace EtherSharp.Tx.Types;
public interface ITxParams<TSelf>
{
    public abstract static TSelf Default { get; }
}