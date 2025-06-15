using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.Metrics;

namespace EtherSharp.Common.Extensions;
internal static class IServiceProviderExtensions
{
    public static Counter<T>? CreateOTELCounter<T>(this IServiceProvider provider,
        string name, string? unit = null, string? description = null, IEnumerable<KeyValuePair<string, object?>>? tags = null)
        where T : struct
    {
        var options = provider.GetService<InstrumentationOptions>();
        var meterFactory = provider.GetService<IMeterFactory>();

        if(options is null || meterFactory is null)
        {
            return null;
        }

        tags ??= [];
        if(options.Tags is not null)
        {
            tags = Enumerable.Union(
                tags,
                options.Tags.AsEnumerable()
            );
        }

        return meterFactory.Create(Diagnostics.MeterName)
            .CreateCounter<T>($"{options.InstrumentNamePrefix}{name}", unit, description, tags);
    }

    public static UpDownCounter<T>? CreateOTELUpDownCounter<T>(this IServiceProvider provider,
        string name, string? unit = null, string? description = null, IEnumerable<KeyValuePair<string, object?>>? tags = null)
        where T : struct
    {
        var options = provider.GetService<InstrumentationOptions>();
        var meterFactory = provider.GetService<IMeterFactory>();

        if(options is null || meterFactory is null)
        {
            return null;
        }

        tags ??= [];
        if(options.Tags is not null)
        {
            tags = Enumerable.Union(
                tags,
                options.Tags.AsEnumerable()
            );
        }

        return meterFactory.Create(Diagnostics.MeterName)
            .CreateUpDownCounter<T>($"{options.InstrumentNamePrefix}{name}", unit, description, tags);
    }

    public static ObservableUpDownCounter<T>? CreateOTELObservableUpDownCounter<T>(this IServiceProvider provider,
        string name, Func<T> observeValue, string? unit = null, string? description = null, IEnumerable<KeyValuePair<string, object?>>? tags = null)
        where T : struct
    {
        var options = provider.GetService<InstrumentationOptions>();
        var meterFactory = provider.GetService<IMeterFactory>();

        if(options is null || meterFactory is null)
        {
            return null;
        }

        tags ??= [];
        if(options.Tags is not null)
        {
            tags = Enumerable.Union(
                tags,
                options.Tags.AsEnumerable()
            );
        }

        return meterFactory.Create(Diagnostics.MeterName)
            .CreateObservableUpDownCounter($"{options.InstrumentNamePrefix}{name}", observeValue, unit, description, tags);
    }

    public static ObservableGauge<T>? CreateOTELObservableGauge<T>(this IServiceProvider provider,
        string name, Func<T> observeValue, string? unit = null, string? description = null, IEnumerable<KeyValuePair<string, object?>>? tags = null)
        where T : struct
    {
        var options = provider.GetService<InstrumentationOptions>();
        var meterFactory = provider.GetService<IMeterFactory>();

        if(options is null || meterFactory is null)
        {
            return null;
        }

        tags ??= [];
        if(options.Tags is not null)
        {
            tags = Enumerable.Union(
                tags,
                options.Tags.AsEnumerable()
            );
        }

        return meterFactory.Create(Diagnostics.MeterName)
            .CreateObservableGauge($"{options.InstrumentNamePrefix}{name}", observeValue, unit, description, tags);
    }
}
