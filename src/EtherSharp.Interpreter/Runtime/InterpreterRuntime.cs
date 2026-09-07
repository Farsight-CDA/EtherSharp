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
/// Executes EVM transactions and call simulations against an interpreter state fork.
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
    private readonly IInterpreterHost _host;
    private readonly FrozenDictionary<Address, IPrecompile> _precompiles;
    private readonly InterpreterContext _context;
    private ExecutionState? _executionState;
    private bool _isDisposed;

    internal InterpreterRuntime(
        InterpreterContext context,
        IInterpreterHost host,
        InterpreterExecutionSpec executionSpec,
        InterpreterOptions options,
        FrozenDictionary<Address, IPrecompile> precompiles
    )
    {
        ExecutionSpec = executionSpec;
        Options = options;
        _context = context;
        _host = host;
        _storage = new InterpreterStorage(host);
        _precompiles = precompiles;
    }

    /// <summary>The interpreter resource limits.</summary>
    public InterpreterOptions Options { get; }
    /// <summary>The consensus rules used for execution.</summary>
    public InterpreterExecutionSpec ExecutionSpec { get; }

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
        _host.Unregister();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Executes a transaction from the supplied sender and retains its state changes.
    /// </summary>
    /// <remarks>The sender nonce is incremented even when EVM execution reverts.</remarks>
    public ValueTask<TxCallResult> ExecuteTransactionAsync(Address sender, LegacyTransaction transaction)
    {
        ArgumentNullException.ThrowIfNull(transaction);
        return transaction.ChainId != _context.ChainId
            ? throw new InvalidOperationException("Transaction chain ID does not match the execution context.")
            : ExecuteTopLevelAsync(sender, retainState: true, transaction: TransactionEnvironment.CreateForTransaction(sender, transaction, _context));
    }

    /// <inheritdoc cref="ExecuteTransactionAsync(Address, LegacyTransaction)"/>
    public ValueTask<TxCallResult> ExecuteTransactionAsync(Address sender, EIP1559Transaction transaction)
    {
        ArgumentNullException.ThrowIfNull(transaction);
        return transaction.ChainId != _context.ChainId
            ? throw new InvalidOperationException("Transaction chain ID does not match the execution context.")
            : ExecuteTopLevelAsync(sender, retainState: true, transaction: TransactionEnvironment.CreateForTransaction(sender, transaction, _context));
    }

    /// <summary>
    /// Simulates a transaction from the supplied sender and discards all state changes.
    /// </summary>
    /// <remarks>The sender nonce is incremented during execution, then restored with the other simulated state changes.</remarks>
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
                sender, retainState: false, transaction: TransactionEnvironment.CreateForTransaction(sender, transaction, _context), options: options
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
                sender, retainState: false, transaction: TransactionEnvironment.CreateForTransaction(sender, transaction, _context), options: options
            );
    }

    /// <summary>
    /// Simulates a call from the supplied sender and discards all state changes.
    /// </summary>
    /// <param name="sender">The caller exposed through <c>msg.sender</c>.</param>
    /// <param name="call">The destination, value, and calldata supplied to the call.</param>
    /// <param name="options">The simulation options.</param>
    /// <remarks>The call uses a zero gas price, an empty access list, and no blob hashes.</remarks>
    public ValueTask<TxCallResult> SimulateCallAsync(
        Address sender,
        ITxInput call,
        InterpreterSimulationOptions options = default
    )
    {
        ArgumentNullException.ThrowIfNull(call);
        return ExecuteTopLevelAsync(sender, retainState: false, call: call, options: options);
    }

    private async ValueTask<TxCallResult> ExecuteTopLevelAsync(
        Address sender,
        bool retainState,
        TransactionEnvironment? transaction = null,
        ITxInput? call = null,
        InterpreterSimulationOptions options = default
    )
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
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

            var environment = transaction ?? TransactionEnvironment.CreateForCall(
                sender,
                call!,
                await _storage.GetAccountStorage(sender).GetNonceAsync(),
                _context
            );

            execution.Transaction = environment;
            if(execution.Hooks is not null)
            {
                await execution.Hooks.OnExecutionStartAsync(_context, environment, _storage);
            }
            var senderStorage = _storage.GetAccountStorage(sender);
            ulong senderNonce = await senderStorage.GetNonceAsync();
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

            ExecutionResult result;
            if(environment.Input.To is Address target)
            {
                senderStorage.SetNonce(senderNonce + 1);
                result = await ExecuteMessageCallAsync(
                    new CallFrame(
                        checked(execution.NextFrameId++),
                        EvmOpcode.Call,
                        sender,
                        null,
                        sender,
                        target,
                        target,
                        environment.Input.Value,
                        environment.Input.Data
                    )
                );
            }
            else
            {
                if(environment.Input.Data.Length > ExecutionSpec.MaxInitCodeLength)
                {
                    throw new InvalidOperationException("Transaction initcode exceeds the configured limit.");
                }

                var createdAddress = Address.DeriveCreate(sender, senderNonce);
                result = await ExecuteContractCreationAsync(new CallFrame(
                    checked(execution.NextFrameId++),
                    EvmOpcode.Create,
                    sender,
                    null,
                    sender,
                    createdAddress,
                    createdAddress,
                    environment.Input.Value,
                    environment.Input.Data
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
        if(_executionState!.Hooks is not null)
        {
            await _executionState.Hooks.OnCallEnterAsync(call, _storage);
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
            if(_precompiles.TryGetValue(call.To, out var precompile))
            {
                result = await precompile.ExecuteAsync(_host, new PrecompileCall(
                    _context,
                    call.Origin,
                    call.Caller,
                    call.Address,
                    call.Value,
                    call.Input,
                    call.Depth,
                    call.IsStatic
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
                result = await ExecuteOpcodesAsync(new BytecodeFrame(call, accountStorage, Options), new ZeroPaddedData(byteCode.ByteCode));
            }
        }

        if(!result.IsSuccess)
        {
            _storage.Reset(callSnapshot);
        }

        if(_executionState.Hooks is not null)
        {
            await _executionState.Hooks.OnCallExitAsync(call, result, _storage);
        }

        return result;
    }

    private async ValueTask<ExecutionResult> ExecuteContractCreationAsync(CallFrame call)
    {
        if(_executionState!.Hooks is not null)
        {
            await _executionState.Hooks.OnCallEnterAsync(call, _storage);
        }

        var result = call.Depth > CallFrame.MAX_DEPTH
            ? ExecutionResult.CallEntryFailure(CallEntryFailureReason.DepthExceeded)
            : ExecutionResult.Success();
        var creatorStorage = _storage.GetAccountStorage(call.From);
        var creatorBalance = UInt256.Zero;
        ulong creatorNonce = 0;
        if(result.IsSuccess)
        {
            creatorBalance = await creatorStorage.GetBalanceAsync();
            if(creatorBalance < call.Value)
            {
                result = ExecutionResult.CallEntryFailure(CallEntryFailureReason.InsufficientBalance);
            }
            else
            {
                creatorNonce = await creatorStorage.GetNonceAsync();
                if(creatorNonce == UInt64.MaxValue)
                {
                    result = ExecutionResult.CallEntryFailure(CallEntryFailureReason.CreatorNonceOverflow);
                }
            }
        }

        if(result.IsSuccess)
        {
            creatorStorage.SetNonce(creatorNonce + 1);
            var createdStorage = _storage.GetAccountStorage(call.Address);
            if(await createdStorage.HasCreateCollisionAsync())
            {
                result = ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.ContractAddressCollision);
            }
            else
            {
                var creationSnapshot = _storage.TakeSnapshot();
                createdStorage.InitializeCreatedContract();
                if(!call.Value.IsZero)
                {
                    var createdBalance = await createdStorage.GetBalanceAsync();
                    creatorStorage.SetBalance(creatorBalance - call.Value);
                    createdStorage.SetBalance(createdBalance + call.Value);
                }

                result = await ExecuteOpcodesAsync(
                    new BytecodeFrame(call, createdStorage, Options),
                    new ZeroPaddedData(call.Input)
                );
                if(!result.IsSuccess)
                {
                    _storage.Reset(creationSnapshot);
                }
                else if(result.Data.Length > ExecutionSpec.MaxRuntimeCodeLength)
                {
                    _storage.Reset(creationSnapshot);
                    result = ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.RuntimeCodeTooLarge);
                }
                else if(!result.Data.IsEmpty && result.Data.Span[0] == 0xEF)
                {
                    _storage.Reset(creationSnapshot);
                    result = ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.InvalidRuntimeCode);
                }
                else
                {
                    var runtimeCode = new EVMByteCode(result.Data.ToArray());
                    createdStorage.SetCode(in runtimeCode);
                }
            }
        }

        if(_executionState.Hooks is not null)
        {
            await _executionState.Hooks.OnCallExitAsync(call, result, _storage);
        }

        return result;
    }
}
