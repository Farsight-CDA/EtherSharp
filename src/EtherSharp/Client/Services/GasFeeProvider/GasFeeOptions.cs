using EtherSharp.Tx.EIP1559;
using EtherSharp.Tx.Legacy;

namespace EtherSharp.Client.Services.GasFeeProvider;

/// <summary>
/// Configuration for the built-in gas fee providers.
/// </summary>
public sealed class GasFeeOptions
{
    /// <summary>
    /// Percentage added to the estimated gas limit of every transaction.
    /// </summary>
    public ulong GasLimitOffsetPercentage { get; set; } = 15;

    /// <summary>
    /// Fee estimation settings used by <see cref="EIP1559GasFeeProvider"/>.
    /// </summary>
    public EIP1559GasFeeOptions EIP1559 { get; } = new EIP1559GasFeeOptions();

    /// <summary>
    /// Fee estimation settings used by <see cref="LegacyGasFeeProvider"/>.
    /// </summary>
    public LegacyGasFeeOptions Legacy { get; } = new LegacyGasFeeOptions();

    internal void Validate(string paramName)
    {
        EIP1559.Validate(paramName);
        Legacy.Validate(paramName);
    }
}
