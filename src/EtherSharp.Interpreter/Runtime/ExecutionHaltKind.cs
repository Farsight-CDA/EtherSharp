namespace EtherSharp.Interpreter.Runtime;

internal enum ExecutionHaltKind
{
    Success,
    Revert,
    ExceptionalHalt,
    // The call did not enter execution; its allocated execution gas is not consumed.
    CallEntryFailure,
    // An exceptional halt carrying a precompile-specific failure reason.
    PrecompileFailure
}
