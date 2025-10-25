using EtherSharp.Tx;

namespace EtherSharp.Client.Modules.Query;

public interface ICallable<T>
{
    internal IEnumerable<ITxInput> GetCalls();
    internal Func<ReadOnlySpan<byte[]>, T> GetResultSelector();
}
