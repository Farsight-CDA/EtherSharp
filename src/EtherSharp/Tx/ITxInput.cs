using EtherSharp.Types;
using System.Numerics;

namespace EtherSharp.Tx;
public interface ITxInput
{
    public Address To { get; }
    public BigInteger Value { get; }

    internal int DataLength { get; }
    internal void WriteDataTo(Span<byte> destination);
}
