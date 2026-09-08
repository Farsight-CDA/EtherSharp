namespace EtherSharp.Interpreter.Runtime.Tracing;

/// <summary>Provides read-only access to the active bytecode execution.</summary>
/// <remarks>This reader and its live state are borrowed and valid only during the awaited hook callback.</remarks>
public interface IInterpreterExecutionReader
{
    /// <summary>Gets the invocation, shared with call entry and exit callbacks.</summary>
    IInterpreterFrameReader Frame { get; }
    /// <summary>Gets the live operand stack.</summary>
    IInterpreterStackReader Stack { get; }
    /// <summary>Gets the live execution memory.</summary>
    IInterpreterMemoryReader Memory { get; }
}
