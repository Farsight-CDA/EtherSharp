using EtherSharp.Common.Exceptions;
using EtherSharp.Contract;
using EtherSharp.Interpreter.Forking;
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
/// Holds retained EVM state for structured execution against an interpreter state fork.
/// </summary>
/// <remarks>
/// Use the owning state fork's structured run APIs to execute this interpreter. Cloning must not run
/// concurrently with a structured lane using this interpreter.
/// </remarks>
internal sealed partial class InterpreterRuntime : IInterpreter, IInterpreterLane
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

    /// <summary>The interpreter resource limits.</summary>
    public InterpreterResourceLimits ResourceLimits { get; }
    /// <summary>The consensus rules used for execution.</summary>
    public InterpreterExecutionSpec ExecutionSpec { get; }

    internal InterpreterRuntime(
        InterpreterStateFork fork,
        InterpreterContext context,
        InterpreterExecutionSpec executionSpec,
        InterpreterResourceLimits resourceLimits,
        FrozenDictionary<Address, IPrecompile> precompiles
    )
    {
        ExecutionSpec = executionSpec;
        ResourceLimits = resourceLimits;
        Fork = fork;
        _context = context;
        _host = new ForkInterpreterHost(this);
        _storage = new InterpreterStorage(this);
        _precompiles = precompiles;
    }

    internal InterpreterRuntime Clone()
    {
        var clone = new InterpreterRuntime(Fork, _context, ExecutionSpec, ResourceLimits, _precompiles)
        {
            _interruptionCount = InterruptionCount
        };
        _storage.CopyTo(clone._storage);
        return clone;
    }

    /// <summary>
    /// Executes a transaction from the supplied sender and retains its state changes, including supplied state overrides.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when the transaction is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the nonce handling mode is invalid.</exception>
    /// <exception cref="InvalidOperationException">Thrown when transaction validation fails or an execution is already in progress.</exception>
    public ValueTask<TxCallResult> ExecuteTransactionAsync(
        Address sender,
        LegacyTransaction transaction,
        InterpreterExecutionOptions options = default
    )
    {
        ArgumentNullException.ThrowIfNull(transaction);
        return transaction.ChainId != _context.ChainId
            ? throw new InvalidOperationException("Transaction chain ID does not match the execution context.")
            : ExecuteTopLevelAsync(
                TransactionEnvironment.CreateForTransaction(sender, transaction, _context),
                hasExplicitNonce: true,
                retainState: true,
                options: options
            );
    }

    /// <inheritdoc cref="ExecuteTransactionAsync(Address, LegacyTransaction, InterpreterExecutionOptions)"/>
    public ValueTask<TxCallResult> ExecuteTransactionAsync(
        Address sender,
        EIP1559Transaction transaction,
        InterpreterExecutionOptions options = default
    )
    {
        ArgumentNullException.ThrowIfNull(transaction);
        return transaction.ChainId != _context.ChainId
            ? throw new InvalidOperationException("Transaction chain ID does not match the execution context.")
            : ExecuteTopLevelAsync(
                TransactionEnvironment.CreateForTransaction(sender, transaction, _context),
                hasExplicitNonce: true,
                retainState: true,
                options: options
            );
    }

    /// <summary>
    /// Executes a call from the supplied sender and retains its state changes, including supplied state overrides.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when the call is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the nonce handling mode is invalid.</exception>
    /// <exception cref="InvalidOperationException">Thrown when call validation fails or an execution is already in progress.</exception>
    public ValueTask<TxCallResult> ExecuteCallAsync(
        Address sender,
        ITxInput call,
        InterpreterExecutionOptions options = default
    )
    {
        ArgumentNullException.ThrowIfNull(call);
        return ExecuteTopLevelAsync(
            TransactionEnvironment.CreateForCall(sender, call, 0, _context),
            hasExplicitNonce: false,
            retainState: true,
            options: options
        );
    }

    /// <inheritdoc cref="ExecuteCallAsync(Address, ITxInput, InterpreterExecutionOptions)"/>
    /// <exception cref="CallRevertedException">Thrown when execution reverts.</exception>
    /// <exception cref="CallParsingException">Thrown when the return data cannot be decoded.</exception>
    public async ValueTask<T> ExecuteCallAsync<T>(
        Address sender,
        ITxInput<T> call,
        InterpreterExecutionOptions options = default
    )
    {
        var result = await SafeExecuteCallAsync(sender, call, options);
        return result.Unwrap();
    }

    /// <inheritdoc cref="ExecuteCallAsync(Address, ITxInput, InterpreterExecutionOptions)"/>
    public async ValueTask<CallResult<T>> SafeExecuteCallAsync<T>(
        Address sender,
        ITxInput<T> call,
        InterpreterExecutionOptions options = default
    )
    {
        var result = await ExecuteCallAsync(sender, (ITxInput) call, options);
        return CallResult<T>.ParseFrom(result, call.To, call.ReadResultFrom);
    }

    /// <summary>
    /// Simulates a transaction from the supplied sender and discards all state changes.
    /// </summary>
    public ValueTask<TxCallResult> SimulateTransactionAsync(
        Address sender,
        LegacyTransaction transaction,
        InterpreterExecutionOptions options = default
    )
    {
        ArgumentNullException.ThrowIfNull(transaction);
        return transaction.ChainId != _context.ChainId
            ? throw new InvalidOperationException("Transaction chain ID does not match the execution context.")
            : ExecuteTopLevelAsync(
                TransactionEnvironment.CreateForTransaction(sender, transaction, _context),
                hasExplicitNonce: true,
                retainState: false,
                options: options
            );
    }

    /// <inheritdoc cref="SimulateTransactionAsync(Address, LegacyTransaction, InterpreterExecutionOptions)"/>
    public ValueTask<TxCallResult> SimulateTransactionAsync(
        Address sender,
        EIP1559Transaction transaction,
        InterpreterExecutionOptions options = default
    )
    {
        ArgumentNullException.ThrowIfNull(transaction);
        return transaction.ChainId != _context.ChainId
            ? throw new InvalidOperationException("Transaction chain ID does not match the execution context.")
            : ExecuteTopLevelAsync(
                TransactionEnvironment.CreateForTransaction(sender, transaction, _context),
                hasExplicitNonce: true,
                retainState: false,
                options: options
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
        InterpreterExecutionOptions options = default
    )
    {
        ArgumentNullException.ThrowIfNull(call);
        return ExecuteTopLevelAsync(
            TransactionEnvironment.CreateForCall(sender, call, 0, _context),
            hasExplicitNonce: false,
            retainState: false,
            options: options
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
        InterpreterExecutionOptions options = default
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
        InterpreterExecutionOptions options = default
    )
    {
        var result = await SimulateCallAsync(sender, (ITxInput) call, options);
        return CallResult<T>.ParseFrom(result, call.To, call.ReadResultFrom);
    }

    private async ValueTask<TxCallResult> ExecuteTopLevelAsync(
        TransactionEnvironment environment,
        bool hasExplicitNonce,
        bool retainState,
        InterpreterExecutionOptions options = default
    )
    {
        bool skipTopLevelNonceChecks = options.TopLevelNonceHandling switch
        {
            TopLevelNonceHandling.Default => !hasExplicitNonce && environment.Input.To is not null,
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

            var (senderBalance, senderNonce, targetBalance, byteCode) = await _storage.GetAsync(
                !environment.Input.Value.IsZero
                    ? StateRequest.Balance(environment.Sender)
                    : StateRequest.Default<UInt256>(),
                !skipTopLevelNonceChecks
                    ? StateRequest.Nonce(environment.Sender)
                    : StateRequest.Default<ulong>(),
                environment.Input.To is { } balanceTarget && !environment.Input.Value.IsZero
                    ? StateRequest.Balance(balanceTarget)
                    : StateRequest.Default<UInt256>(),
                environment.Input.To is { } codeTarget && !_precompiles.ContainsKey(codeTarget)
                    ? StateRequest.Code(codeTarget)
                    : StateRequest.Default<EVMByteCode>()
            );

            if(!hasExplicitNonce && !skipTopLevelNonceChecks)
            {
                environment = environment with
                {
                    Nonce = senderNonce
                };
            }

            execution.Transaction = environment;
            if(execution.Hooks is not null)
            {
                await execution.Hooks.OnExecutionStartAsync(_context, environment, _storage);
            }

            if(!skipTopLevelNonceChecks)
            {
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
            if(environment.Input.To is { } messageTarget)
            {
                if(!skipTopLevelNonceChecks)
                {
                    _storage
                        .GetAccountStorage(environment.Sender)
                        .SetNonce(senderNonce + 1);
                }
                result = await ExecuteMessageCallAsync(
                    CallFrame.CreateTopLevelMessageCall(
                        checked(execution.NextFrameId++),
                        environment,
                        messageTarget
                    ),
                    senderBalance,
                    targetBalance,
                    byteCode
                );
            }
            else
            {
                if(environment.Input.Data.Length > ExecutionSpec.MaxInitCodeLength)
                {
                    throw new InvalidOperationException("Transaction initcode exceeds the configured limit.");
                }

                var createdAddress = Address.DeriveCreate(environment.Sender, senderNonce);
                result = await ExecuteContractCreationAsync(
                    CallFrame.CreateTopLevelContractCreation(
                        checked(execution.NextFrameId++),
                        environment,
                        createdAddress
                    ),
                    senderBalance,
                    senderNonce
                );
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
        bool transfersValue = !call.Value.IsZero && call.Type is EvmOpcode.Call or EvmOpcode.CallCode;
        var (sourceBalance, targetBalance, byteCode) = await _storage.GetAsync(
            transfersValue ? StateRequest.Balance(call.From) : StateRequest.Default<UInt256>(),
            transfersValue ? StateRequest.Balance(call.Address) : StateRequest.Default<UInt256>(),
            !_precompiles.ContainsKey(call.To)
                ? StateRequest.Code(call.To)
                : StateRequest.Default<EVMByteCode>()
        );
        return await ExecuteMessageCallAsync(
            call,
            sourceBalance,
            targetBalance,
            byteCode
        );
    }

    private async ValueTask<ExecutionResult> ExecuteMessageCallAsync(
        CallFrame call,
        UInt256 sourceBalance,
        UInt256 targetBalance,
        EVMByteCode byteCode
    )
    {
        _precompiles.TryGetValue(call.To, out var precompile);
        if(_executionState!.Hooks is not null)
        {
            await _executionState.Hooks.OnContractEnterAsync(call, _storage);
        }

        var accountStorage = _storage.GetAccountStorage(call.Address);
        var callSnapshot = _storage.TakeSnapshot();
        var result = await ExecuteMessageCallCoreAsync(
            call,
            accountStorage,
            precompile,
            sourceBalance,
            targetBalance,
            byteCode
        );

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
            await _executionState.Hooks.OnContractExitAsync(call, result, _storage);
        }

        return result;
    }

    private async ValueTask<ExecutionResult> ExecuteMessageCallCoreAsync(
        CallFrame call,
        InterpreterAccountStorage accountStorage,
        IPrecompile? precompile,
        UInt256 sourceBalance,
        UInt256 targetBalance,
        EVMByteCode byteCode
    )
    {
        if(call.Depth > CallFrame.MAX_DEPTH)
        {
            return ExecutionResult.CallEntryFailure(CallEntryFailureReason.DepthExceeded);
        }

        bool transfersValue = !call.Value.IsZero && call.Type is EvmOpcode.Call or EvmOpcode.CallCode;
        if(transfersValue)
        {
            if(sourceBalance < call.Value)
            {
                return ExecutionResult.CallEntryFailure(CallEntryFailureReason.InsufficientBalance);
            }
            if(call.From != call.Address)
            {
                var sourceStorage = _storage.GetAccountStorage(call.From);
                sourceStorage.SetBalance(sourceBalance - call.Value);
                accountStorage.SetBalance(targetBalance + call.Value);
            }
        }

        if(precompile is not null)
        {
            return await precompile.ExecuteAsync(this, new PrecompileCall(
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

        // EIP-7702 delegation: load the target's code without following further delegations.
        if(byteCode.Length == 3 + Address.BYTES_LENGTH
            && byteCode.ByteCode.Span[0] == 0xEF
            && byteCode.ByteCode.Span[1] == 0x01
            && byteCode.ByteCode.Span[2] == 0x00)
        {
            var delegationTarget = Address.FromBytes(byteCode.ByteCode.Span[3..]);
            byteCode = await _storage.GetAsync(StateRequest.Code(delegationTarget));
        }
        return await ExecuteOpcodesAsync(
            new BytecodeFrame(call, accountStorage, ResourceLimits, ExecutionSpec),
            new ZeroPaddedData(byteCode.ByteCode)
        );
    }

    private async ValueTask<ExecutionResult> ExecuteContractCreationAsync(
        CallFrame call,
        UInt256 creatorBalance,
        ulong creatorNonce
    )
    {
        if(_executionState!.Hooks is not null)
        {
            await _executionState.Hooks.OnContractEnterAsync(call, _storage);
        }

        var result = await ExecuteContractCreationCoreAsync(call, creatorBalance, creatorNonce);
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

    private async ValueTask<ExecutionResult> ExecuteContractCreationCoreAsync(
        CallFrame call,
        UInt256 creatorBalance,
        ulong creatorNonce
    )
    {
        if(call.Depth > CallFrame.MAX_DEPTH)
        {
            return ExecutionResult.CallEntryFailure(CallEntryFailureReason.DepthExceeded);
        }

        var creatorStorage = _storage.GetAccountStorage(call.From);
        if(creatorBalance < call.Value)
        {
            return ExecutionResult.CallEntryFailure(CallEntryFailureReason.InsufficientBalance);
        }
        if(creatorNonce == UInt64.MaxValue)
        {
            return ExecutionResult.CallEntryFailure(CallEntryFailureReason.CreatorNonceOverflow);
        }

        creatorStorage.SetNonce(creatorNonce + 1);
        var createdStorage = _storage.GetAccountStorage(call.Address);
        var (createdNonce, createdCodeHash, createdBalance) = await _storage.GetAsync(
            StateRequest.Nonce(call.Address),
            StateRequest.CodeHash(call.Address),
            call.Value.IsZero
                ? StateRequest.Default<UInt256>()
                : StateRequest.Balance(call.Address)
        );
        if(createdNonce != 0
            || (createdCodeHash is not null && createdCodeHash.Value != Bytes32.EmptyCodeHash))
        {
            return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.ContractAddressCollision);
        }

        var creationSnapshot = _storage.TakeSnapshot();
        createdStorage.InitializeCreatedContract();
        if(!call.Value.IsZero)
        {
            creatorStorage.SetBalance(creatorBalance - call.Value);
            createdStorage.SetBalance(createdBalance + call.Value);
        }

        var result = await ExecuteOpcodesAsync(
            new BytecodeFrame(call, createdStorage, ResourceLimits, ExecutionSpec),
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
