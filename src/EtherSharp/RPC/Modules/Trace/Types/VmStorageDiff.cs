using EtherSharp.Numerics;

namespace EtherSharp.RPC.Modules.Trace.Types;

/// <summary>
/// Represents a storage write.
/// </summary>
/// <param name="Key">Storage key written.</param>
/// <param name="Val">Value written to storage.</param>
public sealed record VmStorageDiff(UInt256 Key, UInt256 Val);
