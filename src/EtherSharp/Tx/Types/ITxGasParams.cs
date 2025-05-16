using System.Numerics;

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
    /// Returns a new instance of the gas params with fees multiplied by the given multiplier, then divided by the given divider.
    /// </summary>
    /// <param name="multiplier"></param>
    /// <param name="divider"></param>
    /// <param name="minimumIncrement"></param>
    /// <returns></returns>
    public TSelf IncrementByFactor(BigInteger multiplier, BigInteger divider, BigInteger minimumIncrement);

    /// <summary>
    /// Encodes this instance to bytes.
    /// </summary>
    /// <returns></returns>
    public byte[] Encode();
}

public interface ITxGasParams;