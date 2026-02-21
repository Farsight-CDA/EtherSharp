namespace EtherSharp.ABI.Types.Base;

internal interface IPackedEncodeType
{
    /// <summary>
    /// Gets the packed encoded size in bytes.
    /// </summary>
    public int PackedSize { get; }

    void EncodePacked(Span<byte> buffer);
}
