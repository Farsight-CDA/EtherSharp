using EtherSharp.Common.Instrumentation;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace EtherSharp.Common.Extensions;

internal static class IServiceProviderExtensions
{
    private static TagList MergeTags(TagList list1, TagList list2)
    {
        if(list1.Count == 0)
        {
            return list2;
        }

        if(list2.Count == 0)
        {
            return list1;
        }
        //
        var merged = list1;
        foreach(var tag in list2)
        {
            merged.Add(tag);
        }
        return merged;
    }

    internal static OTELCounter<T>? CreateOTELCounter<T>(this IServiceProvider provider,
        string name, string? unit = null, string? description = null, TagList tags = default)
        where T : struct
    {
        var options = provider.GetService<InstrumentationOptions>();
        var meterFactory = provider.GetService<IMeterFactory>();

        if(options is null || meterFactory is null)
        {
            return null;
        }
        //
        var mergedTags = MergeTags(options.Tags, tags);
        var counter = meterFactory.Create(Diagnostics.METER_NAME)
            .CreateCounter<T>($"{options.InstrumentNamePrefix}{name}", unit, description);
        return new OTELCounter<T>(counter, mergedTags);
    }

    internal static OTELUpDownCounter<T>? CreateOTELUpDownCounter<T>(this IServiceProvider provider,
        string name, string? unit = null, string? description = null, TagList tags = default)
        where T : struct
    {
        var options = provider.GetService<InstrumentationOptions>();
        var meterFactory = provider.GetService<IMeterFactory>();

        if(options is null || meterFactory is null)
        {
            return null;
        }
        //
        var mergedTags = MergeTags(options.Tags, tags);
        var counter = meterFactory.Create(Diagnostics.METER_NAME)
            .CreateUpDownCounter<T>($"{options.InstrumentNamePrefix}{name}", unit, description);
        return new OTELUpDownCounter<T>(counter, mergedTags);
    }

    internal static ObservableUpDownCounter<T>? CreateOTELObservableUpDownCounter<T>(this IServiceProvider provider,
        string name, Func<T> observeValue, string? unit = null, string? description = null, TagList tags = default)
        where T : struct
    {
        var options = provider.GetService<InstrumentationOptions>();
        var meterFactory = provider.GetService<IMeterFactory>();

        if(options is null || meterFactory is null)
        {
            return null;
        }
        //
        var mergedTags = MergeTags(options.Tags, tags);
        return meterFactory.Create(Diagnostics.METER_NAME)
            .CreateObservableUpDownCounter(
                $"{options.InstrumentNamePrefix}{name}",
                () => new Measurement<T>(observeValue(), mergedTags),
                unit,
                description
            );
    }

    internal static ObservableGauge<T>? CreateOTELObservableGauge<T>(this IServiceProvider provider,
        string name, Func<T> observeValue, string? unit = null, string? description = null, TagList tags = default)
        where T : struct
    {
        var options = provider.GetService<InstrumentationOptions>();
        var meterFactory = provider.GetService<IMeterFactory>();

        if(options is null || meterFactory is null)
        {
            return null;
        }
        //
        var mergedTags = MergeTags(options.Tags, tags);
        return meterFactory.Create(Diagnostics.METER_NAME)
            .CreateObservableGauge(
                $"{options.InstrumentNamePrefix}{name}",
                () => new Measurement<T>(observeValue(), mergedTags),
                unit,
                description
            );
    }
}
