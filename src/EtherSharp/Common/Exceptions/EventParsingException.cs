using EtherSharp.Types;

namespace EtherSharp.Common.Exceptions;

public class EventParsingException(Log log, string reason)
    : Exception($"Failed parsing event {log.TransactionHash}:{log.LogIndex}: {reason}")
{
    public Log Log { get; } = log;
    public string Reason { get; } = reason;
}