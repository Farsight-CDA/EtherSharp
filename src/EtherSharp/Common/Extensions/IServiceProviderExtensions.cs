using EtherSharp.Common.Instrumentation;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.Metrics;

namespace EtherSharp.Common.Extensions;

internal static class IServiceProviderExtensions
{
    internal static OTELCounter<T>? CreateOTELCounter<T>(this IServiceProvider provider,
        string name, string? unit = null, string? description = null)
        where T : struct
    {
        var options = provider.GetService<InstrumentationOptions>();
        var meterFactory = provider.GetService<IMeterFactory>();

        if(options is null || meterFactory is null)
        {
            return null;
        }
        //
        var counter = meterFactory.Create(Diagnostics.MeterName)
            .CreateCounter<T>($"{options.InstrumentNamePrefix}{name}", unit, description);
        return new OTELCounter<T>(counter, options);
    }

    internal static OTELUpDownCounter<T>? CreateOTELUpDownCounter<T>(this IServiceProvider provider,
        string name, string? unit = null, string? description = null)
        where T : struct
    {
        var options = provider.GetService<InstrumentationOptions>();
        var meterFactory = provider.GetService<IMeterFactory>();

        if(options is null || meterFactory is null)
        {
            return null;
        }
        //
        var counter = meterFactory.Create(Diagnostics.MeterName)
            .CreateUpDownCounter<T>($"{options.InstrumentNamePrefix}{name}", unit, description);
        return new OTELUpDownCounter<T>(counter, options);
    }

    internal static ObservableUpDownCounter<T>? CreateOTELObservableUpDownCounter<T>(this IServiceProvider provider,
        string name, Func<T> observeValue, string? unit = null, string? description = null)
        where T : struct
    {
        var options = provider.GetService<InstrumentationOptions>();
        var meterFactory = provider.GetService<IMeterFactory>();

        if(options is null || meterFactory is null)
        {
            return null;
        }
        //
        return meterFactory.Create(Diagnostics.MeterName)
            .CreateObservableUpDownCounter(
                $"{options.InstrumentNamePrefix}{name}",
                () => new Measurement<T>(observeValue(), options.Tags),
                unit,
                description
            );
    }

    internal static ObservableGauge<T>? CreateOTELObservableGauge<T>(this IServiceProvider provider,
        string name, Func<T> observeValue, string? unit = null, string? description = null)
        where T : struct
    {
        var options = provider.GetService<InstrumentationOptions>();
        var meterFactory = provider.GetService<IMeterFactory>();

        if(options is null || meterFactory is null)
        {
            return null;
        }
        //
        return meterFactory.Create(Diagnostics.MeterName)
            .CreateObservableGauge(
                $"{options.InstrumentNamePrefix}{name}",
                () => new Measurement<T>(observeValue(), options.Tags),
                unit,
                description
            );
    }
}
