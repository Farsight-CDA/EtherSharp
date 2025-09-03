namespace EtherSharp.ABI.Types.Base;
internal interface IPackedEncodeType
{
    /// <summary>
    /// Byte count needed to ABI encode the value using packed mode.
    /// </summary>
    public int PackedSize { get; }

    void EncodePacked(Span<byte> buffer);
}
