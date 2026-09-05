using EtherSharp.Contract;
using EtherSharp.Crypto;
using EtherSharp.Interpreter.Runtime.ExecutionSpecs;
using EtherSharp.Interpreter.Runtime.Memory;
using EtherSharp.Interpreter.Runtime.Precompiles;
using EtherSharp.Interpreter.Runtime.Storage;
using EtherSharp.Numerics;
using EtherSharp.Tx;
using EtherSharp.Tx.Types;
using EtherSharp.Types;
using System.Collections.Frozen;
using System.Diagnostics;

namespace EtherSharp.Interpreter.Runtime;

/// <summary>
/// Executes EVM transactions and call simulations against an interpreter state fork.
/// </summary>
/// <remarks>
/// Instances are not thread-safe. Await each operation before starting another operation or disposing
/// the runtime. Concurrent use of a single runtime is unsupported and is not guarded at runtime.
/// </remarks>
public class InterpreterRuntime : IDisposable
{
    private readonly record struct MessageCall(
        Address Origin,
        Address Caller,
        Address Address,
        Address CodeAddress,
        Address? ValueSource,
        UInt256 Value,
        ReadOnlyMemory<byte> Input,
        int Depth,
        bool IsStatic
    );

    private readonly record struct ContractCreation(
        Address Origin,
        Address Creator,
        Address CreatedAddress,
        UInt256 Endowment,
        ReadOnlyMemory<byte> InitCode,
        int Depth
    );

    private readonly InterpreterStorage _storage;
    private readonly IInterpreterHost _host;
    private readonly FrozenDictionary<Address, IPrecompile> _precompiles;
    private readonly InterpreterContext _context;
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

    /// <summary>
    /// Gets the interpreter configuration.
    /// </summary>
    public InterpreterOptions Options { get; }
    /// <summary>
    /// Gets the consensus behavior used for execution.
    /// </summary>
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
    /// <param name="sender">The transaction sender.</param>
    /// <param name="transaction">The unsigned transaction payload.</param>
    /// <returns>The transaction execution result.</returns>
    /// <remarks>The sender nonce is incremented even when EVM execution reverts.</remarks>
    public async ValueTask<TxCallResult> ExecuteTransactionAsync(
        Address sender,
        ITransaction transaction
    )
    {
        ThrowIfDisposed();
        var storageSnapshot = _storage.TakeSnapshot();
        try
        {
            var environment = TransactionEnvironment.CreateFrom(sender, transaction, _context);
            var result = await ExecuteTopLevelAsync(environment);
            _storage.Commit();
            return new TxCallResult(result.IsSuccess, result.Data);
        }
        catch
        {
            _storage.Reset(storageSnapshot);
            throw;
        }
    }

    /// <summary>
    /// Simulates a transaction from the supplied sender and discards all state changes.
    /// </summary>
    /// <param name="sender">The transaction sender.</param>
    /// <param name="transaction">The unsigned transaction payload.</param>
    /// <param name="options">The simulation options.</param>
    /// <returns>The simulated call result.</returns>
    /// <remarks>The sender nonce is incremented during execution, then restored with the other simulated state changes.</remarks>
    public async ValueTask<TxCallResult> SimulateTransactionAsync(
        Address sender,
        ITransaction transaction,
        InterpreterSimulationOptions options = default
    )
    {
        ThrowIfDisposed();
        var storageSnapshot = _storage.TakeSnapshot();
        try
        {
            if(options.StateOverrides is { } stateOverrides)
            {
                _storage.ApplyStateOverrides(stateOverrides);
            }

            var environment = TransactionEnvironment.CreateFrom(sender, transaction, _context);
            var result = await ExecuteTopLevelAsync(environment);
            return new TxCallResult(result.IsSuccess, result.Data);
        }
        finally
        {
            _storage.Reset(storageSnapshot);
        }
    }

    /// <summary>
    /// Simulates a call from the supplied sender and discards all state changes.
    /// </summary>
    /// <param name="sender">The caller exposed through <c>msg.sender</c>.</param>
    /// <param name="call">The destination, value, and calldata supplied to the call.</param>
    /// <param name="options">The simulation options.</param>
    /// <returns>The simulated call result.</returns>
    /// <remarks>The call uses a zero gas price, an empty access list, and no blob hashes.</remarks>
    public async ValueTask<TxCallResult> SimulateCallAsync(
        Address sender,
        ITxInput call,
        InterpreterSimulationOptions options = default
    )
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(call);

