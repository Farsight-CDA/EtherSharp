using EtherSharp.Contract;
using EtherSharp.Interpreter.Forking;
using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Runtime.Storage;

internal readonly struct StateRequest<TValue>(
    StateRequest<TValue>.Kind requestKind,
    Address address,
    Bytes32 key = default
)
{
    public enum Kind : byte
    {
        Balance,
        Nonce,
        Code,
        CodeHash,
        Storage
    }

    public Kind RequestKind { get; } = requestKind;
    public Address Address { get; } = address;
    public Bytes32 Key { get; } = key;

    public HostRequest<TValue> CreateHostRequest()
    {
        HostRequest request = RequestKind switch
        {
            Kind.Balance => new HostRequest.Balance(Address),
            Kind.Nonce => new HostRequest.Nonce(Address),
            Kind.Code => new HostRequest.Code(Address),
            Kind.CodeHash => new HostRequest.CodeHash(Address),
            Kind.Storage => new HostRequest.Storage(Address, Key),
            _ => throw new ArgumentOutOfRangeException(nameof(RequestKind))
        };
        return (HostRequest<TValue>) request;
    }
}

internal static class StateRequest
{
    public static StateRequest<UInt256> Balance(Address address)
        => new(StateRequest<UInt256>.Kind.Balance, address);

    public static StateRequest<ulong> Nonce(Address address)
        => new(StateRequest<ulong>.Kind.Nonce, address);

    public static StateRequest<EVMByteCode> Code(Address address)
        => new(StateRequest<EVMByteCode>.Kind.Code, address);

    public static StateRequest<Bytes32?> CodeHash(Address address)
        => new(StateRequest<Bytes32?>.Kind.CodeHash, address);

    public static StateRequest<Bytes32> Storage(
        Address address,
        Bytes32 key
    ) => new(StateRequest<Bytes32>.Kind.Storage, address, key);
}
