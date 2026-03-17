using EtherSharp.Client;
using EtherSharp.RPC.Transport;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace EtherSharp.Tests.Client;

public sealed class EtherClientDisposalTests
{
    [Fact]
    public async Task Should_Dispose_Provider_Services_And_Transport()
    {
        var transport = Substitute.For<IRPCTransport, IAsyncDisposable>();
        transport.SupportsFilters.Returns(true);
        transport.SupportsSubscriptions.Returns(false);

        var builder = EtherClientBuilder.CreateEmpty()
            .WithRPCTransport(transport);

        var asyncSentinel = Substitute.For<IAsyncDisposable>();
        var syncSentinel = Substitute.For<IDisposable>();

        ((IInternalEtherClientBuilder) builder).Services.AddSingleton(_ => asyncSentinel);
        ((IInternalEtherClientBuilder) builder).Services.AddSingleton(_ => syncSentinel);

        var client = builder.BuildReadClient();

        _ = client.AsInternal().Provider.GetRequiredService<IRPCTransport>();
        _ = client.AsInternal().Provider.GetRequiredService<IAsyncDisposable>();
        _ = client.AsInternal().Provider.GetRequiredService<IDisposable>();

        await client.DisposeAsync();

        Assert.Equal(1, CountCalls((IAsyncDisposable) transport, nameof(IAsyncDisposable.DisposeAsync)));
        Assert.Equal(1, CountCalls(asyncSentinel, nameof(IAsyncDisposable.DisposeAsync)));
        Assert.Equal(1, CountCalls(syncSentinel, nameof(IDisposable.Dispose)));
    }

    private static int CountCalls(object substitute, string methodName)
        => substitute
            .ReceivedCalls()
            .Count(call => call.GetMethodInfo().Name == methodName);
}
