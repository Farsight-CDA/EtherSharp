namespace EtherSharp.Interpreter.Runtime.Tracing;

/// <summary>Reads the live operand stack without changing it.</summary>
public interface IInterpreterStackReader
{
    /// <summary>Gets the number of stack words.</summary>
    int Count { get; }
}
