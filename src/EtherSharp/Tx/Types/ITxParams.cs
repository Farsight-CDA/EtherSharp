namespace EtherSharp.Tx.Types;
public interface ITxParams<TSelf>
{
    /// <summary>
    /// The default TxParams if none are provided by the caller.
    /// </summary>
    public abstract static TSelf Default { get; }
}