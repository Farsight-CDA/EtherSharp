namespace EtherSharp.ABI.Types.Base;
internal interface IPackedEncodeType
{
    public int PackedSize { get; }

    public void EncodePacked(Span<byte> buffer);
}
