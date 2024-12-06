namespace EtherSharp.Events;
public interface IEventData
{
    public string Address { get; }
    public byte[][] Topics { get; }
    public byte[] Data { get; }
}
