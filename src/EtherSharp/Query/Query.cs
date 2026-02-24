namespace EtherSharp.Query;

internal record Query<T>(IReadOnlyList<IQuery> Queries, Func<ReadOnlySpan<ReadOnlyMemory<byte>>, T> ReadResultFrom) : IQuery<T>
{
    T IQuery<T>.ReadResultFrom(params scoped ReadOnlySpan<ReadOnlyMemory<byte>> queryResults) => ReadResultFrom(queryResults);
}
