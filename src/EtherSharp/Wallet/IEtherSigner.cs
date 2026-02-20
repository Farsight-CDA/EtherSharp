using EtherSharp.Types;

namespace EtherSharp.Wallet;

public interface IEtherSigner
{
    public Address Address { get; }

    public bool TrySign(ReadOnlySpan<byte> data, Span<byte> destination);
    public bool TrySignRecoverable(ReadOnlySpan<byte> data, Span<byte> destination);
}
