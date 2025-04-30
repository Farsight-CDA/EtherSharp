namespace EtherSharp.ABI.Types.Interfaces;
internal interface IPackedEncodeType
{
    public int PackedSize { get; }

    public void EncodePacked(Span<byte> buffer);
}
