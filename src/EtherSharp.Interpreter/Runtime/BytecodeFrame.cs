using EtherSharp.Contract;
using EtherSharp.Interpreter.Runtime.Memory;
using EtherSharp.Interpreter.Runtime.Stack;
using EtherSharp.Interpreter.Runtime.Storage;
using EtherSharp.Interpreter.Runtime.Tracing;

namespace EtherSharp.Interpreter.Runtime;

internal sealed class BytecodeFrame(
    CallFrame call,
    InterpreterAccountStorage accountStorage,
    InterpreterResourceLimits resourceLimits
) : IInterpreterExecution
{
    public CallFrame Call { get; } = call;
    public ZeroPaddedData CallData { get; } = new(call.Type is EvmOpcode.Create or EvmOpcode.Create2
        ? ReadOnlyMemory<byte>.Empty
        : call.Input
    );

    public OperandStack Stack { get; } = new();
    public LinearMemory Memory { get; } = new(resourceLimits.MaxMemorySize);
    public ReturnDataBuffer ReturnData { get; } = new();
    public InterpreterAccountStorage AccountStorage { get; } = accountStorage;

    IInterpreterFrame IInterpreterExecution.Frame => Call;
    IInterpreterStack IInterpreterExecution.Stack => Stack;
    IInterpreterMemory IInterpreterExecution.Memory => Memory;
}
