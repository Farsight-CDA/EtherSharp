namespace EtherSharp.Crypto.Curves;
public ref struct ECPoint
{
    public Span<byte> X;
    public Span<byte> Y;

    public readonly bool IsInfinity
        => X.IndexOfAnyExcept((byte) 0) == -1 && Y.IndexOfAnyExcept((byte) 0) == -1;

    public ECPoint(Span<byte> x, Span<byte> y)
    {
        X = x;
        Y = y;
    }
}
