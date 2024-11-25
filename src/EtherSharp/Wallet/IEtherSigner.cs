namespace EtherSharp.Wallet;
public interface IEtherSigner
{
    public bool TrySign(ReadOnlySpan<byte> data, Span<byte> destination);
    public bool TrySignRecoverable(ReadOnlySpan<byte> data, Span<byte> destination);
}
