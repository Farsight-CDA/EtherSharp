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
            return result;
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
            return await ExecuteTopLevelAsync(environment);
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
            return await ExecuteTopLevelAsync(environment);
        }
        finally
        {
            _storage.Reset(storageSnapshot);
        }
    }

    private void ThrowIfDisposed()
        => ObjectDisposedException.ThrowIf(_isDisposed, this);

    private async ValueTask<TxCallResult> ExecuteTopLevelAsync(
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

    private async ValueTask<TxCallResult> ExecuteMessageCallAsync(
        TransactionEnvironment transaction,
        MessageCall messageCall
    )
    {
        if(messageCall.Depth > CallFrame.MAX_DEPTH)
        {
            return new TxCallResult(false, ReadOnlyMemory<byte>.Empty);
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
                return new TxCallResult(false, ReadOnlyMemory<byte>.Empty);
            }

            if(valueSource != messageCall.Address)
            {
                var targetBalance = await accountStorage.GetBalanceAsync();
                valueSourceStorage.SetBalance(sourceBalance - messageCall.Value);
                accountStorage.SetBalance(targetBalance + messageCall.Value);
            }
        }

        TxCallResult result;
        if(_precompiles.TryGetValue(messageCall.CodeAddress, out var precompile))
        {
            result = await precompile.ExecuteAsync(
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

        if(!result.Success)
        {
            _storage.Reset(callSnapshot);
        }

        return result;
    }

    private async ValueTask<TxCallResult> ExecuteContractCreationAsync(
        TransactionEnvironment transaction,
        ContractCreation creation
    )
    {
        if(creation.Depth > CallFrame.MAX_DEPTH)
        {
            return new TxCallResult(false, ReadOnlyMemory<byte>.Empty);
        }

        var creatorStorage = _storage.GetAccountStorage(creation.Creator);
        var creatorBalance = await creatorStorage.GetBalanceAsync();
        if(creatorBalance < creation.Endowment)
        {
            return new TxCallResult(false, ReadOnlyMemory<byte>.Empty);
        }

        ulong creatorNonce = await creatorStorage.GetNonceAsync();
        if(creatorNonce == UInt64.MaxValue)
        {
            return new TxCallResult(false, ReadOnlyMemory<byte>.Empty);
        }

        creatorStorage.SetNonce(creatorNonce + 1);
        var createdStorage = _storage.GetAccountStorage(creation.CreatedAddress);
        if(await createdStorage.HasCreateCollisionAsync())
        {
            return new TxCallResult(false, ReadOnlyMemory<byte>.Empty);
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
        if(!creationResult.Success)
        {
            _storage.Reset(creationSnapshot);
            return creationResult;
        }
        if(creationResult.Data.Length > ExecutionSpec.MaxRuntimeCodeLength
            || (!creationResult.Data.IsEmpty && creationResult.Data.Span[0] == 0xEF))
        {
            _storage.Reset(creationSnapshot);
            return new TxCallResult(false, ReadOnlyMemory<byte>.Empty);
        }

        var runtimeCode = new EVMByteCode(creationResult.Data.ToArray());
        createdStorage.SetCode(in runtimeCode);
        return creationResult;
    }

    private async ValueTask<TxCallResult> ExecuteOpcodesAsync(
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
                    return new TxCallResult(true, ReadOnlyMemory<byte>.Empty);
                case >= EvmOpcode.Add and <= EvmOpcode.SMod:
                {
                    if(!callFrame.Stack.TryPop(out UInt256 first, out UInt256 second))
                    {
                        return callFrame.Revert();
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
                        return callFrame.Revert();
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
                        return callFrame.Revert();
                    }

                    callFrame.Stack.Push(UInt256.Pow(first, second));
                    break;
                }
                case EvmOpcode.SignExtend:
                {
                    if(!callFrame.Stack.TryPop(out UInt256 byteIndex, out Int256 value))
                    {
                        return callFrame.Revert();
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
                        return callFrame.Revert();
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
                        return callFrame.Revert();
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
                        return callFrame.Revert();
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
                        return callFrame.Revert();
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
                        return callFrame.Revert();
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
                        return callFrame.Revert();
                    }

                    var data = callFrame.Memory.Access(offset, length);
                    callFrame.Stack.Push(Keccak256.HashData(data.Span));
                    break;
                }
                case EvmOpcode.Address:
                    callFrame.Stack.Push(callFrame.To);
                    break;
                case EvmOpcode.Balance:
                {
                    if(!callFrame.Stack.TryPop(out Address address))
                    {
                        return callFrame.Revert();
                    }

                    callFrame.Stack.Push(await _storage.GetAccountStorage(address).GetBalanceAsync());
                    break;
                }
                case EvmOpcode.Origin:
                    callFrame.Stack.Push(callFrame.Origin);
                    break;
                case EvmOpcode.Caller:
                    callFrame.Stack.Push(callFrame.From);
                    break;
                case EvmOpcode.CallValue:
                    callFrame.Stack.Push(callFrame.Value);
                    break;
                case EvmOpcode.CallDataLoad:
                {
                    if(!callFrame.Stack.TryPop(out UInt256 offset))
                    {
                        return callFrame.Revert();
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
                    callFrame.Stack.Push((UInt256) callFrame.CallData.Length);
                    break;
                case EvmOpcode.CallDataCopy:
                {
                    if(!callFrame.Stack.TryPop(
                        out UInt256 destinationOffset,
                        out UInt256 sourceOffset,
                        out UInt256 length
                    ))
                    {
                        return callFrame.Revert();
                    }

                    callFrame.CallData.CopyTo(
                        sourceOffset,
                        callFrame.Memory.Access(destinationOffset, length)
                    );
                    break;
                }
                case EvmOpcode.CodeSize:
                    callFrame.Stack.Push((UInt256) code.Length);
                    break;
                case EvmOpcode.CodeCopy:
                {
                    if(!callFrame.Stack.TryPop(
                        out UInt256 destinationOffset,
                        out UInt256 sourceOffset,
                        out UInt256 length
                    ))
                    {
                        return callFrame.Revert();
                    }

                    code.CopyTo(sourceOffset, callFrame.Memory.Access(destinationOffset, length));
                    break;
                }
                case EvmOpcode.GasPrice:
                    callFrame.Stack.Push(transaction.EffectiveGasPrice);
                    break;
                case EvmOpcode.ExtCodeSize:
                {
                    if(!callFrame.Stack.TryPop(out Address address))
                    {
                        return callFrame.Revert();
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
                        return callFrame.Revert();
                    }

                    var externalCode = new ZeroPaddedData((await _storage.GetAccountStorage(address).GetCodeAsync()).ByteCode);
                    externalCode.CopyTo(sourceOffset, callFrame.Memory.Access(destinationOffset, length));
                    break;
                }
                case EvmOpcode.ReturnDataSize:
                    callFrame.Stack.Push((UInt256) callFrame.ReturnData.Length);
                    break;
                case EvmOpcode.ReturnDataCopy:
                {
                    if(!callFrame.Stack.TryPop(
                        out UInt256 destinationOffset,
                        out UInt256 sourceOffset,
                        out UInt256 length
                    ))
                    {
                        return callFrame.Revert();
                    }

                    if(!callFrame.ReturnData.TryCopyTo(
                        sourceOffset,
                        callFrame.Memory.Access(destinationOffset, length)
                    ))
                    {
                        return callFrame.Revert();
                    }

                    break;
                }
                case EvmOpcode.ExtCodeHash:
                {
                    if(!callFrame.Stack.TryPop(out Address address))
                    {
                        return callFrame.Revert();
                    }

                    callFrame.Stack.Push(await _storage.GetAccountStorage(address).GetExtCodeHashAsync());
                    break;
                }
                case EvmOpcode.BlockHash:
                {
                    if(!callFrame.Stack.TryPop(out UInt256 blockNumber))
                    {
                        return callFrame.Revert();
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
                    callFrame.Stack.Push(_context.Coinbase);
                    break;
                case EvmOpcode.Timestamp:
                    callFrame.Stack.Push((UInt256) _context.BlockTimestamp.ToUnixTimeSeconds());
                    break;
                case EvmOpcode.Number:
                    callFrame.Stack.Push((UInt256) _context.BlockNumber);
                    break;
                case EvmOpcode.PrevRandao:
                    callFrame.Stack.Push(_context.PrevRandao);
                    break;
                case EvmOpcode.GasLimit:
                    callFrame.Stack.Push(_context.GasLimit);
                    break;
                case EvmOpcode.ChainId:
                    callFrame.Stack.Push((UInt256) _context.ChainId);
                    break;
                case EvmOpcode.SelfBalance:
                    callFrame.Stack.Push(await callFrame.AccountStorage.GetBalanceAsync());
                    break;
                case EvmOpcode.BaseFee:
                    if(!_context.BaseFee.HasValue)
                    {
                        return callFrame.Revert();
                    }

                    callFrame.Stack.Push(_context.BaseFee.Value);
                    break;
                case EvmOpcode.BlobHash:
                    if(!_context.BlobBaseFee.HasValue
                        || !callFrame.Stack.TryPop(out UInt256 blobIndex))
                    {
                        return callFrame.Revert();
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
                        return callFrame.Revert();
                    }

                    callFrame.Stack.Push(_context.BlobBaseFee.Value);
                    break;
                case EvmOpcode.Pop:
                    if(!callFrame.Stack.TryPop(out Bytes32 _))
                    {
                        return callFrame.Revert();
                    }

                    break;
                case EvmOpcode.MLoad:
                {
                    if(!callFrame.Stack.TryPop(out UInt256 offset))
                    {
                        return callFrame.Revert();
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
                        return callFrame.Revert();
                    }

                    value.CopyTo(callFrame.Memory.Access(offset, Bytes32.BYTE_LENGTH).Span);
                    break;
                }
                case EvmOpcode.MStore8:
                {
                    if(!callFrame.Stack.TryPop(out UInt256 offset, out Bytes32 value))
                    {
                        return callFrame.Revert();
                    }

                    callFrame.Memory.Access(offset, 1).Span[0] = value[^1];
                    break;
                }
                case EvmOpcode.SLoad:
                {
                    if(!callFrame.Stack.TryPop(out Bytes32 key))
                    {
                        return callFrame.Revert();
                    }

                    callFrame.Stack.Push(await callFrame.AccountStorage.SLoadAsync(key));
                    break;
                }
                case EvmOpcode.SStore:
                {
                    if(callFrame.IsStatic)
                    {
                        return callFrame.Revert();
                    }

                    if(!callFrame.Stack.TryPop(out Bytes32 key, out Bytes32 value))
                    {
                        return callFrame.Revert();
                    }

                    callFrame.AccountStorage.SStore(in key, in value);
                    break;
                }
                case EvmOpcode.Jump:
                {
                    if(!callFrame.Stack.TryPop(out UInt256 destination)
                        || !IsValidJumpDestination(code.Data.Span, destination))
                    {
                        return callFrame.Revert();
                    }

                    programCounter = (int) destination;
                    continue;
                }
                case EvmOpcode.JumpI:
                {
                    if(!callFrame.Stack.TryPop(out UInt256 destination, out UInt256 condition))
                    {
                        return callFrame.Revert();
                    }
                    if(condition == UInt256.Zero)
                    {
                        break;
                    }
                    if(!IsValidJumpDestination(code.Data.Span, destination))
                    {
                        return callFrame.Revert();
                    }

                    programCounter = (int) destination;
                    continue;
                }
                case EvmOpcode.Pc:
                    callFrame.Stack.Push((UInt256) programCounter);
                    break;
                case EvmOpcode.MSize:
                    callFrame.Stack.Push((UInt256) callFrame.Memory.Size);
                    break;
                case EvmOpcode.Gas:
                    //ToDo: Gas tracking
                    callFrame.Stack.Push(UInt256.MaxValue);
                    break;
                case EvmOpcode.JumpDest:
                    break;
                case EvmOpcode.TLoad:
                {
                    if(!callFrame.Stack.TryPop(out Bytes32 key))
                    {
                        return callFrame.Revert();
                    }

                    callFrame.Stack.Push(callFrame.AccountStorage.TLoad(in key));
                    break;
                }
                case EvmOpcode.TStore:
                {
                    if(callFrame.IsStatic)
                    {
                        return callFrame.Revert();
                    }

                    if(!callFrame.Stack.TryPop(out Bytes32 key, out Bytes32 value))
                    {
                        return callFrame.Revert();
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
                        return callFrame.Revert();
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
                        return callFrame.Revert();
                    }

                    programCounter += pushLength;
                    break;
                }
                case >= EvmOpcode.Dup1 and <= EvmOpcode.Dup16:
                {
                    int depth = (byte) opcode - ((byte) EvmOpcode.Dup1 - 1);
                    if(!callFrame.Stack.TryDup(depth))
                    {
                        return callFrame.Revert();
                    }

                    break;
                }
                case >= EvmOpcode.Swap1 and <= EvmOpcode.Swap16:
                {
                    int depth = (byte) opcode - ((byte) EvmOpcode.Swap1 - 1);
                    if(!callFrame.Stack.TrySwap(depth))
                    {
                        return callFrame.Revert();
                    }

                    break;
                }
                case >= EvmOpcode.Log0 and <= EvmOpcode.Log4:
                {
                    if(callFrame.IsStatic)
                    {
                        return callFrame.Revert();
                    }

                    int topicCount = (byte) opcode - (byte) EvmOpcode.Log0;
                    if(!callFrame.Stack.TryPop(out UInt256 offset, out UInt256 length))
                    {
                        return callFrame.Revert();
                    }

                    var topics = new Bytes32[topicCount];
                    for(int i = 0; i < topics.Length; i++)
                    {
                        if(!callFrame.Stack.TryPop(out topics[i]))
                        {
                            return callFrame.Revert();
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
                        return callFrame.Revert();
                    }

                    if(!callFrame.Stack.TryPop(
                        out UInt256 endowment,
                        out UInt256 offset,
                        out UInt256 length
                    ) || length > (UInt256) ExecutionSpec.MaxInitCodeLength)
                    {
                        return callFrame.Revert();
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
                    callFrame.ReturnData.Set(creationResult.Success
                        ? ReadOnlyMemory<byte>.Empty
                        : creationResult.Data
                    );
                    callFrame.Stack.Push(creationResult.Success
                        ? createdAddress
                        : Address.Zero
                    );

                    break;
                }
                case EvmOpcode.Create2:
                {
                    if(callFrame.IsStatic)
                    {
                        return callFrame.Revert();
                    }

                    if(!callFrame.Stack.TryPop(
                        out UInt256 endowment,
                        out UInt256 offset,
                        out UInt256 length,
                        out Bytes32 salt
                    ) || length > (UInt256) ExecutionSpec.MaxInitCodeLength)
                    {
                        return callFrame.Revert();
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
                    callFrame.ReturnData.Set(creationResult.Success
                        ? ReadOnlyMemory<byte>.Empty
                        : creationResult.Data
                    );
                    callFrame.Stack.Push(creationResult.Success
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
                        return callFrame.Revert();
                    }

                    if(callFrame.IsStatic && value != UInt256.Zero)
                    {
                        return callFrame.Revert();
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
                    callFrame.Stack.Push(callResult.Success ? UInt256.One : UInt256.Zero);
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
                        return callFrame.Revert();
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
                        : new TxCallResult(false, ReadOnlyMemory<byte>.Empty);

                    callFrame.ReturnData.Set(callResult.Data);
                    var output = callFrame.Memory.Access(outputOffset, outputSize);
                    callResult.Data.Span[..Math.Min(callResult.Data.Length, output.Length)].CopyTo(output.Span);
                    callFrame.Stack.Push(callResult.Success ? UInt256.One : UInt256.Zero);
                    break;
                }
                case EvmOpcode.Return:
                {
                    return callFrame.Stack.TryPop(out UInt256 offset, out UInt256 length)
                        ? new TxCallResult(
                            true,
                            callFrame.Memory.Access(offset, length).ReadOnlyMemory
                        )
                        : callFrame.Revert();
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
                        return callFrame.Revert();
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
                    callFrame.Stack.Push(callResult.Success ? UInt256.One : UInt256.Zero);
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
                        return callFrame.Revert();
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
                    callFrame.Stack.Push(callResult.Success ? UInt256.One : UInt256.Zero);
                    break;
                }
                case EvmOpcode.Revert:
                {
                    return callFrame.Stack.TryPop(out UInt256 offset, out UInt256 length)
                        ? callFrame.Revert(
                            callFrame.Memory.Access(offset, length).ReadOnlyMemory
                        )
                        : callFrame.Revert();
                }
                case EvmOpcode.Invalid:
                    return callFrame.Revert();
                case EvmOpcode.SelfDestruct:
                {
                    if(callFrame.IsStatic
                        || !callFrame.Stack.TryPop(out Address beneficiary))
                    {
                        return callFrame.Revert();
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

                    return new TxCallResult(true, ReadOnlyMemory<byte>.Empty);
                }
                default:
                    return Enum.IsDefined(opcode)
                        ? throw new NotImplementedException($"Opcode {opcode} at program counter {programCounter} is not implemented.")
                        : callFrame.Revert();
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
