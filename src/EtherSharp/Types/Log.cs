using EtherSharp.Events;
using System.Numerics;

namespace EtherSharp.Types;
public record Log(
    string Address,
    string BlockHash,
    uint BlockNumber,
    uint LogIndex,
    string TransactionHash,
    uint TransactionIndex,
    byte[][] Topics,
    byte[] Data
) : ITxEvent<Log>
{
    public static Log Decode(Log data) => data;
}
