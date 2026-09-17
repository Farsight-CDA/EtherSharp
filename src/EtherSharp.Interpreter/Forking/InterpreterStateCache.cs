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
    public Dictionary<Bytes32, TxCallResult> PrecompileCalls { get; } = [];

    public InterpreterStateCache(IReadOnlyList<InterpreterDataResult>? initialState = null)
    {
        if(initialState is null)
        {
            return;
        }

        for(int i = 0; i < initialState.Count; i++)
        {
            ArgumentNullException.ThrowIfNull(initialState[i]);
            if(!TryStore(initialState[i], copyBuffers: true))
            {
                throw new ArgumentException(
                    $"The initial state contains multiple values for the same logical key: {initialState[i]}.",
                    nameof(initialState)
                );
            }
        }
    }

    public bool Contains(HostRequest request)
        => request switch
        {
            HostRequest.Balance balance => Balances.ContainsKey(balance.Address),
            HostRequest.Nonce nonce => Nonces.ContainsKey(nonce.Address),
            HostRequest.Code code => Code.ContainsKey(code.Address),
            HostRequest.CodeHash codeHash => CodeHashes.ContainsKey(codeHash.Address),
            HostRequest.Storage storage => Storage.ContainsKey((storage.Address, storage.Key)),
            HostRequest.PrecompileCall call => PrecompileCalls.ContainsKey(call.Id),
            _ => throw new NotSupportedException(),
        };

    public void Store(InterpreterDataResult result)
        => TryStore(result, copyBuffers: false);

    private bool TryStore(InterpreterDataResult result, bool copyBuffers)
        => result switch
        {
            InterpreterDataResult.Balance balance => Balances.TryAdd(balance.Address, balance.Value),
            InterpreterDataResult.Nonce nonce => Nonces.TryAdd(nonce.Address, nonce.Value),
            InterpreterDataResult.Code code => Code.TryAdd(
                code.Address,
                copyBuffers
                    ? new EVMByteCode(code.Value.ByteCode.ToArray())
                    : code.Value
            ),
            InterpreterDataResult.CodeHash codeHash => CodeHashes.TryAdd(
                codeHash.Address,
                codeHash.Value == Bytes32.Zero
                    ? null
                    : codeHash.Value
            ),
            InterpreterDataResult.Storage storage => Storage.TryAdd((storage.Address, storage.Key), storage.Value),
            InterpreterDataResult.PrecompileCall call => PrecompileCalls.TryAdd(
                HostRequest.PrecompileCall.ComputeId(call.Caller, call.Target, call.Value, call.Input.Span),
                copyBuffers
                    ? new TxCallResult(call.Result.Success, call.Result.Data.ToArray())
                    : call.Result
            ),
            _ => throw new NotSupportedException(),
        };
}
