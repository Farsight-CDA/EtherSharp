namespace EtherSharp.Interpreter.Runtime;

internal readonly struct ExecutionResult
{
    private readonly ExecutionHaltKind _haltKind;
    private readonly int _failureReason;

    public bool IsSuccess => _haltKind == ExecutionHaltKind.Success;

    // Return/revert data on those outcomes; empty on either kind of failure.
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

    public static ExecutionResult Success(ReadOnlyMemory<byte> data = default)
        => new(ExecutionHaltKind.Success, data);

    public static ExecutionResult Revert(ReadOnlyMemory<byte> data = default)
        => new(ExecutionHaltKind.Revert, data);

    public static ExecutionResult ExceptionalHalt(ExceptionalHaltReason reason)
        => Enum.IsDefined(reason)
            ? new(ExecutionHaltKind.ExceptionalHalt, failureReason: (int) reason)
            : throw new ArgumentOutOfRangeException(nameof(reason));

    public static ExecutionResult CallEntryFailure(CallEntryFailureReason reason)
        => Enum.IsDefined(reason)
            ? new(ExecutionHaltKind.CallEntryFailure, failureReason: (int) reason)
            : throw new ArgumentOutOfRangeException(nameof(reason));

    public bool IsRevert(out ReadOnlyMemory<byte> data)
    {
        bool isRevert = _haltKind == ExecutionHaltKind.Revert;
        data = isRevert ? Data : default;
        return isRevert;
    }

    public bool IsExceptionalHalt(out ExceptionalHaltReason reason)
    {
        bool isExceptionalHalt = _haltKind == ExecutionHaltKind.ExceptionalHalt;
        reason = isExceptionalHalt ? (ExceptionalHaltReason) _failureReason : default;
        return isExceptionalHalt;
    }

    public bool IsCallEntryFailure(out CallEntryFailureReason reason)
    {
        bool isCallEntryFailure = _haltKind == ExecutionHaltKind.CallEntryFailure;
        reason = isCallEntryFailure ? (CallEntryFailureReason) _failureReason : default;
        return isCallEntryFailure;
    }
}
