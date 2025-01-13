using EtherSharp.Types;
using System.Numerics;

namespace EtherSharp.Tx;
public interface ITxInput
{
    public Address To { get; }
    public BigInteger Value { get; }

    public int DataLength { get; }
    public void WriteDataTo(Span<byte> destination);
}
