using EtherSharp.Types;

namespace EtherSharp.Client.Modules.Query;

/// <summary>
/// Represents a call payload that returns a result of type <typeparamref name="T"/> when eth_call'ed.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IQueryable<T>
{
    internal IEnumerable<ICallInput> GetQueryInputs();
    internal T ReadResultFrom(params ReadOnlySpan<TxCallResult> callResults);
}
