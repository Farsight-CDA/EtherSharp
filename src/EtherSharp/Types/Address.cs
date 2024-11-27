namespace EtherSharp.Types;
public class Address
{
    private readonly byte[] _addressBytes;

    public string String { get; }
    public ReadOnlySpan<byte> Bytes => _addressBytes;

    private Address(string s, byte[] b)
    {
        String = s;
        _addressBytes = b;
    }

    public static Address FromString(string s)
    {
        if(s.StartsWith("0x"))
        {
            return new Address(s, Convert.FromHexString(s.AsSpan()[2..]));
        }
        else
        {
            return new Address($"0x{s}", Convert.FromHexString(s));
        }
    }

    public static Address FromBytes(ReadOnlySpan<byte> b)
        => new Address($"0x{Convert.ToHexString(b)}", b.ToArray());
}
