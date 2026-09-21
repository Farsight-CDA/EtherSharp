namespace EtherSharp.Interpreter.Runtime.ExecutionSpecs;

/// <summary>Defines the linear and quadratic coefficients for memory expansion gas.</summary>
public sealed record MemoryGasParameters
{
    /// <summary>The linear cost per 32-byte word of active memory.</summary>
    public required ulong LinearCostPerWord { get; init; }

    /// <summary>The nonzero divisor applied to the square of the active memory word count.</summary>
    public required ulong QuadraticDivisor { get; init; }

    internal void Validate()
        => ArgumentOutOfRangeException.ThrowIfZero(QuadraticDivisor);
}
