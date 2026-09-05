using EtherSharp.Contract;
using EtherSharp.Interpreter.Runtime;
using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Forking;

public sealed partial class InterpreterStateFork
{
    private sealed class InterpreterSession(InterpreterStateFork fork) : IInterpreterHost
    {
        // Passive participant state: only the fork changes these flags, under its state lock.
        public bool IsUnregistered { get; set; }
        public bool IsReadInProgress { get; set; }

        public ValueTask<UInt256> GetBalanceAsync(Address address)
            => fork.GetAsync(this, fork._cache.Balances, address,
                static key => new InterpreterDataRequest.Balance(key)
            );

        public ValueTask<ulong> GetNonceAsync(Address address)
            => fork.GetAsync(this, fork._cache.Nonces, address,
                static key => new InterpreterDataRequest.Nonce(key)
            );

        public ValueTask<EVMByteCode> GetCodeAsync(Address address)
            => fork.GetAsync(this, fork._cache.Code, address,
                static key => new InterpreterDataRequest.Code(key)
            );

        public ValueTask<Bytes32?> GetCodeHashAsync(Address address)
            => fork.GetAsync(this, fork._cache.CodeHashes, address,
                static key => new InterpreterDataRequest.CodeHash(key)
            );

        public ValueTask<Bytes32> GetStorageAtAsync(Address address, Bytes32 key)
            => fork.GetAsync(this, fork._cache.Storage, (Address: address, Slot: key),
                static key => new InterpreterDataRequest.Storage(key.Address, key.Slot)
            );

        public Task<TxCallResult> CallAsync(
            Address caller,
            Address target,
            UInt256 value,
            ReadOnlyMemory<byte> input
        ) => fork.GetAsync(this, fork._cache.Calls,
            InterpreterDataRequest.Call.ComputeId(caller, target, value, input.Span),
            id => new InterpreterDataRequest.Call(caller, target, value, input, id)
        ).AsTask();

        public void Unregister()
            => fork.RemoveInterpreter(this);
    }
}
