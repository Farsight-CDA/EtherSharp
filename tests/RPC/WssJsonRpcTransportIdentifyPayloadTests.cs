using EtherSharp.RPC.Transport;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace EtherSharp.Tests.RPC;

public class WssJsonRpcTransportIdentifyPayloadTests
{
    [Fact]
    public void Should_Identify_Response_With_Numeric_Id()
    {
        var (payloadType, requestId, subscriptionId) = Identify("{\"jsonrpc\":\"2.0\",\"id\":1,\"result\":\"0x1\"}");

        Assert.Equal(WssJsonRpcTransport.PayloadType.Response, payloadType);
        Assert.Equal(1, requestId);
        Assert.Null(subscriptionId);
    }

    [Fact]
    public void Should_Identify_Response_With_String_Id()
    {
        var (hexPayloadType, hexRequestId, hexSubscriptionId) = Identify("{\"jsonrpc\":\"2.0\",\"id\":\"0x1A\",\"result\":\"0x1\"}");
        Assert.Equal(WssJsonRpcTransport.PayloadType.Response, hexPayloadType);
        Assert.Equal(26, hexRequestId);
        Assert.Null(hexSubscriptionId);

        var (decimalPayloadType, decimalRequestId, decimalSubscriptionId) = Identify("{\"jsonrpc\":\"2.0\",\"id\":\"42\",\"result\":\"0x1\"}");
        Assert.Equal(WssJsonRpcTransport.PayloadType.Response, decimalPayloadType);
        Assert.Equal(42, decimalRequestId);
        Assert.Null(decimalSubscriptionId);
    }

    [Fact]
    public void Should_Identify_Subscription_Payload()
    {
        string expectedSubscriptionId = "0xfeedbeef";
        string payload =
            $"{{\"jsonrpc\":\"2.0\",\"method\":\"eth_subscription\",\"params\":{{\"subscription\":\"{expectedSubscriptionId}\",\"result\":{{\"number\":\"0x1\"}}}}}}";

        var (payloadType, requestId, subscriptionId) = Identify(payload);

        Assert.Equal(WssJsonRpcTransport.PayloadType.Subscription, payloadType);
        Assert.Equal(-1, requestId);
        Assert.Equal(expectedSubscriptionId, subscriptionId);
    }

    [Fact]
    public void Should_Identify_Unknown_Payload_When_No_Markers_Are_Present()
    {
        var (payloadType, requestId, subscriptionId) = Identify("{\"jsonrpc\":\"2.0\",\"method\":\"eth_newHeads\"}");

        Assert.Equal(WssJsonRpcTransport.PayloadType.Unknown, payloadType);
        Assert.Equal(-1, requestId);
        Assert.Null(subscriptionId);
    }

    private static (WssJsonRpcTransport.PayloadType PayloadType, int RequestId, string? SubscriptionId) Identify(string payload)
    {
        IServiceProvider provider = new ServiceCollection().BuildServiceProvider();
        using var transport = new WssJsonRpcTransport(new Uri("ws://127.0.0.1:1/"), TimeSpan.FromSeconds(1), provider);
        transport.IdentifyPayload(Encoding.UTF8.GetBytes(payload), out var payloadType, out int requestId, out string subscriptionId);
        return (payloadType, requestId, subscriptionId);
    }
}
