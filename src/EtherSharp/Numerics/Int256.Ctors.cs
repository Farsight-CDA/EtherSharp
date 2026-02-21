namespace EtherSharp.Numerics;

#pragma warning disable CS1591

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

    //public Int256(int n)
    //{
    //    if(n < 0)
    //    {
    //        Int256 value = new(new UInt256((ulong) -n));
    //        _value = Negate(value)._value;
    //    }
    //    else
    //    {
    //        _value = new UInt256((ulong) n);
    //    }
    //}

    public Int256(long n)
    {
        if(n < 0)
        {
            var value = new Int256(new UInt256((ulong) -n));
            _value = Negate(value)._value;
        }
        else
        {
            _value = new UInt256((ulong) n);
        }
    }
}
