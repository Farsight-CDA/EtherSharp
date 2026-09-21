using EtherSharp.Interpreter.Runtime;

namespace EtherSharp.Interpreter.Forking;

internal sealed class ForkInterpreterHost(InterpreterRuntime interpreter) : IInterpreterHost
{
    private readonly InterpreterStateFork _fork = interpreter.Fork;

    public async ValueTask<T1> GetAsync<T1>(HostRequest<T1> request)
    {
        await _fork.EnsureCachedAsync(interpreter, [request]);
        return _fork.GetCached(request);
    }

    public async ValueTask<(T1 First, T2 Second)> GetAsync<T1, T2>(
        HostRequest<T1> first,
        HostRequest<T2> second
    )
    {
        await _fork.EnsureCachedAsync(interpreter, [first, second]);
        return (_fork.GetCached(first), _fork.GetCached(second));
    }

    public async ValueTask<(T1 First, T2 Second, T3 Third)> GetAsync<T1, T2, T3>(
        HostRequest<T1> first,
        HostRequest<T2> second,
        HostRequest<T3> third
    )
    {
        await _fork.EnsureCachedAsync(interpreter, [first, second, third]);
        return (_fork.GetCached(first), _fork.GetCached(second), _fork.GetCached(third));
    }

    public async ValueTask<(T1 First, T2 Second, T3 Third, T4 Fourth)> GetAsync<T1, T2, T3, T4>(
        HostRequest<T1> first,
        HostRequest<T2> second,
        HostRequest<T3> third,
        HostRequest<T4> fourth
    )
    {
        await _fork.EnsureCachedAsync(interpreter, [first, second, third, fourth]);
        return (
            _fork.GetCached(first),
            _fork.GetCached(second),
            _fork.GetCached(third),
            _fork.GetCached(fourth)
        );
    }
}
