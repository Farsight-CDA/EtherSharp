using System.Diagnostics.Metrics;

namespace EtherSharp.Common.Instrumentation;
internal record OTELCounter<T>(Counter<T> Counter, InstrumentationOptions Options)
    where T : struct
{
    public void Add(T delta)
        => Counter.Add(delta, Options.Tags);
    public void Add(T delta, KeyValuePair<string, object?> tag)
    {
        if(Options.Tags.Count == 0)
        {
            Counter.Add(delta, tag);
        }
        else
        {
            var tags = Options.Tags;
            tags.Add(tag);
            Counter.Add(delta, tags);
        }
    }
}
