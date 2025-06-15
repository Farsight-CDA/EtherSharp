using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.Metrics;

namespace EtherSharp.Common.Extensions;
internal static class IServiceProviderExtensions
{
    public static Counter<T>? CreateInstrumentationCounter<T>(this IServiceProvider provider, string name, string? unit = null, string? description = null)
        where T : struct
    {
        var options = provider.GetService<InstrumentationOptions>();
        var meterFactory = provider.GetService<IMeterFactory>();

        if(options is null || meterFactory is null)
        {
            return null;
        }

        return meterFactory.Create(Diagnostics.MeterName)
            .CreateCounter<T>($"{options.InstrumentNamePrefix}{name}", unit, description);
    }
}
