using EtherSharp.Contract;
using EtherSharp.Interpreter.Forking;
using EtherSharp.Interpreter.Runtime;
using EtherSharp.Interpreter.Runtime.Tracing;
using EtherSharp.Numerics;
using EtherSharp.Tx;
using EtherSharp.Types;
using NSubstitute;

namespace EtherSharp.Tests.Interpreter.Runtime;

public sealed class InstructionHookTests
{
    private static readonly Address _target = Address.FromString("0x0000000000000000000000000000000000001000");

    [Fact]
    public async Task InstructionHook_ShouldBeAwaitedAndReceiveInstructionLocals()
    {
        using var runtime = CreateRuntime();
        var gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var instructions = new List<(int, EvmOpcode)>();
        var hooks = new Hooks((pc, opcode) =>
        {
            instructions.Add((pc, opcode));
            entered.TrySetResult();
            return new ValueTask(gate.Task);
        });

        var execution = runtime.SimulateCallAsync(Address.Zero, new Input(), WithCode("60015000"), hooks);
        await entered.Task.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.False(execution.IsCompleted);
        Assert.Equal((0, EvmOpcode.Push1), Assert.Single(instructions));
        gate.SetResult();

        Assert.True((await execution).Success);
        Assert.Equal(new[] { (0, EvmOpcode.Push1), (2, EvmOpcode.Pop), (3, EvmOpcode.Stop) }, instructions);
    }

    [Fact]
    public async Task InstructionHookException_ShouldRestoreStateAndAllowUntracedReuse()
    {
        using var runtime = CreateRuntime();
        int calls = 0;
        var hooks = new Hooks((_, opcode) =>
        {
            calls++;
            return opcode == EvmOpcode.Stop
                ? throw new InvalidOperationException("Tracer failure")
                : ValueTask.CompletedTask;
        });

        // Write slot zero, then throw from the callback preceding STOP.
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await runtime.SimulateCallAsync(Address.Zero, new Input(), WithCode("602a5f5500"), hooks)
        );
        int previousCalls = calls;

        var read = await runtime.SimulateCallAsync(Address.Zero, new Input(), WithCode("5f545f5260205ff3"));
        Assert.True(read.Success);
        Assert.Equal(new byte[32], read.Data.ToArray());
        Assert.Equal(previousCalls, calls);
    }

    private static InterpreterRuntime CreateRuntime()
    {
        var provider = Substitute.For<IInterpreterDataProvider>();
        provider.FetchAsync(Arg.Any<InterpreterContext>(), Arg.Any<ReadOnlyMemory<InterpreterDataRequest>>())
            .Returns(Task.FromResult<IReadOnlyList<InterpreterDataResult>>(
                [
                    new InterpreterDataResult.Nonce(Address.Zero, 0),
                    new InterpreterDataResult.Storage(_target, Bytes32.Zero, Bytes32.Zero)
                ]
            ));
        return new InterpreterStateFork(provider, new InterpreterContext(
            1, 1, DateTimeOffset.UnixEpoch, [], UInt256.Zero, UInt256.Zero,
            Address.Zero, UInt256.Zero, (UInt256) 30_000_000
        )).CreateInterpreter();
    }

    private static InterpreterSimulationOptions WithCode(string code)
        => new()
        {
            StateOverrides = new Dictionary<Address, AccountOverride>
            {
                [_target] = new(code: Convert.FromHexString(code))
            }
        };

    private sealed class Input : ITxInput
    {
        public Address? To => _target;
        public UInt256 Value => UInt256.Zero;
        public ReadOnlyMemory<byte> Data => ReadOnlyMemory<byte>.Empty;
    }

    private sealed class Hooks(Func<int, EvmOpcode, ValueTask> callback) : IInterpreterExecutionHooks
    {
        public ValueTask OnInstructionAsync(
            IInterpreterFrameReader frame,
            int programCounter,
            EvmOpcode opcode,
            IInterpreterStateReader state
        ) => callback(programCounter, opcode);
    }
}
