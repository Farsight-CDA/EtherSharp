using EtherSharp.Tx.Types;

namespace EtherSharp.Tx.Legacy;

public record LegacyTxParams : ITxParams<LegacyTxParams>
{
    public static LegacyTxParams Default { get; } = new LegacyTxParams();

    public byte[] Encode() => [];
    public static LegacyTxParams Decode(ReadOnlySpan<byte> data) => Default;
}
