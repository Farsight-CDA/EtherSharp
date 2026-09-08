using EtherSharp.Contract;
using EtherSharp.Crypto;
using EtherSharp.Interpreter.Runtime.Memory;
using EtherSharp.Numerics;
using EtherSharp.Types;
using System.Diagnostics;

namespace EtherSharp.Interpreter.Runtime;

public partial class InterpreterRuntime
{
    private async ValueTask<ExecutionResult> ExecuteOpcodesAsync(BytecodeFrame callFrame, ZeroPaddedData code)
    {
        int programCounter = 0;

        while(true)
        {
            var opcode = (EvmOpcode) code[programCounter];

            if(_executionState!.Hooks is not null)
            {
                await _executionState.Hooks.OnInstructionAsync(callFrame, programCounter, opcode, _storage);
            }

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
                        && second.IsZero)
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
                        // Yul smod returns zero for a divisor of -1, including MinValue % -1.
                        EvmOpcode.SMod when second == UInt256.MaxValue => UInt256.Zero,
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
                    if(modulus.IsZero)
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
                        EvmOpcode.IsZero => value.IsZero ? UInt256.One : UInt256.Zero,
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
                    if(!callFrame.Stack.TryPush(callFrame.Call.Address))
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
                    if(!callFrame.Stack.TryPush(callFrame.Call.Origin))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackOverflow);
                    }

                    break;
                case EvmOpcode.Caller:
                    if(!callFrame.Stack.TryPush(callFrame.Call.Caller))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackOverflow);
                    }

                    break;
                case EvmOpcode.CallValue:
                    if(!callFrame.Stack.TryPush(callFrame.Call.Value))
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
                    if(!callFrame.Stack.TryPush(_executionState!.Transaction.EffectiveGasPrice))
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
                        blobIndex < (UInt256) _executionState!.Transaction.BlobHashes.Length
                            ? _executionState.Transaction.BlobHashes.Span[(int) blobIndex]
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
                    if(callFrame.Call.IsStatic)
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
                    if(condition.IsZero)
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
                    if(callFrame.Call.IsStatic)
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
                    if(callFrame.Call.IsStatic)
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
                    _storage.AddLog(callFrame.Call.Address, topics, data);
                    if(_executionState!.Hooks is not null)
                    {
                        await _executionState.Hooks.OnLogAsync(callFrame, topics, data, _storage);
                    }
                    break;
                }
                case EvmOpcode.Create:
                {
                    if(callFrame.Call.IsStatic)
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
                        callFrame.Call.Address,
                        await callFrame.AccountStorage.GetNonceAsync()
                    );
                    var creationResult = await ExecuteContractCreationAsync(
                        CallFrame.CreateContractCreation(
                            checked(_executionState!.NextFrameId++),
                            EvmOpcode.Create,
                            callFrame.Call,
                            createdAddress,
                            endowment,
                            callFrame.Memory.Access(offset, length).ReadOnlyMemory
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
                    if(callFrame.Call.IsStatic)
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
                        callFrame.Call.Address,
                        salt,
                        Keccak256.HashData(initCode.Span)
                    );
                    var creationResult = await ExecuteContractCreationAsync(
                        CallFrame.CreateContractCreation(
                            checked(_executionState!.NextFrameId++),
                            EvmOpcode.Create2,
                            callFrame.Call,
                            createdAddress,
                            endowment,
                            initCode
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

                    if(callFrame.Call.IsStatic && !value.IsZero)
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.WriteProtection);
                    }

                    int outputSize = callFrame.Memory.Access(outputOffset, outputLength).Length;
                    var callResult = await ExecuteMessageCallAsync(
                        CallFrame.CreateMessageCall(
                            checked(_executionState!.NextFrameId++),
                            EvmOpcode.Call,
                            callFrame.Call,
                            address,
                            callFrame.Memory.Access(inputOffset, inputLength).ReadOnlyMemory,
                            value
                        )
                    );

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
                    var callResult = await ExecuteMessageCallAsync(
                        CallFrame.CreateMessageCall(
                            checked(_executionState!.NextFrameId++),
                            EvmOpcode.CallCode,
                            callFrame.Call,
                            codeAddress,
                            callFrame.Memory.Access(inputOffset, inputLength).ReadOnlyMemory,
                            value
                        )
                    );

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
                    var callResult = await ExecuteMessageCallAsync(CallFrame.CreateMessageCall(
                        checked(_executionState!.NextFrameId++),
                        EvmOpcode.DelegateCall,
                        callFrame.Call,
                        codeAddress,
                        callFrame.Memory.Access(inputOffset, inputLength).ReadOnlyMemory
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
                    var callResult = await ExecuteMessageCallAsync(CallFrame.CreateMessageCall(
                        checked(_executionState!.NextFrameId++),
                        EvmOpcode.StaticCall,
                        callFrame.Call,
                        address,
                        callFrame.Memory.Access(inputOffset, inputLength).ReadOnlyMemory
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
                    if(callFrame.Call.IsStatic)
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.WriteProtection);
                    }
                    if(!callFrame.Stack.TryPop(out Address beneficiary))
                    {
                        return ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.StackUnderflow);
                    }

                    var balance = await callFrame.AccountStorage.GetBalanceAsync();
                    bool isSelfBeneficiary = beneficiary == callFrame.Call.Address;
                    bool shouldDelete = callFrame.AccountStorage.IsCreatedInTransaction;

                    if(!balance.IsZero)
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

                    if(_executionState!.Hooks is not null)
                    {
                        await _executionState.Hooks.OnSelfDestructAsync(callFrame, beneficiary, balance, _storage);
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
