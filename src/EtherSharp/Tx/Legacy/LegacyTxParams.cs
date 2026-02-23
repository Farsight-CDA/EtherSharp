using EtherSharp.Tx.Types;

namespace EtherSharp.Tx.Legacy;

/// <summary>
/// Defines additional transaction parameters for legacy transactions.
/// </summary>
public record LegacyTxParams : ITxParams<LegacyTxParams>
{
    /// <inheritdoc/>
    public static LegacyTxParams Default { get; } = new LegacyTxParams();

    /// <inheritdoc/>
    public byte[] Encode() => [];

    /// <inheritdoc/>
    public static LegacyTxParams Decode(ReadOnlySpan<byte> data) => Default;
}
