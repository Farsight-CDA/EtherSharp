using EtherSharp.Interpreter.Runtime.ExecutionSpecs;

namespace EtherSharp.Interpreter.Runtime;

/// <summary>Represents retained interpreter state belonging to an interpreter state fork.</summary>
public interface IInterpreter
{
    /// <summary>The interpreter resource limits.</summary>
    public InterpreterResourceLimits ResourceLimits { get; }

    /// <summary>The consensus rules used for execution.</summary>
    public InterpreterExecutionSpec ExecutionSpec { get; }

    /// <summary>The lifetime number of asynchronous host-request waits resumed, including failed requests.</summary>
    /// <remarks>
    /// A group of requests counts once, even if resolved in multiple batches. Requests that complete
    /// synchronously do not count. New interpreters start at zero; clones inherit the source's count
    /// and track subsequent interruptions independently.
    /// </remarks>
    public long InterruptionCount { get; }
}
