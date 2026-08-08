using EtherSharp.Types;

namespace EtherSharp.RPC.Modules.Trace.Types;

/// <summary>
/// Maps every altered account to its field-level state changes.
/// </summary>
public sealed class TraceStateDiff : Dictionary<Address, AccountStateDiff>;
