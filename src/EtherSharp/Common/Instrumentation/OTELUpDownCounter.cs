using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace EtherSharp.Common.Instrumentation;

internal record OTELUpDownCounter<T>(UpDownCounter<T> Counter, TagList Tags)
    where T : struct
{
    public void Add(T delta)
        => Counter.Add(delta, Tags);

    public void Add(T delta, KeyValuePair<string, object?> tag)
    {
        if(Tags.Count == 0)
        {
            Counter.Add(delta, tag);
        }
        else
        {
            var tags = Tags;
            tags.Add(tag);
            Counter.Add(delta, tags);
        }
    }

    public void Add(T delta, TagList tags)
    {
        if(Tags.Count == 0)
        {
            Counter.Add(delta, tags);
        }
        else
        {
            var merged = Tags;
            foreach(var tag in tags)
            {
                merged.Add(tag);
            }
            Counter.Add(delta, merged);
        }
    }
}
