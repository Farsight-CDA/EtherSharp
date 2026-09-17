using EtherSharp.Interpreter.Forking;

namespace EtherSharp.Interpreter.Runtime;

/// <summary>
/// Provides upstream EVM state and call execution for an interpreter state fork.
/// </summary>
/// <remarks>Each host instance belongs to one retained interpreter state.</remarks>
public interface IInterpreterHost
{
    /// <summary>Resolves one typed host request.</summary>
    public ValueTask<T1> GetAsync<T1>(
        HostRequest<T1> request
    );

    /// <summary>Resolves two typed host requests together.</summary>
    public ValueTask<(T1 First, T2 Second)> GetAsync<T1, T2>(
        HostRequest<T1> first,
        HostRequest<T2> second
    );

    /// <summary>Resolves three typed host requests together.</summary>
    public ValueTask<(T1 First, T2 Second, T3 Third)> GetAsync<T1, T2, T3>(
        HostRequest<T1> first,
        HostRequest<T2> second,
        HostRequest<T3> third
    );
}
