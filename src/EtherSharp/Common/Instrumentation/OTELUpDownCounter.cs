using System.Diagnostics.Metrics;

namespace EtherSharp.Common.Instrumentation;
internal record OTELUpDownCounter<T>(UpDownCounter<T> Counter, InstrumentationOptions Options)
    where T : struct
{
}
