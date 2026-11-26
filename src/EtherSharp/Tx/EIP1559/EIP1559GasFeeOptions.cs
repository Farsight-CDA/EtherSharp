namespace EtherSharp.Tx.EIP1559;

/// <summary>
/// Fee estimation settings for transactions priced with the EIP-1559 fee model.
/// </summary>
public sealed class EIP1559GasFeeOptions
{
    /// <summary>
    /// Number of recent blocks used when querying fee history.
    /// </summary>
    public int FeeHistoryRange { get; set; } = 5;

    /// <summary>
    /// Reward percentile used to estimate the priority fee.
    /// </summary>
    public double PriorityFeePercentile { get; set; } = 25;

    /// <summary>
    /// Percentage added to the estimated base fee.
    /// </summary>
    public int BaseFeeOffsetPercentage { get; set; } = 20;

    /// <summary>
    /// Percentage added to the estimated priority fee.
    /// </summary>
    public int PriorityFeeOffsetPercentage { get; set; } = 20;

    internal void Validate(string paramName)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(FeeHistoryRange, 1, paramName);

        if(!(PriorityFeePercentile is >= 0 and <= 100))
        {
            throw new ArgumentOutOfRangeException(paramName, PriorityFeePercentile, $"{nameof(PriorityFeePercentile)} must be between 0 and 100.");
        }

        ArgumentOutOfRangeException.ThrowIfNegative(BaseFeeOffsetPercentage, paramName);
        ArgumentOutOfRangeException.ThrowIfNegative(PriorityFeeOffsetPercentage, paramName);
    }
}
