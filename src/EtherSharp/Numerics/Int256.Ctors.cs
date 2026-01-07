namespace EtherSharp.Numerics;

public readonly partial struct Int256
{
    public Int256(ReadOnlySpan<byte> bytes, bool isBigEndian = false)
    {
        _value = new UInt256(bytes, isBigEndian);
    }

    public Int256(UInt256 value)
    {
        _value = value;
    }

    public Int256(int n)
    {
        if(n < 0)
        {
            Int256 value = new(new UInt256((ulong) -n));
            var res = Negate(value);
            _value = res._value;
        }
        else
        {
            _value = new UInt256((ulong) n);
        }
    }
}