        var storageSnapshot = _storage.TakeSnapshot();
        try
        {
            if(options.StateOverrides is { } stateOverrides)
            {
                _storage.ApplyStateOverrides(stateOverrides);
            }

            ulong nonce = await _storage.GetAccountStorage(sender).GetNonceAsync();
            var environment = new TransactionEnvironment(
                sender,
                nonce,
                (ulong) _context.GasLimit,
                UInt256.Zero,
                call,
                [],
                []
            );
            var result = await ExecuteTopLevelAsync(environment);
            return new TxCallResult(result.IsSuccess, result.Data);
        }
        finally
        {
            _storage.Reset(storageSnapshot);
        }
    }

    private void ThrowIfDisposed()
        => ObjectDisposedException.ThrowIf(_isDisposed, this);

    private async ValueTask<ExecutionResult> ExecuteTopLevelAsync(
        TransactionEnvironment transaction
    )
    {
        var senderStorage = _storage.GetAccountStorage(transaction.Sender);
        ulong senderNonce = await senderStorage.GetNonceAsync();
        if(senderNonce != transaction.Nonce)
        {
            throw new InvalidOperationException(
                $"Invalid transaction nonce. Expected {senderNonce}, received {transaction.Nonce}."
            );
        }
        if(senderNonce == UInt64.MaxValue)
        {
            throw new InvalidOperationException("Transaction sender nonce cannot be incremented.");
        }

        if(transaction.Input.To is Address target)
        {
            senderStorage.SetNonce(senderNonce + 1);
            return await ExecuteMessageCallAsync(transaction, new MessageCall(
                transaction.Sender,
                transaction.Sender,
                target,
                target,
                transaction.Sender,
                transaction.Input.Value,
                transaction.Input.Data,
                0,
                false
            ));
        }

        if(transaction.Input.Data.Length > ExecutionSpec.MaxInitCodeLength)
        {
            throw new InvalidOperationException("Transaction initcode exceeds the configured limit.");
        }

        var createdAddress = Address.DeriveCreate(transaction.Sender, senderNonce);
        return await ExecuteContractCreationAsync(transaction, new ContractCreation(
            transaction.Sender,
            transaction.Sender,
            createdAddress,
            transaction.Input.Value,
            transaction.Input.Data,
            0
        ));
    }

    private async ValueTask<ExecutionResult> ExecuteMessageCallAsync(
        TransactionEnvironment transaction,
        MessageCall messageCall
    )
    {
        if(messageCall.Depth > CallFrame.MAX_DEPTH)
        {
            return ExecutionResult.CallEntryFailure(CallEntryFailureReason.DepthExceeded);
        }

        var accountStorage = _storage.GetAccountStorage(messageCall.Address);
        var callSnapshot = _storage.TakeSnapshot();

        if(messageCall.ValueSource is Address valueSource
            && messageCall.Value != UInt256.Zero)
        {
            var valueSourceStorage = valueSource == messageCall.Address
                ? accountStorage
                : _storage.GetAccountStorage(valueSource);
            var sourceBalance = await valueSourceStorage.GetBalanceAsync();
            if(sourceBalance < messageCall.Value)
            {
                return ExecutionResult.CallEntryFailure(CallEntryFailureReason.InsufficientBalance);
            }

            if(valueSource != messageCall.Address)
            {
                var targetBalance = await accountStorage.GetBalanceAsync();
                valueSourceStorage.SetBalance(sourceBalance - messageCall.Value);
                accountStorage.SetBalance(targetBalance + messageCall.Value);
            }
        }

        ExecutionResult result;
        if(_precompiles.TryGetValue(messageCall.CodeAddress, out var precompile))
        {
            var precompileResult = await precompile.ExecuteAsync(
                _host,
                new PrecompileCall(
                    _context,
                    messageCall.Origin,
                    messageCall.Caller,
                    messageCall.Address,
                    messageCall.Value,
                    messageCall.Input,
                    messageCall.Depth,
                    messageCall.IsStatic
                )
            );
            // Native precompile failure is exceptional, not execution of the REVERT opcode.
            result = precompileResult.Success
                ? ExecutionResult.Success(precompileResult.Data)
                : ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.PrecompileFailure);
        }
        else
        {
            var callFrame = new CallFrame(
                messageCall.Origin,
                messageCall.Caller,
                messageCall.Address,
                messageCall.CodeAddress,
                messageCall.Value,
                messageCall.Input,
                accountStorage,
                Options,
                messageCall.Depth,
                messageCall.IsStatic
            );

            var codeStorage = callFrame.CodeAddress == callFrame.To
                ? callFrame.AccountStorage
                : _storage.GetAccountStorage(callFrame.CodeAddress);
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
            result = await ExecuteOpcodesAsync(transaction, callFrame, byteCode);
        }

        if(!result.IsSuccess)
        {
            _storage.Reset(callSnapshot);
        }

        return result;
    }

    private async ValueTask<ExecutionResult> ExecuteContractCreationAsync(
        TransactionEnvironment transaction,
        ContractCreation creation
    )
    {
        if(creation.Depth > CallFrame.MAX_DEPTH)
        {
            return ExecutionResult.CallEntryFailure(CallEntryFailureReason.DepthExceeded);
        }

        var creatorStorage = _storage.GetAccountStorage(creation.Creator);
        var creatorBalance = await creatorStorage.GetBalanceAsync();
        if(creatorBalance < creation.Endowment)
        {
            return ExecutionResult.CallEntryFailure(CallEntryFailureReason.InsufficientBalance);
        }

        ulong creatorNonce = await creatorStorage.GetNonceAsync();
        if(creatorNonce == UInt64.MaxValue)
        {
            return ExecutionResult.CallEntryFailure(CallEntryFailureReason.CreatorNonceOverflow);
        }

        creatorStorage.SetNonce(creatorNonce + 1);
        var createdStorage = _storage.GetAccountStorage(creation.CreatedAddress);
        if(await createdStorage.HasCreateCollisionAsync())
        {
            return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.ContractAddressCollision);
        }

        var creationSnapshot = _storage.TakeSnapshot();
        createdStorage.InitializeCreatedContract();
        if(creation.Endowment != UInt256.Zero)
        {
            var createdBalance = await createdStorage.GetBalanceAsync();
            creatorStorage.SetBalance(creatorBalance - creation.Endowment);
            createdStorage.SetBalance(createdBalance + creation.Endowment);
        }

        var creationFrame = new CallFrame(
            creation.Origin,
            creation.Creator,
            creation.CreatedAddress,
            creation.CreatedAddress,
            creation.Endowment,
            ReadOnlyMemory<byte>.Empty,
            createdStorage,
            Options,
            creation.Depth,
            false
        );
        var creationResult = await ExecuteOpcodesAsync(
            transaction,
            creationFrame,
            new EVMByteCode(creation.InitCode)
        );
        if(!creationResult.IsSuccess)
        {
            _storage.Reset(creationSnapshot);
            return creationResult;
        }
        if(creationResult.Data.Length > ExecutionSpec.MaxRuntimeCodeLength)
        {
            _storage.Reset(creationSnapshot);
            return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.RuntimeCodeTooLarge);
        }
        if(!creationResult.Data.IsEmpty && creationResult.Data.Span[0] == 0xEF)
        {
            _storage.Reset(creationSnapshot);
            return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.InvalidRuntimeCode);
        }

        var runtimeCode = new EVMByteCode(creationResult.Data.ToArray());
        createdStorage.SetCode(in runtimeCode);
        return creationResult;
    }

    private async ValueTask<ExecutionResult> ExecuteOpcodesAsync(
        TransactionEnvironment transaction,
        CallFrame callFrame,
        EVMByteCode byteCode
    )
    {
        var code = new ZeroPaddedData(byteCode.ByteCode);
        int programCounter = 0;

        while(true)
        {
            var opcode = (EvmOpcode) code[programCounter];

            switch(opcode)
            {
                case EvmOpcode.Stop:
                    return ExecutionResult.Success();
                case >= EvmOpcode.Add and <= EvmOpcode.SMod:
                {
                    if(!callFrame.Stack.TryPop(out UInt256 first, out UInt256 second))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackUnderflow);
                    }
                    if(opcode is EvmOpcode.Div or EvmOpcode.SDiv or EvmOpcode.Mod or EvmOpcode.SMod
                        && second == UInt256.Zero)
                    {
                        callFrame.Stack.Push(Bytes32.Zero);
                        break;
                    }

                    callFrame.Stack.Push(opcode switch
                    {
                        EvmOpcode.Add => first + second,
                        EvmOpcode.Mul => first * second,
                        EvmOpcode.Sub => first - second,
                        EvmOpcode.Div => first / second,
                        EvmOpcode.SDiv => (UInt256) ((Int256) first / (Int256) second),
                        EvmOpcode.Mod => first % second,
                        EvmOpcode.SMod => (UInt256) ((Int256) first % (Int256) second),
                        _ => throw new UnreachableException()
                    });
                    break;
                }
                case >= EvmOpcode.AddMod and <= EvmOpcode.MulMod:
                {
                    if(!callFrame.Stack.TryPop(out UInt256 first, out UInt256 second, out UInt256 modulus))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackUnderflow);
                    }
                    if(modulus == UInt256.Zero)
                    {
                        callFrame.Stack.Push(Bytes32.Zero);
                        break;
                    }

                    UInt256 result;
                    switch(opcode)
                    {
                        case EvmOpcode.AddMod:
                            UInt256.AddMod(
                                first,
                                second,
                                modulus,
                                out result
                            );
                            break;
                        case EvmOpcode.MulMod:
                            UInt256.MultiplyMod(
                                first,
                                second,
                                modulus,
                                out result
                            );
                            break;
                        default:
                            throw new UnreachableException();
                    }

                    callFrame.Stack.Push(in result);
                    break;
                }
                case EvmOpcode.Exp:
                {
                    if(!callFrame.Stack.TryPop(out UInt256 first, out UInt256 second))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackUnderflow);
                    }

                    callFrame.Stack.Push(UInt256.Pow(first, second));
                    break;
                }
                case EvmOpcode.SignExtend:
                {
                    if(!callFrame.Stack.TryPop(out UInt256 byteIndex, out Int256 value))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackUnderflow);
                    }

                    if(byteIndex > 31)
                    {
                        callFrame.Stack.Push(in value);
                        break;
                    }

                    int shift = (31 - (int) byteIndex) * 8;
                    callFrame.Stack.Push((value << shift) >> shift);
                    break;
                }
                case >= EvmOpcode.Lt and <= EvmOpcode.Eq:
                case >= EvmOpcode.And and <= EvmOpcode.Xor:
                {
                    if(!callFrame.Stack.TryPop(out UInt256 first, out UInt256 second))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackUnderflow);
                    }

                    callFrame.Stack.Push(opcode switch
                    {
                        EvmOpcode.Lt => first < second ? UInt256.One : UInt256.Zero,
                        EvmOpcode.Gt => first > second ? UInt256.One : UInt256.Zero,
                        EvmOpcode.SLt => (Int256) first < (Int256) second ? UInt256.One : UInt256.Zero,
                        EvmOpcode.SGt => (Int256) first > (Int256) second ? UInt256.One : UInt256.Zero,
                        EvmOpcode.Eq => first == second ? UInt256.One : UInt256.Zero,
                        EvmOpcode.And => first & second,
                        EvmOpcode.Or => first | second,
                        EvmOpcode.Xor => first ^ second,
                        _ => throw new UnreachableException()
                    });
                    break;
                }
                case EvmOpcode.IsZero or EvmOpcode.Not or EvmOpcode.Clz:
                {
                    if(!callFrame.Stack.TryPop(out UInt256 value))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackUnderflow);
                    }

                    callFrame.Stack.Push(opcode switch
                    {
                        EvmOpcode.IsZero => value == UInt256.Zero ? UInt256.One : UInt256.Zero,
                        EvmOpcode.Not => ~value,
                        EvmOpcode.Clz => (UInt256) UInt256.LeadingZeroCount(in value),
                        _ => throw new UnreachableException()
                    });
                    break;
                }
                case EvmOpcode.Byte:
                {
                    if(!callFrame.Stack.TryPop(out UInt256 byteIndex, out Bytes32 value))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackUnderflow);
                    }

                    callFrame.Stack.Push(byteIndex < Bytes32.BYTE_LENGTH
                        ? (UInt256) value[(int) byteIndex]
                        : UInt256.Zero
                    );
                    break;
                }
                case EvmOpcode.Shl or EvmOpcode.Shr:
                {
                    if(!callFrame.Stack.TryPop(out UInt256 shift, out UInt256 value))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackUnderflow);
                    }

                    if(shift >= 256)
                    {
                        callFrame.Stack.Push(UInt256.Zero);
                        break;
                    }

                    callFrame.Stack.Push(opcode switch
                    {
                        EvmOpcode.Shl => value << (int) shift,
                        EvmOpcode.Shr => value >> (int) shift,
                        _ => throw new UnreachableException()
                    });
                    break;
                }
                case EvmOpcode.Sar:
                {
                    if(!callFrame.Stack.TryPop(out UInt256 shift, out Int256 value))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackUnderflow);
                    }

                    callFrame.Stack.Push(shift >= 256
                        ? value.IsNegative ? UInt256.MaxValue : UInt256.Zero
                        : (UInt256) (value >> (int) shift));
                    break;
                }
                case EvmOpcode.Keccak256:
                {
                    if(!callFrame.Stack.TryPop(out UInt256 offset, out UInt256 length))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackUnderflow);
                    }

                    var data = callFrame.Memory.Access(offset, length);
                    callFrame.Stack.Push(Keccak256.HashData(data.Span));
                    break;
                }
                case EvmOpcode.Address:
                    if(!callFrame.Stack.TryPush(callFrame.To))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackOverflow);
                    }

                    break;
                case EvmOpcode.Balance:
                {
                    if(!callFrame.Stack.TryPop(out Address address))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackUnderflow);
                    }

                    callFrame.Stack.Push(await _storage.GetAccountStorage(address).GetBalanceAsync());
                    break;
                }
                case EvmOpcode.Origin:
                    if(!callFrame.Stack.TryPush(callFrame.Origin))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackOverflow);
                    }

                    break;
                case EvmOpcode.Caller:
                    if(!callFrame.Stack.TryPush(callFrame.From))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackOverflow);
                    }

                    break;
                case EvmOpcode.CallValue:
                    if(!callFrame.Stack.TryPush(callFrame.Value))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackOverflow);
                    }

                    break;
                case EvmOpcode.CallDataLoad:
                {
                    if(!callFrame.Stack.TryPop(out UInt256 offset))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackUnderflow);
                    }

                    if(offset >= (UInt256) callFrame.CallData.Length)
                    {
                        callFrame.Stack.Push(Bytes32.Zero);
                        break;
                    }

                    callFrame.Stack.Push(callFrame.CallData.ReadAtOffset((int) offset));
                    break;
                }
                case EvmOpcode.CallDataSize:
                    if(!callFrame.Stack.TryPush((UInt256) callFrame.CallData.Length))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackOverflow);
                    }

                    break;
                case EvmOpcode.CallDataCopy:
                {
                    if(!callFrame.Stack.TryPop(
                        out UInt256 destinationOffset,
                        out UInt256 sourceOffset,
                        out UInt256 length
                    ))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackUnderflow);
                    }

                    callFrame.CallData.CopyTo(
                        sourceOffset,
                        callFrame.Memory.Access(destinationOffset, length)
                    );
                    break;
                }
                case EvmOpcode.CodeSize:
                    if(!callFrame.Stack.TryPush((UInt256) code.Length))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackOverflow);
                    }

                    break;
                case EvmOpcode.CodeCopy:
                {
                    if(!callFrame.Stack.TryPop(
                        out UInt256 destinationOffset,
                        out UInt256 sourceOffset,
                        out UInt256 length
                    ))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackUnderflow);
                    }

                    code.CopyTo(sourceOffset, callFrame.Memory.Access(destinationOffset, length));
                    break;
                }
                case EvmOpcode.GasPrice:
                    if(!callFrame.Stack.TryPush(transaction.EffectiveGasPrice))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackOverflow);
                    }

                    break;
                case EvmOpcode.ExtCodeSize:
                {
                    if(!callFrame.Stack.TryPop(out Address address))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackUnderflow);
                    }

                    callFrame.Stack.Push((UInt256) (await _storage.GetAccountStorage(address).GetCodeAsync()).Length);
                    break;
                }
                case EvmOpcode.ExtCodeCopy:
                {
                    if(!callFrame.Stack.TryPop(
                        out Address address,
                        out UInt256 destinationOffset,
                        out UInt256 sourceOffset,
                        out UInt256 length
                    ))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackUnderflow);
                    }

                    var externalCode = new ZeroPaddedData((await _storage.GetAccountStorage(address).GetCodeAsync()).ByteCode);
                    externalCode.CopyTo(sourceOffset, callFrame.Memory.Access(destinationOffset, length));
                    break;
                }
                case EvmOpcode.ReturnDataSize:
                    if(!callFrame.Stack.TryPush((UInt256) callFrame.ReturnData.Length))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackOverflow);
                    }

                    break;
                case EvmOpcode.ReturnDataCopy:
                {
                    if(!callFrame.Stack.TryPop(
                        out UInt256 destinationOffset,
                        out UInt256 sourceOffset,
                        out UInt256 length
                    ))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackUnderflow);
                    }

                    if(!callFrame.ReturnData.TryCopyTo(
                        sourceOffset,
                        callFrame.Memory.Access(destinationOffset, length)
                    ))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.ReturnDataOutOfBounds);
                    }

                    break;
                }
                case EvmOpcode.ExtCodeHash:
                {
                    if(!callFrame.Stack.TryPop(out Address address))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackUnderflow);
                    }

                    callFrame.Stack.Push(await _storage.GetAccountStorage(address).GetExtCodeHashAsync());
                    break;
                }
                case EvmOpcode.BlockHash:
                {
                    if(!callFrame.Stack.TryPop(out UInt256 blockNumber))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackUnderflow);
                    }

                    if(blockNumber >= (UInt256) _context.BlockNumber)
                    {
                        callFrame.Stack.Push(Bytes32.Zero);
                        break;
                    }

                    var distance = (UInt256) _context.BlockNumber - blockNumber;
                    callFrame.Stack.Push(
                        distance <= 256 && distance <= (UInt256) _context.RecentBlockHashes.Length
                            ? _context.RecentBlockHashes[(int) distance - 1]
                            : Bytes32.Zero
                    );
                    break;
                }
                case EvmOpcode.Coinbase:
                    if(!callFrame.Stack.TryPush(_context.Coinbase))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackOverflow);
                    }

                    break;
                case EvmOpcode.Timestamp:
                    if(!callFrame.Stack.TryPush((UInt256) _context.BlockTimestamp.ToUnixTimeSeconds()))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackOverflow);
                    }

                    break;
                case EvmOpcode.Number:
                    if(!callFrame.Stack.TryPush((UInt256) _context.BlockNumber))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackOverflow);
                    }

                    break;
                case EvmOpcode.PrevRandao:
                    if(!callFrame.Stack.TryPush(_context.PrevRandao))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackOverflow);
                    }

                    break;
                case EvmOpcode.GasLimit:
                    if(!callFrame.Stack.TryPush(_context.GasLimit))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackOverflow);
                    }

                    break;
                case EvmOpcode.ChainId:
                    if(!callFrame.Stack.TryPush((UInt256) _context.ChainId))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackOverflow);
                    }

                    break;
                case EvmOpcode.SelfBalance:
                    if(!callFrame.Stack.TryPush(await callFrame.AccountStorage.GetBalanceAsync()))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackOverflow);
                    }

                    break;
                case EvmOpcode.BaseFee:
                    if(!_context.BaseFee.HasValue)
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.InvalidOpcode);
                    }

                    if(!callFrame.Stack.TryPush(_context.BaseFee.Value))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackOverflow);
                    }

                    break;
                case EvmOpcode.BlobHash:
                    if(!_context.BlobBaseFee.HasValue)
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.InvalidOpcode);
                    }
                    if(!callFrame.Stack.TryPop(out UInt256 blobIndex))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackUnderflow);
                    }

                    callFrame.Stack.Push(
                        blobIndex < (UInt256) transaction.BlobHashes.Length
                            ? transaction.BlobHashes[(int) blobIndex]
                            : Bytes32.Zero
                    );
                    break;
                case EvmOpcode.BlobBaseFee:
                    if(!_context.BlobBaseFee.HasValue)
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.InvalidOpcode);
                    }

                    if(!callFrame.Stack.TryPush(_context.BlobBaseFee.Value))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackOverflow);
                    }

                    break;
                case EvmOpcode.Pop:
                    if(!callFrame.Stack.TryPop(out Bytes32 _))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackUnderflow);
                    }

                    break;
                case EvmOpcode.MLoad:
                {
                    if(!callFrame.Stack.TryPop(out UInt256 offset))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackUnderflow);
                    }

                    callFrame.Stack.Push(Bytes32.FromBytes(
                        callFrame.Memory.Access(offset, Bytes32.BYTE_LENGTH).Span
                    ));
                    break;
                }
                case EvmOpcode.MStore:
                {
                    if(!callFrame.Stack.TryPop(out UInt256 offset, out Bytes32 value))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackUnderflow);
                    }

                    value.CopyTo(callFrame.Memory.Access(offset, Bytes32.BYTE_LENGTH).Span);
                    break;
                }
                case EvmOpcode.MStore8:
                {
                    if(!callFrame.Stack.TryPop(out UInt256 offset, out Bytes32 value))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackUnderflow);
                    }

                    callFrame.Memory.Access(offset, 1).Span[0] = value[^1];
                    break;
                }
                case EvmOpcode.SLoad:
                {
                    if(!callFrame.Stack.TryPop(out Bytes32 key))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackUnderflow);
                    }

                    callFrame.Stack.Push(await callFrame.AccountStorage.SLoadAsync(key));
                    break;
                }
                case EvmOpcode.SStore:
                {
                    if(callFrame.IsStatic)
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.WriteProtection);
                    }

                    if(!callFrame.Stack.TryPop(out Bytes32 key, out Bytes32 value))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackUnderflow);
                    }

                    callFrame.AccountStorage.SStore(in key, in value);
                    break;
                }
                case EvmOpcode.Jump:
                {
                    if(!callFrame.Stack.TryPop(out UInt256 destination))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackUnderflow);
                    }
                    if(!IsValidJumpDestination(code.Data.Span, destination))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.InvalidJumpDestination);
                    }

                    programCounter = (int) destination;
                    continue;
                }
                case EvmOpcode.JumpI:
                {
                    if(!callFrame.Stack.TryPop(out UInt256 destination, out UInt256 condition))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackUnderflow);
                    }
                    if(condition == UInt256.Zero)
                    {
                        break;
                    }
                    if(!IsValidJumpDestination(code.Data.Span, destination))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.InvalidJumpDestination);
                    }

                    programCounter = (int) destination;
                    continue;
                }
                case EvmOpcode.Pc:
                    if(!callFrame.Stack.TryPush((UInt256) programCounter))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackOverflow);
                    }

                    break;
                case EvmOpcode.MSize:
                    if(!callFrame.Stack.TryPush((UInt256) callFrame.Memory.Size))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackOverflow);
                    }

                    break;
                case EvmOpcode.Gas:
                    //ToDo: Gas tracking
                    if(!callFrame.Stack.TryPush(UInt256.MaxValue))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackOverflow);
                    }

                    break;
                case EvmOpcode.JumpDest:
                    break;
                case EvmOpcode.TLoad:
                {
                    if(!callFrame.Stack.TryPop(out Bytes32 key))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackUnderflow);
                    }

                    callFrame.Stack.Push(callFrame.AccountStorage.TLoad(in key));
                    break;
                }
                case EvmOpcode.TStore:
                {
                    if(callFrame.IsStatic)
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.WriteProtection);
                    }

                    if(!callFrame.Stack.TryPop(out Bytes32 key, out Bytes32 value))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackUnderflow);
                    }

                    callFrame.AccountStorage.TStore(in key, in value);
                    break;
                }
                case EvmOpcode.MCopy:
                {
                    if(!callFrame.Stack.TryPop(
                        out UInt256 destinationOffset,
                        out UInt256 sourceOffset,
                        out UInt256 length
                    ))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackUnderflow);
                    }

                    callFrame.Memory.Copy(destinationOffset, sourceOffset, length);
                    break;
                }
                case >= EvmOpcode.Push0 and <= EvmOpcode.Push32:
                {
                    int pushLength = (byte) opcode - (byte) EvmOpcode.Push0;
                    var value = pushLength == 0
                        ? Bytes32.Zero
                        : (Bytes32) (
                            (UInt256) code.ReadAtOffset(programCounter + 1)
                            >> ((Bytes32.BYTE_LENGTH - pushLength) * 8)
                        );
                    if(!callFrame.Stack.TryPush(in value))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackOverflow);
                    }

                    programCounter += pushLength;
                    break;
                }
                case >= EvmOpcode.Dup1 and <= EvmOpcode.Dup16:
                {
                    int depth = (byte) opcode - ((byte) EvmOpcode.Dup1 - 1);
                    if(!callFrame.Stack.TryDup(depth))
                    {
                        return ExecutionResult.ExceptionalHalt(callFrame.Stack.IsFull
                            ? ExceptionalHaltReason.StackOverflow
                            : ExceptionalHaltReason.StackUnderflow
                        );
                    }

                    break;
                }
                case >= EvmOpcode.Swap1 and <= EvmOpcode.Swap16:
                {
                    int depth = (byte) opcode - ((byte) EvmOpcode.Swap1 - 1);
                    if(!callFrame.Stack.TrySwap(depth))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackUnderflow);
                    }

                    break;
                }
                case >= EvmOpcode.Log0 and <= EvmOpcode.Log4:
                {
                    if(callFrame.IsStatic)
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.WriteProtection);
                    }

                    int topicCount = (byte) opcode - (byte) EvmOpcode.Log0;
                    if(!callFrame.Stack.TryPop(out UInt256 offset, out UInt256 length))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackUnderflow);
                    }

                    var topics = new Bytes32[topicCount];
                    for(int i = 0; i < topics.Length; i++)
                    {
                        if(!callFrame.Stack.TryPop(out topics[i]))
                        {
                            return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackUnderflow);
                        }
                    }

                    byte[] data = callFrame.Memory.Access(offset, length).Span.ToArray();
                    _storage.AddLog(callFrame.To, topics, data);
                    break;
                }
                case EvmOpcode.Create:
                {
                    if(callFrame.IsStatic)
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.WriteProtection);
                    }

                    if(!callFrame.Stack.TryPop(
                        out UInt256 endowment,
                        out UInt256 offset,
                        out UInt256 length
                    ))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackUnderflow);
                    }
                    if(length > (UInt256) ExecutionSpec.MaxInitCodeLength)
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.InitCodeTooLarge);
                    }

                    var createdAddress = Address.DeriveCreate(
                        callFrame.To,
                        await callFrame.AccountStorage.GetNonceAsync()
                    );
                    var creationResult = await ExecuteContractCreationAsync(
                        transaction,
                        new ContractCreation(
                            callFrame.Origin,
                            callFrame.To,
                            createdAddress,
                            endowment,
                            callFrame.Memory.Access(offset, length).ReadOnlyMemory,
                            callFrame.Depth + 1
                        )
                    );
                    callFrame.ReturnData.Set(creationResult.IsRevert(out var revertData)
                        ? revertData
                        : ReadOnlyMemory<byte>.Empty
                    );
                    callFrame.Stack.Push(creationResult.IsSuccess
                        ? createdAddress
                        : Address.Zero
                    );

                    break;
                }
                case EvmOpcode.Create2:
                {
                    if(callFrame.IsStatic)
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.WriteProtection);
                    }

                    if(!callFrame.Stack.TryPop(
                        out UInt256 endowment,
                        out UInt256 offset,
                        out UInt256 length,
                        out Bytes32 salt
                    ))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackUnderflow);
                    }
                    if(length > (UInt256) ExecutionSpec.MaxInitCodeLength)
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.InitCodeTooLarge);
                    }

                    var initCode = callFrame.Memory.Access(offset, length).ReadOnlyMemory;
                    var createdAddress = Address.DeriveCreate2(
                        callFrame.To,
                        salt,
                        Keccak256.HashData(initCode.Span)
                    );
                    var creationResult = await ExecuteContractCreationAsync(
                        transaction,
                        new ContractCreation(
                            callFrame.Origin,
                            callFrame.To,
                            createdAddress,
                            endowment,
                            initCode,
                            callFrame.Depth + 1
                        )
                    );
                    callFrame.ReturnData.Set(creationResult.IsRevert(out var revertData)
                        ? revertData
                        : ReadOnlyMemory<byte>.Empty
                    );
                    callFrame.Stack.Push(creationResult.IsSuccess
                        ? createdAddress
                        : Address.Zero
                    );

                    break;
                }
                case EvmOpcode.Call:
                {
                    if(!callFrame.Stack.TryPop(
                        out UInt256 _,
                        out Address address,
                        out UInt256 value,
                        out UInt256 inputOffset,
                        out UInt256 inputLength,
                        out UInt256 outputOffset,
                        out UInt256 outputLength
                    ))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackUnderflow);
                    }

                    if(callFrame.IsStatic && value != UInt256.Zero)
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.WriteProtection);
                    }

                    int outputSize = callFrame.Memory.Access(outputOffset, outputLength).Length;
                    var callResult = await ExecuteMessageCallAsync(transaction, new MessageCall(
                        callFrame.Origin,
                        callFrame.To,
                        address,
                        address,
                        callFrame.To,
                        value,
                        callFrame.Memory.Access(inputOffset, inputLength).ReadOnlyMemory,
                        callFrame.Depth + 1,
                        callFrame.IsStatic
                    ));

                    callFrame.ReturnData.Set(callResult.Data);
                    var output = callFrame.Memory.Access(outputOffset, outputSize);
                    callResult.Data.Span[..Math.Min(callResult.Data.Length, output.Length)].CopyTo(output.Span);
                    callFrame.Stack.Push(callResult.IsSuccess ? UInt256.One : UInt256.Zero);
                    break;
                }
                case EvmOpcode.CallCode:
                {
                    if(!callFrame.Stack.TryPop(
                        out UInt256 _,
                        out Address codeAddress,
                        out UInt256 value,
                        out UInt256 inputOffset,
                        out UInt256 inputLength,
                        out UInt256 outputOffset,
                        out UInt256 outputLength
                    ))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackUnderflow);
                    }

                    int outputSize = callFrame.Memory.Access(outputOffset, outputLength).Length;
                    bool hasSufficientBalance = value == UInt256.Zero
                        || await callFrame.AccountStorage.GetBalanceAsync() >= value;
                    var callResult = hasSufficientBalance
                        ? await ExecuteMessageCallAsync(transaction, new MessageCall(
                            callFrame.Origin,
                            callFrame.To,
                            callFrame.To,
                            codeAddress,
                            null,
                            value,
                            callFrame.Memory.Access(inputOffset, inputLength).ReadOnlyMemory,
                            callFrame.Depth + 1,
                            callFrame.IsStatic
                        ))
                        : ExecutionResult.CallEntryFailure(CallEntryFailureReason.InsufficientBalance);

                    callFrame.ReturnData.Set(callResult.Data);
                    var output = callFrame.Memory.Access(outputOffset, outputSize);
                    callResult.Data.Span[..Math.Min(callResult.Data.Length, output.Length)].CopyTo(output.Span);
                    callFrame.Stack.Push(callResult.IsSuccess ? UInt256.One : UInt256.Zero);
                    break;
                }
                case EvmOpcode.Return:
                {
                    return callFrame.Stack.TryPop(out UInt256 offset, out UInt256 length)
                        ? ExecutionResult.Success(
                            callFrame.Memory.Access(offset, length).ReadOnlyMemory
                        )
                        : ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackUnderflow);
                }
                case EvmOpcode.DelegateCall:
                {
                    if(!callFrame.Stack.TryPop(
                        out UInt256 _,
                        out Address codeAddress,
                        out UInt256 inputOffset,
                        out UInt256 inputLength,
                        out UInt256 outputOffset,
                        out UInt256 outputLength
                    ))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackUnderflow);
                    }

                    int outputSize = callFrame.Memory.Access(outputOffset, outputLength).Length;
                    var callResult = await ExecuteMessageCallAsync(transaction, new MessageCall(
                        callFrame.Origin,
                        callFrame.From,
                        callFrame.To,
                        codeAddress,
                        null,
                        callFrame.Value,
                        callFrame.Memory.Access(inputOffset, inputLength).ReadOnlyMemory,
                        callFrame.Depth + 1,
                        callFrame.IsStatic
                    ));

                    callFrame.ReturnData.Set(callResult.Data);
                    var output = callFrame.Memory.Access(outputOffset, outputSize);
                    callResult.Data.Span[..Math.Min(callResult.Data.Length, output.Length)].CopyTo(output.Span);
                    callFrame.Stack.Push(callResult.IsSuccess ? UInt256.One : UInt256.Zero);
                    break;
                }
                case EvmOpcode.StaticCall:
                {
                    if(!callFrame.Stack.TryPop(
                        out UInt256 _,
                        out Address address,
                        out UInt256 inputOffset,
                        out UInt256 inputLength,
                        out UInt256 outputOffset,
                        out UInt256 outputLength
                    ))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackUnderflow);
                    }

                    int outputSize = callFrame.Memory.Access(outputOffset, outputLength).Length;
                    var callResult = await ExecuteMessageCallAsync(transaction, new MessageCall(
                        callFrame.Origin,
                        callFrame.To,
                        address,
                        address,
                        null,
                        UInt256.Zero,
                        callFrame.Memory.Access(inputOffset, inputLength).ReadOnlyMemory,
                        callFrame.Depth + 1,
                        true
                    ));

                    callFrame.ReturnData.Set(callResult.Data);
                    var output = callFrame.Memory.Access(outputOffset, outputSize);
                    callResult.Data.Span[..Math.Min(callResult.Data.Length, output.Length)].CopyTo(output.Span);
                    callFrame.Stack.Push(callResult.IsSuccess ? UInt256.One : UInt256.Zero);
                    break;
                }
                case EvmOpcode.Revert:
                {
                    return callFrame.Stack.TryPop(out UInt256 offset, out UInt256 length)
                        ? ExecutionResult.Revert(
                            callFrame.Memory.Access(offset, length).ReadOnlyMemory
                        )
                        : ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackUnderflow);
                }
                case EvmOpcode.Invalid:
                    return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.InvalidOpcode);
                case EvmOpcode.SelfDestruct:
                {
                    if(callFrame.IsStatic)
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.WriteProtection);
                    }
                    if(!callFrame.Stack.TryPop(out Address beneficiary))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackUnderflow);
                    }

                    var balance = await callFrame.AccountStorage.GetBalanceAsync();
                    bool isSelfBeneficiary = beneficiary == callFrame.To;
                    bool shouldDelete = callFrame.AccountStorage.IsCreatedInTransaction;

                    if(balance != UInt256.Zero)
                    {
                        if(!isSelfBeneficiary)
                        {
                            var beneficiaryStorage = _storage.GetAccountStorage(beneficiary);
                            var beneficiaryBalance = await beneficiaryStorage.GetBalanceAsync();
                            beneficiaryStorage.SetBalance(beneficiaryBalance + balance);
                        }

                        if(!isSelfBeneficiary || shouldDelete)
                        {
                            callFrame.AccountStorage.SetBalance(UInt256.Zero);
                        }
                    }

                    if(shouldDelete)
                    {
                        callFrame.AccountStorage.ScheduleDeletion();
                    }

                    return ExecutionResult.Success();
                }
                default:
                    return Enum.IsDefined(opcode)
                        ? throw new NotImplementedException($"Opcode {opcode} at program counter {programCounter} is not implemented.")
                        : ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.InvalidOpcode);
            }

            programCounter++;
        }
    }

    private static bool IsValidJumpDestination(ReadOnlySpan<byte> code, UInt256 destination)
    {
        if(destination >= (UInt256) code.Length)
        {
            return false;
        }

        for(int programCounter = 0; programCounter <= (int) destination; programCounter++)
        {
            if(programCounter == (int) destination)
            {
                return code[programCounter] == (byte) EvmOpcode.JumpDest;
            }

            if(EvmOpcodeUtils.TryGetPushLength(code[programCounter], out int pushLength))
            {
                programCounter += pushLength;
            }
        }

        return false;
    }
}
