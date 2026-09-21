using EtherSharp.Interpreter.Forking;

namespace EtherSharp.Interpreter.Runtime;

internal sealed partial class InterpreterRuntime : IInterpreterHost
{
    private readonly ForkInterpreterHost _host;
    private long _interruptionCount;

    public long InterruptionCount => Interlocked.Read(ref _interruptionCount);

    internal InterpreterStateFork Fork { get; }
    internal InterpreterStateFork.RunParticipant.Lane? Participant { get; set; }

    public ValueTask<T1> GetAsync<T1>(HostRequest<T1> request)
        => TrackInterruptionAsync(_host.GetAsync(request));

    public ValueTask<(T1 First, T2 Second)> GetAsync<T1, T2>(
        HostRequest<T1> first,
        HostRequest<T2> second
    ) => TrackInterruptionAsync(_host.GetAsync(first, second));

    public ValueTask<(T1 First, T2 Second, T3 Third)> GetAsync<T1, T2, T3>(
        HostRequest<T1> first,
        HostRequest<T2> second,
        HostRequest<T3> third
    ) => TrackInterruptionAsync(_host.GetAsync(first, second, third));

    public ValueTask<(T1 First, T2 Second, T3 Third, T4 Fourth)> GetAsync<T1, T2, T3, T4>(
        HostRequest<T1> first,
        HostRequest<T2> second,
        HostRequest<T3> third,
        HostRequest<T4> fourth
    ) => TrackInterruptionAsync(_host.GetAsync(first, second, third, fourth));

    private ValueTask<T> TrackInterruptionAsync<T>(ValueTask<T> pending)
        => pending.IsCompleted ? pending : AwaitInterruptionAsync(pending);

    private async ValueTask<T> AwaitInterruptionAsync<T>(ValueTask<T> pending)
    {
        try
        {
            return await pending;
        }
        finally
        {
            Interlocked.Increment(ref _interruptionCount);
        }
    }
}
