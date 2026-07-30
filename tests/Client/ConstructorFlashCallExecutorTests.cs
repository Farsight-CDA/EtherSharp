using EtherSharp.Client;
using EtherSharp.Client.Services.FlashCallExecutor;
using EtherSharp.Contract;
using EtherSharp.Numerics;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.Tx;
using EtherSharp.Types;
using NSubstitute;

namespace EtherSharp.Tests.Client;

public sealed class ConstructorFlashCallExecutorTests
{
    [Theory]
    [InlineData(0UL)]
    [InlineData(0x0102030405060708UL)]
    public async Task Should_Encode_Shift_Free_Helper_Payload(ulong flashCallGasLimit)
    {
        var ethRpcModule = Substitute.For<IEthRpcModule>();
        ethRpcModule.CallAsync(
            Arg.Any<Address?>(),
            Arg.Any<Address?>(),
            Arg.Any<ulong?>(),
            Arg.Any<UInt256?>(),
            Arg.Any<UInt256>(),
            Arg.Any<ReadOnlyMemory<byte>>(),
            Arg.Any<TargetHeight>(),
            Arg.Any<IReadOnlyDictionary<Address, StateOverride>?>(),
            Arg.Any<BlockOverride?>(),
            Arg.Any<CancellationToken>()
        ).Returns(new TxCallResult(true, new byte[] { 1, 0xAA }));

        var executor = new ConstructorFlashCallExecutor(ethRpcModule, new CallGasLimitSettings(null, null));
        byte[] deploymentData = [0x60, 0x00];
        byte[] callData = [0x12, 0x34, 0x56];

        var result = await executor.ExecuteFlashCallAsync(
            IContractDeployment.Create(new EVMByteCode(deploymentData), 0),
            IFlashCall.ForRawFlashCall(0, callData),
            flashCallGasLimit,
            TargetHeight.Latest,
            CancellationToken.None
        );

        Assert.True(result.Success);
        Assert.Equal(new byte[] { 0xAA }, result.Data);

        var rpcCall = Assert.Single(
            ethRpcModule.ReceivedCalls(),
            call => call.GetMethodInfo().Name == nameof(IEthRpcModule.CallAsync)
        );
        var payload = Assert.IsType<ReadOnlyMemory<byte>>(rpcCall.GetArguments()[5]);
        int helperLength = payload.Length - deploymentData.Length - callData.Length;
        var helperByteCode = new EVMByteCode(payload[..helperLength]);

        Assert.False(helperByteCode.ContainsOpcode(0x1C));
        Assert.Equal(
            EVMByteCode.MAX_INIT_LENGTH - (flashCallGasLimit == 0 ? 37 : 45),
            executor.GetMaxPayloadSize(flashCallGasLimit, TargetHeight.Latest)
        );

        byte[] expectedHelper = Convert.FromHexString(flashCallGasLimit == 0
            ? "383d3d39600260223df03d3d3d6003602434865af181533d8160013e3d60010181f3"
            : "383d3d396002602a3df03d3d3d6003602c3486670102030405060708f181533d8160013e3d60010181f3");

        Assert.Equal(expectedHelper, payload[..helperLength].ToArray());
        Assert.Equal([.. deploymentData, .. callData], payload[helperLength..].ToArray());
    }

    [Theory]
    [InlineData(0, 0, 0UL, 32)]
    [InlineData(0, 0, 1UL, 33)]
    [InlineData(222, 0, 0UL, 33)]
    [InlineData(223, 0, 0UL, 34)]
    [InlineData(256, 256, UInt64.MaxValue, 45)]
    public async Task Should_Use_Minimum_Push_Width(
        int deploymentLength,
        int callLength,
        ulong flashCallGasLimit,
        int expectedHelperLength)
    {
        var ethRpcModule = Substitute.For<IEthRpcModule>();
        ethRpcModule.CallAsync(
            Arg.Any<Address?>(),
            Arg.Any<Address?>(),
            Arg.Any<ulong?>(),
            Arg.Any<UInt256?>(),
            Arg.Any<UInt256>(),
            Arg.Any<ReadOnlyMemory<byte>>(),
            Arg.Any<TargetHeight>(),
            Arg.Any<IReadOnlyDictionary<Address, StateOverride>?>(),
            Arg.Any<BlockOverride?>(),
            Arg.Any<CancellationToken>()
        ).Returns(new TxCallResult(true, new byte[] { 1 }));

        var executor = new ConstructorFlashCallExecutor(ethRpcModule, new CallGasLimitSettings(null, null));

        _ = await executor.ExecuteFlashCallAsync(
            IContractDeployment.Create(new EVMByteCode(new byte[deploymentLength]), 0),
            IFlashCall.ForRawFlashCall(0, new byte[callLength]),
            flashCallGasLimit,
            TargetHeight.Latest,
            CancellationToken.None
        );

        var rpcCall = Assert.Single(
            ethRpcModule.ReceivedCalls(),
            call => call.GetMethodInfo().Name == nameof(IEthRpcModule.CallAsync)
        );
        var payload = Assert.IsType<ReadOnlyMemory<byte>>(rpcCall.GetArguments()[5]);
        int helperLength = payload.Length - deploymentLength - callLength;

        Assert.Equal(expectedHelperLength, helperLength);
        Assert.False(new EVMByteCode(payload[..helperLength]).ContainsOpcode(0x1C));
    }
}
