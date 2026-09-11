using EtherSharp.Common.Exceptions;
using EtherSharp.Contract;
using EtherSharp.Interpreter.Runtime.ExecutionSpecs;
using EtherSharp.Interpreter.Runtime.Memory;
using EtherSharp.Interpreter.Runtime.Precompiles;
using EtherSharp.Interpreter.Runtime.Storage;
using EtherSharp.Interpreter.Runtime.Tracing;
using EtherSharp.Numerics;
using EtherSharp.Tx;
using EtherSharp.Tx.EIP1559;
using EtherSharp.Tx.Legacy;
using EtherSharp.Types;
using System.Collections.Frozen;

namespace EtherSharp.Interpreter.Runtime;

/// <summary>
/// Executes and simulates EVM transactions and calls against an interpreter state fork.
/// </summary>
/// <remarks>
/// Overlapping execution operations on the same runtime are rejected. Await an operation before
/// starting another. Disposal must not run concurrently with an execution operation.
/// </remarks>
public partial class InterpreterRuntime : IDisposable
{
    private sealed class ExecutionState(IInterpreterExecutionHooks? hooks)
    {
        public IInterpreterExecutionHooks? Hooks { get; } = hooks;
        public TransactionEnvironment Transaction { get; set; }
        public int NextFrameId { get; set; }
    }

    private readonly InterpreterStorage _storage;
    private readonly FrozenDictionary<Address, IPrecompile> _precompiles;
    private readonly InterpreterContext _context;
    private ExecutionState? _executionState;
    private bool _isDisposed;

    /// <summary>The interpreter resource limits.</summary>
    public InterpreterResourceLimits ResourceLimits { get; }
    /// <summary>The consensus rules used for execution.</summary>
    public InterpreterExecutionSpec ExecutionSpec { get; }

    internal IInterpreterHost Host { get; }

    internal InterpreterRuntime(
        InterpreterContext context,
        IInterpreterHost host,
        InterpreterExecutionSpec executionSpec,
        InterpreterResourceLimits resourceLimits,
        FrozenDictionary<Address, IPrecompile> precompiles
    )
    {
        ExecutionSpec = executionSpec;
        ResourceLimits = resourceLimits;
        _context = context;
        Host = host;
        _storage = new InterpreterStorage(host);
        _precompiles = precompiles;
    }

