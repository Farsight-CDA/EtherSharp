namespace EtherSharp.Types;

internal static class FilterNormalization
{
    public static T[] Normalize<T>(T[] values)
        where T : IComparable<T>, IEquatable<T>
    {
        if(values.Length < 2)
        {
            return values;
        }

        Array.Sort(values);

        int uniqueCount = 1;
        for(int i = 1; i < values.Length; i++)
        {
            if(values[i].Equals(values[uniqueCount - 1]))
            {
                continue;
            }

            values[uniqueCount++] = values[i];
        }

        if(uniqueCount != values.Length)
        {
            Array.Resize(ref values, uniqueCount);
        }

        return values;
    }
}
