using EtherSharp.Contract;
using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Forking;

/// <summary>
/// Stores resolved upstream values. All access, including dictionary access, must be
/// protected by the owning fork's state lock.
/// </summary>
internal sealed class InterpreterStateCache
{
    public Dictionary<Address, UInt256> Balances { get; } = [];
    public Dictionary<Address, ulong> Nonces { get; } = [];
    public Dictionary<Address, EVMByteCode> Code { get; } = [];
    public Dictionary<Address, Bytes32?> CodeHashes { get; } = [];
    public Dictionary<(Address Address, Bytes32 Slot), Bytes32> Storage { get; } = [];
    public Dictionary<Bytes32, TxCallResult> Calls { get; } = [];

    public bool Contains(InterpreterDataRequest request)
        => request switch
        {
            InterpreterDataRequest.Balance balance => Balances.ContainsKey(balance.Address),
            InterpreterDataRequest.Nonce nonce => Nonces.ContainsKey(nonce.Address),
            InterpreterDataRequest.Code code => Code.ContainsKey(code.Address),
            InterpreterDataRequest.CodeHash codeHash => CodeHashes.ContainsKey(codeHash.Address),
            InterpreterDataRequest.Storage storage => Storage.ContainsKey((storage.Address, storage.Key)),
            InterpreterDataRequest.Call call => Calls.ContainsKey(call.Id),
            _ => throw new NotSupportedException(),
        };

    public void Store(InterpreterDataResult result)
    {
        switch(result)
        {
            case InterpreterDataResult.Balance balance:
                Balances[balance.Address] = balance.Value;
                break;
            case InterpreterDataResult.Nonce nonce:
                Nonces[nonce.Address] = nonce.Value;
                break;
            case InterpreterDataResult.Code code:
                Code[code.Address] = code.Value;
                break;
            case InterpreterDataResult.CodeHash codeHash:
                CodeHashes[codeHash.Address] = codeHash.Value == Bytes32.Zero ? null : codeHash.Value;
                break;
            case InterpreterDataResult.Storage storage:
                Storage[(storage.Address, storage.Key)] = storage.Value;
                break;
            case InterpreterDataResult.Call call:
                Calls[InterpreterDataRequest.Call.ComputeId(call.Caller, call.Target, call.Value, call.Input.Span)] = call.Result;
                break;
            default:
                throw new NotSupportedException();
        }
    }
}
