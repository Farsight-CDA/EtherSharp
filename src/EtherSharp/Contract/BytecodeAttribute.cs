namespace EtherSharp.Contract;

/// <summary>
/// Supplies either initcode or runtime code for a generated contract interface.
/// </summary>
/// <param name="initCode">The contract initcode as a hexadecimal string.</param>
/// <param name="runtimeCode">The contract runtime code as a hexadecimal string.</param>
[AttributeUsage(AttributeTargets.Interface)]
[System.Diagnostics.Conditional("ETHERSHARP_GENERATOR")]
public sealed class BytecodeAttribute(string? initCode = null, string? runtimeCode = null) : Attribute
{
    /// <summary>
    /// Gets the contract initcode.
    /// </summary>
    public string? InitCode { get; } = initCode;

    /// <summary>
    /// Gets the contract runtime code.
    /// </summary>
    public string? RuntimeCode { get; } = runtimeCode;
}
