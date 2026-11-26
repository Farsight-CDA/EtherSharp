using EtherSharp.Client;
using EtherSharp.Contract;
using EtherSharp.Numerics;
using EtherSharp.RPC.Transport;
using EtherSharp.Types;
using NSubstitute;

namespace EtherSharp.Tests.Client;

public sealed class EtherModuleTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Should_Create_Unsigned_Transfer_Without_A_Signer(bool useContract)
    {
        var transport = Substitute.For<IRPCTransport>();
        await using var client = EtherClientBuilder.CreateEmpty()
            .WithRPCTransport(transport)
            .WithChainId(1, ChainIdMode.Assume)
            .Build();
        await client.InitializeAsync(forceNoQuery: true);
        transport.ClearReceivedCalls();

        var receiver = Address.Parse("0x0000000000000000000000000000000000000001");
        UInt256 amount = 42;
        var contract = Substitute.For<IPayableContract>();
        contract.Address.Returns(receiver);

        var input = useContract ? client.ETH.Transfer(contract, amount) : client.ETH.Transfer(in receiver, amount);

        Assert.Equal(receiver, input.To);
        Assert.Equal(amount, input.Value);
        Assert.True(input.Data.IsEmpty);
        Assert.Empty(transport.ReceivedCalls());
    }
}
