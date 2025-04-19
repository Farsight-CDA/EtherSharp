namespace EtherSharp.Tx.Types;
public interface ITxGasParams<TSelf> : ITxGasParams
    where TSelf : ITxGasParams<TSelf>
{
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

public interface ITxGasParams;