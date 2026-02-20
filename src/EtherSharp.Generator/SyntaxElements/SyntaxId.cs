namespace EtherSharp.Generator.SyntaxElements;

public readonly struct SyntaxId(int id)
{
    private readonly int Id = id;

    public override bool Equals(object obj)
        => obj is SyntaxId i && i.Id == Id;

    public override int GetHashCode()
        => Id;

    public SyntaxId Combine(SyntaxId other)
        => new SyntaxId(HashCode.Combine(Id, other.Id));

    public static bool operator ==(SyntaxId left, SyntaxId right)
        => left.Equals(right);

    public static bool operator !=(SyntaxId left, SyntaxId right)
        => !left.Equals(right);
}
