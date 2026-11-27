using EtherSharp.Client;
using EtherSharp.RPC.Transport;
using EtherSharp.Tx.Nonce;
using EtherSharp.Types;
using EtherSharp.Wallet;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace EtherSharp.Tests.Client;

public sealed class EtherClientDisposalTests
{
    [Fact]
    public async Task Should_Dispose_Provider_Services_And_Transport()
    {
        var transport = Substitute.For<IRPCTransport, IAsyncDisposable>();
        transport.SupportsSubscriptions.Returns(false);

        var builder = EtherClientBuilder.CreateEmpty()
            .WithRPCTransport(transport);

        var asyncSentinel = Substitute.For<IAsyncDisposable>();
        var syncSentinel = Substitute.For<IDisposable>();

        ((IInternalEtherClientBuilder) builder).Services.AddSingleton(_ => asyncSentinel);
        ((IInternalEtherClientBuilder) builder).Services.AddSingleton(_ => syncSentinel);

        var client = builder.Build();

        _ = client.AsInternal().Provider.GetRequiredService<IRPCTransport>();
        _ = client.AsInternal().Provider.GetRequiredService<IAsyncDisposable>();
        _ = client.AsInternal().Provider.GetRequiredService<IDisposable>();

        await client.DisposeAsync();

        Assert.Equal(1, CountCalls((IAsyncDisposable) transport, nameof(IAsyncDisposable.DisposeAsync)));
        Assert.Equal(1, CountCalls(asyncSentinel, nameof(IAsyncDisposable.DisposeAsync)));
        Assert.Equal(1, CountCalls(syncSentinel, nameof(IDisposable.Dispose)));
    }

    [Fact]
    public async Task Should_Not_Dispose_Base_Client_When_Tx_Client_Is_Disposed()
    {
        var (baseClient, transport) = await CreateInitializedClientAsync();
        await using var client = baseClient;
        var nonceAllocator = Substitute.For<INonceAllocator>();

        var txClient = await AttachAsync(client, nonceAllocator);
        await txClient.DisposeAsync();

        Assert.Equal(1, CountCalls(nonceAllocator, nameof(IDisposable.Dispose)));
        Assert.Equal(0, CountCalls((IAsyncDisposable) transport, nameof(IAsyncDisposable.DisposeAsync)));
        Assert.Equal(1UL, client.ChainId);
    }

    [Fact]
    public async Task Should_Dispose_Attached_Tx_Clients_When_Base_Client_Is_Disposed()
    {
        var (client, transport) = await CreateInitializedClientAsync();
        var firstAllocator = Substitute.For<INonceAllocator>();
        var secondAllocator = Substitute.For<INonceAllocator>();

        var firstTxClient = await AttachAsync(client, firstAllocator);
        var secondTxClient = await AttachAsync(client, secondAllocator);
        await client.DisposeAsync();

        Assert.Equal(1, CountCalls(firstAllocator, nameof(IDisposable.Dispose)));
        Assert.Equal(1, CountCalls(secondAllocator, nameof(IDisposable.Dispose)));
        Assert.Equal(1, CountCalls((IAsyncDisposable) transport, nameof(IAsyncDisposable.DisposeAsync)));
        _ = await Assert.ThrowsAsync<ObjectDisposedException>(
            () => AttachAsync(client, Substitute.For<INonceAllocator>())
        );
    }

    private static async Task<(IEtherClient Client, IRPCTransport Transport)> CreateInitializedClientAsync()
    {
        var transport = Substitute.For<IRPCTransport, IAsyncDisposable>();
        transport.SupportsSubscriptions.Returns(false);

        var client = EtherClientBuilder.CreateEmpty()
            .WithRPCTransport(transport)
            .WithChainId(1, ChainIdMode.Assume)
            .Build();
        await client.InitializeAsync(forceNoQuery: true);

        return (client, transport);
    }

    private static Task<IEtherTxClient> AttachAsync(IEtherClient client, INonceAllocator nonceAllocator)
    {
        var signer = Substitute.For<IEtherSigner>();
        signer.Address.Returns(Address.Parse("0x0000000000000000000000000000000000000001"));

        return client.Attach(signer)
            .WithInitialNonce(0)
            .WithNonceAllocator((_, _) => nonceAllocator)
            .BuildAsync();
    }

    private static int CountCalls(object substitute, string methodName)
        => substitute
            .ReceivedCalls()
            .Count(call => call.GetMethodInfo().Name == methodName);
}
