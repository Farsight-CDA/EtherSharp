using EtherSharp.RPC.Modules.Trace.Converter;
using System.Text.Json.Serialization;

namespace EtherSharp.RPC.Modules.Trace.Types;

/// <summary>
/// Trace payloads requested from an ad-hoc <c>trace_*</c> RPC method.
/// </summary>
[Flags]
[JsonConverter(typeof(TraceTypesJsonConverter))]
public enum TraceTypes
{
    /// <summary>
    /// Flat transaction call trace.
    /// </summary>
    Trace = 1,

    /// <summary>
    /// Recursive virtual-machine execution trace.
    /// </summary>
    VmTrace = 2,

    /// <summary>
    /// Altered account state.
    /// </summary>
    StateDiff = 4,

    /// <summary>
    /// All supported trace payloads.
    /// </summary>
    All = Trace | VmTrace | StateDiff
}
