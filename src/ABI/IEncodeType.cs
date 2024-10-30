namespace EtherSharp.ABI;

public interface IEncodeType
{
    public int MetadataSize { get; }

    public int PayloadSize { get; }
}