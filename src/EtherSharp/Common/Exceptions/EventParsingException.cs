using EtherSharp.Types;

namespace EtherSharp.Common.Exceptions;

/// <summary>
/// Represents a failure while decoding a log into a typed event model.
/// </summary>
/// <param name="log">Raw log that failed to decode.</param>
/// <param name="reason">Human-readable reason explaining why decoding failed.</param>
public class EventParsingException(Log log, string reason)
    : Exception($"Failed parsing event {log.TransactionHash}:{log.LogIndex}: {reason}")
{
    /// <summary>
    /// Raw log payload that could not be decoded.
    /// </summary>
    public Log Log { get; } = log;

    /// <summary>
    /// Human-readable decode failure reason.
    /// </summary>
    public string Reason { get; } = reason;
}
