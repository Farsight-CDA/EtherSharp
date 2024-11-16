using System.Globalization;
using System.Numerics;

namespace EtherSharp.Crypto.Curves;
internal class ED25519 : BIP32Curve
{
    private const string _ed25519NHex = "7FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFED";
    private static readonly byte[] _ed25519NBytes = Convert.FromHexString(_ed25519NHex);
    private static readonly BigInteger _ed25519N = BigInteger.Parse(_ed25519NHex, style: NumberStyles.AllowHexSpecifier);

    protected override string Name => "ed25519 seed";
    protected override BigInteger N => _ed25519N;
    protected override ReadOnlySpan<byte> NBytes => _ed25519NBytes;

    protected override void SerializedPoint(Span<byte> point, Span<byte> destination) 
        => throw new NotImplementedException();
}