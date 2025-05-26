using EtherSharp.Realtime.Events;

namespace EtherSharp.Types;
public record Log(
    Address Address,
    string BlockHash,
    uint BlockNumber,
    uint LogIndex,
    string TransactionHash,
    uint TransactionIndex,
    byte[][] Topics,
    byte[] Data,
    bool Removed
) : ITxEvent<Log>
{
    public static Log Decode(Log data) => data;
}
