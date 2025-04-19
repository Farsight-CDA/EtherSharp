namespace EtherSharp.Tx.Types;
public interface ITxParams<TSelf> : ITxParams
    where TSelf : ITxParams<TSelf>
{
    /// <summary>
    /// The default TxParams if none are provided by the caller.
    /// </summary>
    public abstract static TSelf Default { get; }

    /// <summary>
    /// Decodes an instance of <typeparamref name="TSelf"/> from the given byte span.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public abstract static TSelf Decode(ReadOnlySpan<byte> data);

    /// <summary>
    /// Encodes this instance to bytes.
    /// </summary>
    /// <returns></returns>
    public byte[] Encode();
}

public interface ITxParams;