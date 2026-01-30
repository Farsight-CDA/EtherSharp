namespace EtherSharp.Contract.Sections;

/// <summary>
/// Interface for the logs section of a generated contract schema.
/// </summary>
public interface ILogsSection
{
    /// <summary>
    /// Gets all log topics defined in the contract.
    /// </summary>
    /// <returns></returns>
    public abstract static ReadOnlyMemory<byte>[] GetTopics();
}
