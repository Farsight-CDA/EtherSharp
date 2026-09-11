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
    ReadOnlyMemory<byte> Input,
    GasBudget Gas
) : IInterpreterFrame
{
    public const int MAX_DEPTH = 1024;

    public Address From => Parent?.Address ?? Origin;
    public int Depth { get; } = Parent is null ? 0 : Parent.Depth + 1;
    public bool IsStatic { get; } = Parent?.IsStatic == true || Type == EvmOpcode.StaticCall;

    IInterpreterFrame? IInterpreterFrame.Parent => Parent;

    public static CallFrame CreateMessageCall(
        int id,
        EvmOpcode type,
        CallFrame parent,
        UInt256 requestedGas,
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
        input,
        ForwardGas(parent, requestedGas)
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
        endowment, initCode, ForwardGas(parent, UInt256.MaxValue)
    );

    private static GasBudget ForwardGas(CallFrame parent, UInt256 requestedGas)
    {
        // EIP-150: reserve one sixty-fourth of the caller's available execution gas.
        // CALL overhead and the value stipend will be applied with dynamic opcode pricing.
        ulong maximum = parent.Gas.Remaining - (parent.Gas.Remaining / 64);
        ulong amount = requestedGas < (UInt256) maximum ? (ulong) requestedGas : maximum;
        return parent.Gas.Forward(amount);
    }
}
