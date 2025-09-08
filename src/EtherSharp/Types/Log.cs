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
) : ITxLog<Log>
{
    Log ITxLog.Event => this;

    public static Log Decode(Log log) => log;
}