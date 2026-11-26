namespace EtherSharp.Tx.Legacy;

/// <summary>
/// Fee estimation settings for transactions priced with the legacy gas price fee model.
/// </summary>
public sealed class LegacyGasFeeOptions
{
    /// <summary>
    /// Percentage added to the estimated gas price.
    /// </summary>
    public int GasPriceOffsetPercentage { get; set; } = 15;

    internal void Validate(string paramName)
        => ArgumentOutOfRangeException.ThrowIfNegative(GasPriceOffsetPercentage, paramName);
}
