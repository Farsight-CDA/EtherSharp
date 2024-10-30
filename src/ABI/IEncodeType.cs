namespace EtherSharp.ABI;

public interface IEncodeType
{
    public uint MetadataSize { get; }
    public uint PayloadSize { get; }
}