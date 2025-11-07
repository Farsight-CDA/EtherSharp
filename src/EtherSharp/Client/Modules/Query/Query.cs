namespace EtherSharp.Client.Modules.Query;

internal record Query<T>(IReadOnlyList<IQuery> Queries, Func<ReadOnlySpan<byte[]>, T> ReadResultFrom) : IQuery<T>
{
    T IQuery<T>.ReadResultFrom(params scoped ReadOnlySpan<byte[]> queryResults) => ReadResultFrom(queryResults);
}
