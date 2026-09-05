namespace EtherSharp.Interpreter.Runtime;

/// <summary>
/// Identifies an exceptional execution failure that consumes the frame's remaining execution gas.
/// </summary>
public enum ExceptionalHaltReason
{
    /// <summary>The frame has insufficient gas to complete an operation.</summary>
    OutOfGas,
    /// <summary>The operand stack contains too few values for the instruction.</summary>
    StackUnderflow,
    /// <summary>The instruction would exceed the operand stack capacity.</summary>
    StackOverflow,
    /// <summary>A jump targets an invalid destination.</summary>
    InvalidJumpDestination,
    /// <summary>The opcode is invalid or unavailable in the execution context.</summary>
    InvalidOpcode,
    /// <summary>The operation is prohibited in a static context.</summary>
    WriteProtection,
    /// <summary>A copy reads beyond the return-data buffer.</summary>
    ReturnDataOutOfBounds,
    /// <summary>The destination account prevents contract creation.</summary>
    ContractAddressCollision,
    /// <summary>The returned runtime code has a prohibited format.</summary>
    InvalidRuntimeCode,
    /// <summary>The returned runtime code exceeds the configured size limit.</summary>
    RuntimeCodeTooLarge,
    /// <summary>The supplied initcode exceeds the configured size limit.</summary>
    InitCodeTooLarge,
    /// <summary>A native precompile reports execution failure.</summary>
    PrecompileFailure
}
