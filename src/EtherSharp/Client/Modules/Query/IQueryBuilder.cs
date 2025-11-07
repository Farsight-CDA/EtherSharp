namespace EtherSharp.Client.Modules.Query;

public interface IQueryBuilder<TQuery>
{
    public IReadOnlyList<IQuery> Queries { get; }
    public List<TQuery> ParseResults(byte[][] outputs);
}
