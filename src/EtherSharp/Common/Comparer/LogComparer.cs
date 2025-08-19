using EtherSharp.Types;

namespace EtherSharp.Common.Comparer;
public class LogComparer : IComparer<Log>
{
    public static LogComparer Instance { get; } = new LogComparer();

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
