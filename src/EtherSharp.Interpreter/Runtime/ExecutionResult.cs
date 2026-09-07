namespace EtherSharp.Interpreter.Runtime;

/// <summary>Describes the outcome of an EVM execution.</summary>
public readonly struct ExecutionResult
{
    private readonly ExecutionHaltKind _haltKind;
    private readonly int _failureReason;

    /// <summary>Gets whether execution succeeded.</summary>
    public bool IsSuccess => _haltKind == ExecutionHaltKind.Success;

    /// <summary>Gets borrowed return or revert bytes; empty for entry failures and exceptional halts.</summary>
    public ReadOnlyMemory<byte> Data { get; }

    private ExecutionResult(
        ExecutionHaltKind haltKind,
        ReadOnlyMemory<byte> data = default,
        int failureReason = default
    )
    {
        _haltKind = haltKind;
        _failureReason = failureReason;
        Data = data;
    }

    internal static ExecutionResult Success(ReadOnlyMemory<byte> data = default)
        => new(ExecutionHaltKind.Success, data);

    internal static ExecutionResult Revert(ReadOnlyMemory<byte> data = default)
        => new(ExecutionHaltKind.Revert, data);

    internal static ExecutionResult ExceptionalHalt(ExceptionalHaltReason reason)
        => Enum.IsDefined(reason)
            ? new(ExecutionHaltKind.ExceptionalHalt, failureReason: (int) reason)
            : throw new ArgumentOutOfRangeException(nameof(reason));

    internal static ExecutionResult CallEntryFailure(CallEntryFailureReason reason)
        => Enum.IsDefined(reason)
            ? new(ExecutionHaltKind.CallEntryFailure, failureReason: (int) reason)
            : throw new ArgumentOutOfRangeException(nameof(reason));

    /// <summary>Determines whether execution explicitly reverted and returns its borrowed revert data.</summary>
    public bool IsRevert(out ReadOnlyMemory<byte> data)
    {
        bool isRevert = _haltKind == ExecutionHaltKind.Revert;
        data = isRevert ? Data : default;
        return isRevert;
    }

    /// <summary>Determines whether execution halted exceptionally and returns the reason.</summary>
    public bool IsExceptionalHalt(out ExceptionalHaltReason reason)
    {
        bool isExceptionalHalt = _haltKind == ExecutionHaltKind.ExceptionalHalt;
        reason = isExceptionalHalt ? (ExceptionalHaltReason) _failureReason : default;
        return isExceptionalHalt;
    }

    /// <summary>Determines whether an invocation entry check failed and returns the reason.</summary>
    public bool IsCallEntryFailure(out CallEntryFailureReason reason)
    {
        bool isCallEntryFailure = _haltKind == ExecutionHaltKind.CallEntryFailure;
        reason = isCallEntryFailure ? (CallEntryFailureReason) _failureReason : default;
        return isCallEntryFailure;
    }
}
