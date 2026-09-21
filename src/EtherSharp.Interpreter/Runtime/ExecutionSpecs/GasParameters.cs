namespace EtherSharp.Interpreter.Runtime.ExecutionSpecs;

/// <summary>Groups the dynamic gas pricing parameters for an execution specification.</summary>
public sealed record GasParameters
{
    /// <summary>The memory expansion pricing parameters.</summary>
    public required MemoryGasParameters Memory { get; init; }

    internal void Validate()
    {
        ArgumentNullException.ThrowIfNull(Memory);
        Memory.Validate();
    }
}
