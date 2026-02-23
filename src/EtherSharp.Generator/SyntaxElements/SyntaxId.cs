namespace EtherSharp.Generator.SyntaxElements;

public readonly struct SyntaxId(int id)
{
    private readonly int _id = id;

    public override bool Equals(object obj)
        => obj is SyntaxId i && i._id == _id;

    public override int GetHashCode()
        => _id;

    public SyntaxId Combine(SyntaxId other)
        => new SyntaxId(HashCode.Combine(_id, other._id));

    public static bool operator ==(SyntaxId left, SyntaxId right)
        => left.Equals(right);

    public static bool operator !=(SyntaxId left, SyntaxId right)
        => !left.Equals(right);
}
