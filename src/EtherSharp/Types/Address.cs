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
        => new Address(s, Convert.FromHexString(s));

    public static Address FromBytes(ReadOnlySpan<byte> b)
        => new Address(Convert.ToHexString(b), b.ToArray());
}
