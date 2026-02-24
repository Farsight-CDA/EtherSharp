using EtherSharp.Types;

namespace EtherSharp.Common.Comparer;

/// <summary>
/// Compares logs by block number and then by log index.
/// </summary>
public class EventComparer : IComparer<Log>
{
    /// <summary>
    /// Shared static instance of <see cref="EventComparer"/>.
    /// </summary>
    public static EventComparer Instance { get; } = new EventComparer();

    /// <inheritdoc/>
    public int Compare(Log? x, Log? y)
    {
        ArgumentNullException.ThrowIfNull(x, nameof(x));
        ArgumentNullException.ThrowIfNull(y, nameof(y));

        if(x.BlockNumber > y.BlockNumber)
        {
            return 1;
        }
        else if(x.BlockNumber < y.BlockNumber)
        {
            return -1;
        }

        if(x.LogIndex > y.LogIndex)
        {
            return 1;
        }
        else if(x.LogIndex < y.LogIndex)
        {
            return -1;
        }

        return 0;
    }
}
