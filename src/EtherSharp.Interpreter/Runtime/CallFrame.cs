using EtherSharp.Contract;
using EtherSharp.Interpreter.Runtime.Tracing;
using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Runtime;

internal sealed record CallFrame(
    int Id,
    EvmOpcode Type,
    Address Origin,
    CallFrame? Parent,
    Address Caller,
    Address Address,
    Address To,
    UInt256 Value,
    ReadOnlyMemory<byte> Input
) : IInterpreterFrameReader
{
    public const int MAX_DEPTH = 1024;

    public Address From => Parent?.Address ?? Origin;
    public int Depth { get; } = Parent is null ? 0 : Parent.Depth + 1;
    public bool IsStatic { get; } = Parent?.IsStatic == true || Type == EvmOpcode.StaticCall;

    IInterpreterFrameReader? IInterpreterFrameReader.Parent => Parent;

    public static CallFrame CreateMessageCall(
        int id,
        EvmOpcode type,
        CallFrame parent,
        Address target,
        ReadOnlyMemory<byte> input,
        UInt256 value = default
    ) => new(
        id,
        type,
        parent.Origin,
        parent,
        type == EvmOpcode.DelegateCall
            ? parent.Caller
            : parent.Address,
        type is EvmOpcode.CallCode or EvmOpcode.DelegateCall
            ? parent.Address
            : target,
        target,
        type switch
        {
            EvmOpcode.DelegateCall => parent.Value,
            EvmOpcode.StaticCall => UInt256.Zero,
            _ => value
        },
        input
    );

    public static CallFrame CreateContractCreation(
        int id,
        EvmOpcode type,
        CallFrame parent,
        Address address,
        UInt256 endowment,
        ReadOnlyMemory<byte> initCode
    ) => new(
        id, type, parent.Origin, parent, parent.Address, address, address,
        endowment, initCode
    );

    public static CallFrame CreateSelfDestruct(int id, CallFrame parent, Address beneficiary, UInt256 balance)
        => new(
            id, EvmOpcode.SelfDestruct, parent.Origin, parent, parent.Address, parent.Address, beneficiary,
            balance, ReadOnlyMemory<byte>.Empty
        );
}
