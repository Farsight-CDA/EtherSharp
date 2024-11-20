namespace EtherSharp.Wallet;
public interface IEtherHdWallet
{
    public string Address { get; }
    public void Sign(Span<byte> inBytes, Span<byte> outBytes);
}
