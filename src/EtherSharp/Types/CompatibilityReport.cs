namespace EtherSharp.Types;

/// <summary>
/// Represents a report regarding OPCode Support on an EVM Chain.
/// </summary>
/// <param name="SupportsPush0"></param>
/// <param name="SupportsMCopy"></param>
/// <param name="SupportsTStore"></param>
/// <param name="SupportsBaseFee"></param>
public record CompatibilityReport(
    bool SupportsPush0,
    bool SupportsMCopy,
    bool SupportsTStore,
    bool SupportsBaseFee
);
