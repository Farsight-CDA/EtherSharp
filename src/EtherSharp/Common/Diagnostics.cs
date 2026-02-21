namespace EtherSharp.Common;

/// <summary>
/// Shared diagnostics identifiers used by EtherSharp instrumentation.
/// </summary>
public static class Diagnostics
{
    /// <summary>
    /// Meter name used when creating OpenTelemetry/System.Diagnostics metrics.
    /// </summary>
    /// <remarks>
    /// Use this name when subscribing a <see cref="System.Diagnostics.Metrics.MeterListener"/> or configuring exporter filters.
    /// </remarks>
    public const string METER_NAME = "Ethersharp";
}
