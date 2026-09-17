using EtherSharp.Interpreter.Forking;

namespace EtherSharp.Interpreter.Runtime;

internal sealed partial class InterpreterRuntime : IInterpreterHost
{
    internal InterpreterStateFork Fork { get; }
    internal InterpreterStateFork.RunParticipant.Lane? Participant { get; set; }

    async ValueTask<T1> IInterpreterHost.GetAsync<T1>(HostRequest<T1> request)
    {
        await Fork.EnsureCachedAsync(this, [request]);
        return Fork.GetCached(request);
    }

    async ValueTask<(T1 First, T2 Second)> IInterpreterHost.GetAsync<T1, T2>(
        HostRequest<T1> first,
        HostRequest<T2> second
    )
    {
        await Fork.EnsureCachedAsync(this, [first, second]);
        return (Fork.GetCached(first), Fork.GetCached(second));
    }

    async ValueTask<(T1 First, T2 Second, T3 Third)> IInterpreterHost.GetAsync<T1, T2, T3>(
        HostRequest<T1> first,
        HostRequest<T2> second,
        HostRequest<T3> third
    )
    {
        await Fork.EnsureCachedAsync(this, [first, second, third]);
        return (Fork.GetCached(first), Fork.GetCached(second), Fork.GetCached(third));
    }
}
