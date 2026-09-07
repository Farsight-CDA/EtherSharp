namespace EtherSharp.Interpreter.Runtime;

/// <summary>Identifies the cause of a precompile's exceptional execution failure.</summary>
public enum PrecompileFailureReason
{
    /// <summary>The precompile failed without a more specific reason being available.</summary>
    Unspecified,
    /// <summary>The input byte length is invalid for the precompile.</summary>
    InvalidInputLength,
    /// <summary>The BLAKE2F final-block flag is neither zero nor one.</summary>
    Blake2FInvalidFinalBlockFlag,
    /// <summary>A MODEXP operand's declared length exceeds the configured limit.</summary>
    ModExpOperandLengthExceeded
}
