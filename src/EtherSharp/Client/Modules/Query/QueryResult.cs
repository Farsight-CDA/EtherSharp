namespace EtherSharp.Client.Modules.Query;

public abstract record QueryResult<T>
{
    public record Success(T Value) : QueryResult<T>;
    public record Reverted(byte[] Data) : QueryResult<T>;
}
