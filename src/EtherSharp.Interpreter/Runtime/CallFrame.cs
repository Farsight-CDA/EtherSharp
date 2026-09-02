using EtherSharp.Interpreter.Memory;
using EtherSharp.Interpreter.Stack;
using EtherSharp.Interpreter.Storage;
using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Runtime;

internal sealed class CallFrame(
    Address origin,
    Address from,
    Address to,
    Address codeAddress,
    UInt256 value,
    ReadOnlyMemory<byte> callData,
    InterpreterAccountStorage accountStorage,
    InterpreterOptions options,
    int depth,
    bool isStatic
)
{
    public const int MAX_DEPTH = 1024;

    public Address Origin { get; } = origin;
    public Address From { get; } = from;
    public Address To { get; } = to;
    public Address CodeAddress { get; } = codeAddress;
    public UInt256 Value { get; } = value;
    public int Depth { get; } = depth;
    public bool IsStatic { get; } = isStatic;
    public ZeroPaddedData CallData { get; } = new(callData);
    public OperandStack Stack { get; } = new();
    public LinearMemory Memory { get; } = new(options.MaxMemorySize);
    public ReturnDataBuffer ReturnData { get; } = new();
    public InterpreterAccountStorage AccountStorage { get; } = accountStorage;

    public TxCallResult Revert(ReadOnlyMemory<byte> data = default)
        => new(false, data);
}
