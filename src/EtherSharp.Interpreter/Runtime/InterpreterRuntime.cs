using EtherSharp.Contract;
using EtherSharp.Crypto;
using EtherSharp.Interpreter.Memory;
using EtherSharp.Interpreter.Precompiles;
using EtherSharp.Interpreter.Storage;
using EtherSharp.Numerics;
using EtherSharp.Types;
using System.Collections.Frozen;
using System.Diagnostics;

namespace EtherSharp.Interpreter.Runtime;

/// <summary>
/// Executes EVM transactions and call simulations against a supplied global state.
/// </summary>
/// <param name="context">The block and transaction context for execution.</param>
/// <param name="host">The host used to read upstream state and execute calls.</param>
/// <param name="executionSpec">The consensus behavior used for execution.</param>
/// <param name="options">The interpreter resource limits.</param>
public class InterpreterRuntime(
    InterpreterContext context,
    IInterpreterHost host,
    InterpreterExecutionSpec executionSpec,
    InterpreterOptions? options = null
)
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

    private readonly InterpreterStorage _storage = new(
        context ?? throw new ArgumentNullException(nameof(context)),
        host ?? throw new ArgumentNullException(nameof(host))
    );
    private readonly IInterpreterHost _host = host;
    private readonly FrozenDictionary<Address, IPrecompile> _precompiles = (executionSpec
        ?? throw new ArgumentNullException(nameof(executionSpec)))
            .Validate()
            .CreatePrecompileLookup();

    /// <summary>
    /// Gets the block and transaction context for execution.
    /// </summary>
    public InterpreterContext Context { get; } = context;
    /// <summary>
    /// Gets the interpreter configuration.
    /// </summary>
    public InterpreterOptions Options { get; } = (options ?? new InterpreterOptions()).Validate();
    /// <summary>
    /// Gets the consensus behavior used for execution.
    /// </summary>
    public InterpreterExecutionSpec ExecutionSpec { get; } = executionSpec
        ?? throw new ArgumentNullException(nameof(executionSpec));

    /// <summary>
    /// Executes a transaction from the supplied sender and retains its state changes.
    /// </summary>
    /// <param name="sender">The caller exposed through <c>msg.sender</c>.</param>
    /// <param name="to">The account whose code and storage are used.</param>
    /// <param name="value">The native value exposed through <c>msg.value</c>.</param>
    /// <param name="callData">The calldata supplied to the call.</param>
    /// <returns>The transaction execution result.</returns>
    /// <remarks>The sender nonce is incremented even when EVM execution reverts.</remarks>
    public async ValueTask<TxCallResult> ExecuteTransactionAsync(
        Address sender,
        Address to,
        UInt256 value,
        ReadOnlyMemory<byte> callData
    )
    {
        var storageSnapshot = _storage.TakeSnapshot();
        TxCallResult result;
        try
        {
            var senderStorage = _storage.GetAccountStorage(sender);
            ulong senderNonce = await senderStorage.GetNonceAsync();
            senderStorage.SetNonce(checked(senderNonce + 1));

            result = await ExecuteMessageCallAsync(new MessageCall(
                sender,
                sender,
                to,
                to,
                sender,
                value,
                callData,
                0,
                false
            ));
        }
        catch
        {
            _storage.Reset(storageSnapshot);
            throw;
        }

        _storage.Commit();
        return result;
    }

    /// <summary>
    /// Simulates a call from the supplied sender with the specified options and discards all state changes.
    /// </summary>
    /// <param name="sender">The caller exposed through <c>msg.sender</c>.</param>
    /// <param name="to">The account whose code and storage are used.</param>
    /// <param name="value">The native value exposed through <c>msg.value</c>.</param>
    /// <param name="callData">The calldata supplied to the call.</param>
    /// <param name="options">The simulation options.</param>
    /// <returns>The simulated call result.</returns>
    /// <remarks>The sender nonce is incremented during execution, then restored with the other simulated state changes.</remarks>
    public async ValueTask<TxCallResult> SimulateCallAsync(
        Address sender,
        Address to,
        UInt256 value,
        ReadOnlyMemory<byte> callData,
        InterpreterCallOptions options = default
    )
    {
        var storageSnapshot = _storage.TakeSnapshot();
        try
        {
            if(options.StateOverrides is { } stateOverrides)
            {
                _storage.ApplyStateOverrides(stateOverrides);
            }

            var senderStorage = _storage.GetAccountStorage(sender);
            ulong senderNonce = await senderStorage.GetNonceAsync();
            senderStorage.SetNonce(checked(senderNonce + 1));

            return await ExecuteMessageCallAsync(new MessageCall(
                sender,
                sender,
                to,
                to,
                sender,
                value,
                callData,
                0,
                false
            ));
        }
        finally
        {
            _storage.Reset(storageSnapshot);
        }
    }

    private async ValueTask<TxCallResult> ExecuteMessageCallAsync(MessageCall messageCall)
    {
        if(messageCall.Depth >= CallFrame.MAX_DEPTH)
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
                    Context,
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
            result = await ExecuteOpcodesAsync(callFrame, byteCode);
        }

        if(!result.Success)
        {
            _storage.Reset(callSnapshot);
        }

        return result;
    }

    private async ValueTask<TxCallResult> ExecuteOpcodesAsync(
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
                    callFrame.Stack.Push(Context.GasPrice);
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

                    callFrame.Stack.Push(await _storage.GetAccountStorage(address).GetCodeHashAsync());
                    break;
                }
                case EvmOpcode.BlockHash:
                {
                    if(!callFrame.Stack.TryPop(out UInt256 blockNumber))
                    {
                        return callFrame.Revert();
                    }

                    if(blockNumber >= (UInt256) Context.BlockNumber)
                    {
                        callFrame.Stack.Push(Bytes32.Zero);
                        break;
                    }

                    var distance = (UInt256) Context.BlockNumber - blockNumber;
                    callFrame.Stack.Push(
                        distance <= 256 && distance <= (UInt256) Context.RecentBlockHashes.Length
                            ? Context.RecentBlockHashes[(int) distance - 1]
                            : Bytes32.Zero
                    );
                    break;
                }
                case EvmOpcode.Coinbase:
                    callFrame.Stack.Push(Context.Coinbase);
                    break;
                case EvmOpcode.Timestamp:
                    callFrame.Stack.Push((UInt256) Context.BlockTimestamp.ToUnixTimeSeconds());
                    break;
                case EvmOpcode.Number:
                    callFrame.Stack.Push((UInt256) Context.BlockNumber);
                    break;
                case EvmOpcode.PrevRandao:
                    callFrame.Stack.Push(Context.PrevRandao);
                    break;
                case EvmOpcode.GasLimit:
                    callFrame.Stack.Push(Context.GasLimit);
                    break;
                case EvmOpcode.ChainId:
                    callFrame.Stack.Push((UInt256) Context.ChainId);
                    break;
                case EvmOpcode.SelfBalance:
                    callFrame.Stack.Push(await callFrame.AccountStorage.GetBalanceAsync());
                    break;
                case EvmOpcode.BaseFee:
                    if(!Context.BaseFee.HasValue)
                    {
                        return callFrame.Revert();
                    }

                    callFrame.Stack.Push(Context.BaseFee.Value);
                    break;
                case EvmOpcode.BlobHash:
                    if(!Context.BlobBaseFee.HasValue
                        || !callFrame.Stack.TryPop(out UInt256 _))
                    {
                        return callFrame.Revert();
                    }

                    callFrame.Stack.Push(Bytes32.Zero);
                    break;
                case EvmOpcode.BlobBaseFee:
                    if(!Context.BlobBaseFee.HasValue)
                    {
                        return callFrame.Revert();
                    }

                    callFrame.Stack.Push(Context.BlobBaseFee.Value);
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
                    if(callFrame.IsStatic)
                    {
                        return callFrame.Revert();
                    }

                    throw new NotSupportedException("Contract creation is not supported.");
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
                    callFrame.ReturnData.Clear();

                    var callResult = await ExecuteMessageCallAsync(new MessageCall(
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
                    callFrame.ReturnData.Clear();

                    bool hasSufficientBalance = value == UInt256.Zero
                        || await callFrame.AccountStorage.GetBalanceAsync() >= value;
                    var callResult = hasSufficientBalance
                        ? await ExecuteMessageCallAsync(new MessageCall(
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
                    callFrame.ReturnData.Clear();

                    var callResult = await ExecuteMessageCallAsync(new MessageCall(
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
                    callFrame.ReturnData.Clear();

                    var callResult = await ExecuteMessageCallAsync(new MessageCall(
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
                    if(balance != UInt256.Zero && beneficiary != callFrame.To)
                    {
                        var beneficiaryStorage = _storage.GetAccountStorage(beneficiary);
                        var beneficiaryBalance = await beneficiaryStorage.GetBalanceAsync();
                        callFrame.AccountStorage.SetBalance(UInt256.Zero);
                        beneficiaryStorage.SetBalance(beneficiaryBalance + balance);
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
