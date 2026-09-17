using EtherSharp.Contract;
using EtherSharp.Interpreter.Forking;
using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Runtime;

internal sealed partial class InterpreterRuntime : IInterpreterHost
{
    internal InterpreterStateFork Fork { get; }
    internal InterpreterStateFork.RunParticipant.Lane? Participant { get; set; }

    ValueTask<UInt256> IInterpreterHost.GetBalanceAsync(Address address)
        => Fork.GetAsync(this, Fork.Cache.Balances, address,
            static key => [new InterpreterDataRequest.Balance(key)]
        );

    ValueTask<ulong> IInterpreterHost.GetNonceAsync(Address address)
        => Fork.GetAsync(this, Fork.Cache.Nonces, address,
            static key => [new InterpreterDataRequest.Nonce(key)]
        );

    ValueTask<EVMByteCode> IInterpreterHost.GetCodeAsync(Address address)
        => Fork.GetAsync(this, Fork.Cache.Code, address,
            static key => [new InterpreterDataRequest.Code(key)]
        );

    ValueTask<Bytes32?> IInterpreterHost.GetCodeHashAsync(Address address)
        => Fork.GetAsync(this, Fork.Cache.CodeHashes, address,
            static key => [new InterpreterDataRequest.CodeHash(key)]
        );

    ValueTask<Bytes32> IInterpreterHost.GetStorageAtAsync(Address address, Bytes32 key)
        => Fork.GetAsync(this, Fork.Cache.Storage, (Address: address, Slot: key),
            static key => [new InterpreterDataRequest.Storage(key.Address, key.Slot)]
        );

    Task<TxCallResult> IInterpreterHost.CallPrecompileAsync(
        Address caller,
        Address target,
        UInt256 value,
        ReadOnlyMemory<byte> input
    ) => Fork.GetAsync(this, Fork.Cache.PrecompileCalls,
        InterpreterDataRequest.PrecompileCall.ComputeId(caller, target, value, input.Span),
        id => [new InterpreterDataRequest.PrecompileCall(caller, target, value, input, id)]
    ).AsTask();
}