    internal InterpreterRuntime Clone(IInterpreterHost host)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        var clone = new InterpreterRuntime(_context, host, ExecutionSpec, ResourceLimits, _precompiles);
        _storage.CopyTo(clone._storage);
        return clone;
    }

    /// <summary>
    /// Removes this interpreter from its state fork's batching participants.
    /// </summary>
    /// <remarks>This method must not be called while an interpreter operation is in progress.</remarks>
    public void Dispose()
    {
        if(_isDisposed)
        {
            return;
        }

        _isDisposed = true;
        Host.Unregister();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Executes a transaction from the supplied sender with optional tracing hooks and retains its state changes.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when the transaction is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the nonce handling mode is invalid.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when the interpreter is disposed.</exception>
    /// <exception cref="InvalidOperationException">Thrown when transaction validation fails or an execution is already in progress.</exception>
    public ValueTask<TxCallResult> ExecuteTransactionAsync(
        Address sender,
        LegacyTransaction transaction,
        IInterpreterExecutionHooks? hooks = default,
        TopLevelNonceHandling topLevelNonceHandling = TopLevelNonceHandling.Default
    )
    {
        ArgumentNullException.ThrowIfNull(transaction);
        return transaction.ChainId != _context.ChainId
            ? throw new InvalidOperationException("Transaction chain ID does not match the execution context.")
            : ExecuteTopLevelAsync(
                TransactionEnvironment.CreateForTransaction(sender, transaction, _context), retainState: true, isCall: false,
                options: new InterpreterSimulationOptions { Hooks = hooks, TopLevelNonceHandling = topLevelNonceHandling }
            );
    }

    /// <inheritdoc cref="ExecuteTransactionAsync(Address, LegacyTransaction, IInterpreterExecutionHooks, TopLevelNonceHandling)"/>
    public ValueTask<TxCallResult> ExecuteTransactionAsync(
        Address sender,
        EIP1559Transaction transaction,
        IInterpreterExecutionHooks? hooks = default,
        TopLevelNonceHandling topLevelNonceHandling = TopLevelNonceHandling.Default
    )
    {
        ArgumentNullException.ThrowIfNull(transaction);
        return transaction.ChainId != _context.ChainId
            ? throw new InvalidOperationException("Transaction chain ID does not match the execution context.")
            : ExecuteTopLevelAsync(
                TransactionEnvironment.CreateForTransaction(sender, transaction, _context), retainState: true, isCall: false,
                options: new InterpreterSimulationOptions { Hooks = hooks, TopLevelNonceHandling = topLevelNonceHandling }
            );
    }

    /// <summary>
    /// Executes a call from the supplied sender with optional tracing hooks and retains its state changes.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when the call is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the nonce handling mode is invalid.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when the interpreter is disposed.</exception>
    /// <exception cref="InvalidOperationException">Thrown when call validation fails or an execution is already in progress.</exception>
    public ValueTask<TxCallResult> ExecuteCallAsync(
        Address sender,
        ITxInput call,
        IInterpreterExecutionHooks? hooks = default,
        TopLevelNonceHandling topLevelNonceHandling = TopLevelNonceHandling.Default
    )
    {
        ArgumentNullException.ThrowIfNull(call);
        return ExecuteTopLevelAsync(
            TransactionEnvironment.CreateForCall(sender, call, 0, _context), retainState: true, isCall: true,
            options: new InterpreterSimulationOptions { Hooks = hooks, TopLevelNonceHandling = topLevelNonceHandling }
        );
    }

    /// <inheritdoc cref="ExecuteCallAsync(Address, ITxInput, IInterpreterExecutionHooks, TopLevelNonceHandling)"/>
    /// <exception cref="CallRevertedException">Thrown when execution reverts.</exception>
    /// <exception cref="CallParsingException">Thrown when the return data cannot be decoded.</exception>
    public async ValueTask<T> ExecuteCallAsync<T>(
        Address sender,
        ITxInput<T> call,
        IInterpreterExecutionHooks? hooks = default,
        TopLevelNonceHandling topLevelNonceHandling = TopLevelNonceHandling.Default
    )
    {
        var result = await SafeExecuteCallAsync(sender, call, hooks, topLevelNonceHandling);
        return result.Unwrap();
    }

    /// <inheritdoc cref="ExecuteCallAsync(Address, ITxInput, IInterpreterExecutionHooks, TopLevelNonceHandling)"/>
    public async ValueTask<CallResult<T>> SafeExecuteCallAsync<T>(
        Address sender,
        ITxInput<T> call,
        IInterpreterExecutionHooks? hooks = default,
        TopLevelNonceHandling topLevelNonceHandling = TopLevelNonceHandling.Default
    )
    {
        var result = await ExecuteCallAsync(sender, (ITxInput) call, hooks, topLevelNonceHandling);
        return CallResult<T>.ParseFrom(result, call.To, call.ReadResultFrom);
    }

    /// <summary>
    /// Simulates a transaction from the supplied sender and discards all state changes.
    /// </summary>
    public ValueTask<TxCallResult> SimulateTransactionAsync(
        Address sender,
        LegacyTransaction transaction,
        InterpreterSimulationOptions options = default
    )
    {
        ArgumentNullException.ThrowIfNull(transaction);
        return transaction.ChainId != _context.ChainId
            ? throw new InvalidOperationException("Transaction chain ID does not match the execution context.")
            : ExecuteTopLevelAsync(
                TransactionEnvironment.CreateForTransaction(sender, transaction, _context), retainState: false, isCall: false, options: options
            );
    }

    /// <inheritdoc cref="SimulateTransactionAsync(Address, LegacyTransaction, InterpreterSimulationOptions)"/>
    public ValueTask<TxCallResult> SimulateTransactionAsync(
        Address sender,
        EIP1559Transaction transaction,
        InterpreterSimulationOptions options = default
    )
    {
        ArgumentNullException.ThrowIfNull(transaction);
        return transaction.ChainId != _context.ChainId
            ? throw new InvalidOperationException("Transaction chain ID does not match the execution context.")
            : ExecuteTopLevelAsync(
                TransactionEnvironment.CreateForTransaction(sender, transaction, _context), retainState: false, isCall: false, options: options
            );
    }

    /// <summary>
    /// Simulates a call from the supplied sender and discards all state changes.
    /// </summary>
    /// <param name="sender">The caller exposed through <c>msg.sender</c>.</param>
    /// <param name="call">The destination, value, and calldata supplied to the call.</param>
    /// <param name="options">The simulation options.</param>
    public ValueTask<TxCallResult> SimulateCallAsync(
        Address sender,
        ITxInput call,
        InterpreterSimulationOptions options = default
    )
    {
        ArgumentNullException.ThrowIfNull(call);
        return ExecuteTopLevelAsync(
            TransactionEnvironment.CreateForCall(sender, call, 0, _context), retainState: false, isCall: true, options: options
        );
    }

    /// <summary>
    /// Simulates a call from the supplied sender, discards all state changes, and returns the decoded value.
    /// </summary>
    /// <typeparam name="T">The decoded return type.</typeparam>
    /// <param name="sender">The caller exposed through <c>msg.sender</c>.</param>
    /// <param name="call">The destination, value, calldata, and result decoder supplied to the call.</param>
    /// <param name="options">The simulation options.</param>
    /// <exception cref="CallRevertedException">Thrown when execution reverts.</exception>
    /// <exception cref="CallParsingException">Thrown when the return data cannot be decoded.</exception>
    public async ValueTask<T> SimulateCallAsync<T>(
        Address sender,
        ITxInput<T> call,
        InterpreterSimulationOptions options = default
    )
    {
        var result = await SafeSimulateCallAsync(sender, call, options);
        return result.Unwrap();
    }

    /// <summary>
    /// Simulates a call from the supplied sender, discards all state changes, and returns its typed outcome.
    /// </summary>
    /// <param name="sender">The caller exposed through <c>msg.sender</c>.</param>
    /// <param name="call">The destination, value, calldata, and result decoder supplied to the call.</param>
    /// <param name="options">The simulation options.</param>
    public async ValueTask<CallResult<T>> SafeSimulateCallAsync<T>(
        Address sender,
        ITxInput<T> call,
        InterpreterSimulationOptions options = default
    )
    {
        var result = await SimulateCallAsync(sender, (ITxInput) call, options);
        return CallResult<T>.ParseFrom(result, call.To, call.ReadResultFrom);
    }

    private async ValueTask<TxCallResult> ExecuteTopLevelAsync(
        TransactionEnvironment environment,
        bool retainState,
        bool isCall,
        InterpreterSimulationOptions options = default
    )
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        bool skipTopLevelNonceChecks = options.TopLevelNonceHandling switch
        {
            TopLevelNonceHandling.Default => isCall && environment.Input.To is not null,
            TopLevelNonceHandling.Validate => false,
            TopLevelNonceHandling.Skip when environment.Input.To is null
                => throw new InvalidOperationException("Top-level contract creation requires sender nonce checks."),
            TopLevelNonceHandling.Skip => true,
            _ => throw new ArgumentOutOfRangeException(nameof(options), options.TopLevelNonceHandling, "Invalid top-level nonce handling mode.")
        };

        var execution = new ExecutionState(options.Hooks);
        if(Interlocked.CompareExchange(ref _executionState, execution, null) is not null)
        {
            throw new InvalidOperationException("An execution is already in progress on this interpreter.");
        }

        InterpreterStorage.Snapshot? snapshot = null;
        try
        {
            snapshot = _storage.TakeSnapshot();
            if(options.StateOverrides is { } stateOverrides)
            {
                _storage.ApplyStateOverrides(stateOverrides);
            }

            var senderStorage = _storage.GetAccountStorage(environment.Sender);
            if(isCall && !skipTopLevelNonceChecks)
            {
                environment = environment with { Nonce = await senderStorage.GetNonceAsync() };
            }

            execution.Transaction = environment;
            if(execution.Hooks is not null)
            {
                await execution.Hooks.OnExecutionStartAsync(_context, environment, _storage);
            }

            ulong senderNonce = 0;
            if(!skipTopLevelNonceChecks)
            {
                senderNonce = await senderStorage.GetNonceAsync();
                if(senderNonce != environment.Nonce)
                {
                    throw new InvalidOperationException(
                        $"Invalid transaction nonce. Expected {senderNonce}, received {environment.Nonce}."
                    );
                }
                if(senderNonce == UInt64.MaxValue)
                {
                    throw new InvalidOperationException("Transaction sender nonce cannot be incremented.");
                }
            }

            ExecutionResult result;
            if(environment.Input.To is Address target)
            {
                if(!skipTopLevelNonceChecks)
                {
                    senderStorage.SetNonce(senderNonce + 1);
                }
                result = await ExecuteMessageCallAsync(
                    new CallFrame(
                        checked(execution.NextFrameId++),
                        EvmOpcode.Call,
                        environment.Sender,
                        null,
                        environment.Sender,
                        target,
                        target,
                        environment.Input.Value,
                        environment.Input.Data,
                        new GasBudget(environment.GasLimit)
                    )
                );
            }
            else
            {
                if(environment.Input.Data.Length > ExecutionSpec.MaxInitCodeLength)
                {
                    throw new InvalidOperationException("Transaction initcode exceeds the configured limit.");
                }

                var createdAddress = Address.DeriveCreate(environment.Sender, senderNonce);
                result = await ExecuteContractCreationAsync(new CallFrame(
                    checked(execution.NextFrameId++),
                    EvmOpcode.Create,
                    environment.Sender,
                    null,
                    environment.Sender,
                    createdAddress,
                    createdAddress,
                    environment.Input.Value,
                    environment.Input.Data,
                    new GasBudget(environment.GasLimit)
                ));
            }

            if(execution.Hooks is not null)
            {
                await execution.Hooks.OnExecutionEndAsync(_context, environment, result, _storage);
            }
            if(retainState)
            {
                _storage.Commit();
                snapshot = null;
            }
            return new TxCallResult(result.IsSuccess, result.Data);
        }
        finally
        {
            try
            {
                if(snapshot is { } startingState)
                {
                    _storage.Reset(startingState);
                }
            }
            finally
            {
                Volatile.Write(ref _executionState, null);
            }
        }
    }

    private async ValueTask<ExecutionResult> ExecuteMessageCallAsync(CallFrame call)
    {
        _precompiles.TryGetValue(call.To, out var precompile);
        if(_executionState!.Hooks is not null)
        {
            if(precompile is not null)
            {
                await _executionState.Hooks.OnPrecompileEnterAsync(call, _storage);
            }
            else
            {
                await _executionState.Hooks.OnContractEnterAsync(call, _storage);
            }
        }

        var result = call.Depth > CallFrame.MAX_DEPTH
            ? ExecutionResult.CallEntryFailure(CallEntryFailureReason.DepthExceeded)
            : ExecutionResult.Success();

        var accountStorage = _storage.GetAccountStorage(call.Address);
        var callSnapshot = _storage.TakeSnapshot();

        if(result.IsSuccess && !call.Value.IsZero && call.Type is EvmOpcode.Call or EvmOpcode.CallCode)
        {
            var sourceStorage = call.From == call.Address
                ? accountStorage
                : _storage.GetAccountStorage(call.From);
            var sourceBalance = await sourceStorage.GetBalanceAsync();
            if(sourceBalance < call.Value)
            {
                result = ExecutionResult.CallEntryFailure(CallEntryFailureReason.InsufficientBalance);
            }
            else if(call.From != call.Address)
            {
                var targetBalance = await accountStorage.GetBalanceAsync();
                sourceStorage.SetBalance(sourceBalance - call.Value);
                accountStorage.SetBalance(targetBalance + call.Value);
            }
        }

        if(result.IsSuccess)
        {
            if(precompile is not null)
            {
                result = await precompile.ExecuteAsync(Host, new PrecompileCall(
                    _context,
                    call.Origin,
                    call.Caller,
                    call.Address,
                    call.Value,
                    call.Input,
                    call.Depth,
                    call.IsStatic,
                    call.Gas
                ));
            }
            else
            {
                var codeStorage = call.To == call.Address ? accountStorage : _storage.GetAccountStorage(call.To);
                var byteCode = await codeStorage.GetCodeAsync();
                // EIP-7702 delegation: load the target's code without following further delegations.
                if(byteCode.Length == 3 + Address.BYTES_LENGTH
                    && byteCode.ByteCode.Span[0] == 0xEF
                    && byteCode.ByteCode.Span[1] == 0x01
                    && byteCode.ByteCode.Span[2] == 0x00)
                {
                    var delegationTarget = Address.FromBytes(byteCode.ByteCode.Span[3..]);
                    byteCode = await _storage.GetAccountStorage(delegationTarget).GetCodeAsync();
                }
                result = await ExecuteOpcodesAsync(new BytecodeFrame(call, accountStorage, ResourceLimits), new ZeroPaddedData(byteCode.ByteCode));
            }
        }

        if(!result.IsSuccess)
        {
            _storage.Reset(callSnapshot);
        }
        if(result.IsExceptionalHalt(out _))
        {
            call.Gas.ConsumeAll();
        }

        if(_executionState.Hooks is not null)
        {
            if(precompile is not null)
            {
                await _executionState.Hooks.OnPrecompileExitAsync(call, result, _storage);
            }
            else
            {
                await _executionState.Hooks.OnContractExitAsync(call, result, _storage);
            }
        }

        return result;
    }

    private async ValueTask<ExecutionResult> ExecuteContractCreationAsync(CallFrame call)
    {
        if(_executionState!.Hooks is not null)
        {
            await _executionState.Hooks.OnContractEnterAsync(call, _storage);
        }

        var result = await ExecuteContractCreationCoreAsync(call);
        if(result.IsExceptionalHalt(out _))
        {
            call.Gas.ConsumeAll();
        }
        if(_executionState.Hooks is not null)
        {
            await _executionState.Hooks.OnContractExitAsync(call, result, _storage);
        }

        return result;
    }

    private async ValueTask<ExecutionResult> ExecuteContractCreationCoreAsync(CallFrame call)
    {
        if(call.Depth > CallFrame.MAX_DEPTH)
        {
            return ExecutionResult.CallEntryFailure(CallEntryFailureReason.DepthExceeded);
        }

        var creatorStorage = _storage.GetAccountStorage(call.From);
        var creatorBalance = UInt256.Zero;
        if(!call.Value.IsZero)
        {
            creatorBalance = await creatorStorage.GetBalanceAsync();
            if(creatorBalance < call.Value)
            {
                return ExecutionResult.CallEntryFailure(CallEntryFailureReason.InsufficientBalance);
            }
        }

        ulong creatorNonce = await creatorStorage.GetNonceAsync();
        if(creatorNonce == UInt64.MaxValue)
        {
            return ExecutionResult.CallEntryFailure(CallEntryFailureReason.CreatorNonceOverflow);
        }

        creatorStorage.SetNonce(creatorNonce + 1);
        var createdStorage = _storage.GetAccountStorage(call.Address);
        if(await createdStorage.HasCreateCollisionAsync())
        {
            return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.ContractAddressCollision);
        }

        var creationSnapshot = _storage.TakeSnapshot();
        createdStorage.InitializeCreatedContract();
        if(!call.Value.IsZero)
        {
            var createdBalance = await createdStorage.GetBalanceAsync();
            creatorStorage.SetBalance(creatorBalance - call.Value);
            createdStorage.SetBalance(createdBalance + call.Value);
        }

        var result = await ExecuteOpcodesAsync(
            new BytecodeFrame(call, createdStorage, ResourceLimits),
            new ZeroPaddedData(call.Input)
        );

        result = result switch
        {
            { IsSuccess: true } when result.Data.Length > ExecutionSpec.MaxRuntimeCodeLength
                => ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.RuntimeCodeTooLarge),
            { IsSuccess: true } when !result.Data.IsEmpty && result.Data.Span[0] == 0xEF
                => ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.InvalidRuntimeCode),
            _ => result
        };

        if(!result.IsSuccess)
        {
            _storage.Reset(creationSnapshot);
            return result;
        }

        createdStorage.SetCode(new EVMByteCode(result.Data.ToArray()));
        return result;
    }
}
