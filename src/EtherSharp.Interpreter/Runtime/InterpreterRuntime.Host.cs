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
        => ReadAsync(Fork.Cache.Balances, address,
            static key => [new InterpreterDataRequest.Balance(key)]
        );

    ValueTask<ulong> IInterpreterHost.GetNonceAsync(Address address)
        => ReadAsync(Fork.Cache.Nonces, address,
            static key => [new InterpreterDataRequest.Nonce(key)]
        );

    ValueTask<EVMByteCode> IInterpreterHost.GetCodeAsync(Address address)
        => ReadAsync(Fork.Cache.Code, address,
            static key => [new InterpreterDataRequest.Code(key)]
        );

    ValueTask<Bytes32?> IInterpreterHost.GetCodeHashAsync(Address address)
        => ReadAsync(Fork.Cache.CodeHashes, address,
            static key => [new InterpreterDataRequest.CodeHash(key)]
        );

    ValueTask<Bytes32> IInterpreterHost.GetStorageAtAsync(Address address, Bytes32 key)
        => ReadAsync(Fork.Cache.Storage, (Address: address, Slot: key),
            static key => [new InterpreterDataRequest.Storage(key.Address, key.Slot)]
        );

    Task<TxCallResult> IInterpreterHost.CallPrecompileAsync(
        Address caller,
        Address target,
        UInt256 value,
        ReadOnlyMemory<byte> input
    ) => ReadAsync(Fork.Cache.PrecompileCalls,
        InterpreterDataRequest.PrecompileCall.ComputeId(caller, target, value, input.Span),
        id => [new InterpreterDataRequest.PrecompileCall(caller, target, value, input, id)]
    ).AsTask();

    private async ValueTask<TValue> ReadAsync<TKey, TValue>(
        Dictionary<TKey, TValue> cache,
        TKey key,
        Func<TKey, List<InterpreterDataRequest>> createRequests
    ) where TKey : notnull
    {
        if(Fork.TryGetCached(cache, key, out var value))
        {
            return value;
        }

        await Fork.EnsureCachedAsync(this, createRequests(key));
        return Fork.GetCached(cache, key);
    }
}
