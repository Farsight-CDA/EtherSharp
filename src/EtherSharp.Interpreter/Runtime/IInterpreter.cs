using EtherSharp.Interpreter.Runtime.ExecutionSpecs;

namespace EtherSharp.Interpreter.Runtime;

/// <summary>Represents retained interpreter state belonging to an interpreter state fork.</summary>
public interface IInterpreter
{
    /// <summary>The interpreter resource limits.</summary>
    public InterpreterResourceLimits ResourceLimits { get; }

    /// <summary>The consensus rules used for execution.</summary>
    public InterpreterExecutionSpec ExecutionSpec { get; }
}
