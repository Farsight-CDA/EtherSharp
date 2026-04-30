namespace EtherSharp.Contract.Sections;

/// <summary>
/// Interface for a generated contract log type.
/// </summary>
public interface IGeneratedLog
{
    /// <summary>
    /// Gets the log topic bytes.
    /// </summary>
    /// <returns></returns>
    public abstract static ReadOnlyMemory<byte> TopicBytes { get; }
}
