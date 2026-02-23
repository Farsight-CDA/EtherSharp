using EtherSharp.Client;
using EtherSharp.Common;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace EtherSharp.Tests.Client;

public class JsonSerializerOptionsTests
{
    [Fact]
    public void WithJsonSerializerOptions_UsesConfiguredOptions()
    {
        var customJsonSerializerOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = null,
            WriteIndented = true
        };

        var client = EtherClientBuilder
            .CreateForHttpRpc("http://localhost")
            .WithJsonSerializerOptions(customJsonSerializerOptions)
            .BuildReadClient();

        var clientOptions = GetSerializerOptions(client);

        Assert.NotSame(customJsonSerializerOptions, clientOptions);
        Assert.Null(clientOptions.PropertyNamingPolicy);
        Assert.True(clientOptions.WriteIndented);
    }

    [Fact]
    public void BuildReadClient_UsesDistinctDefaultSerializerOptionsPerClient()
    {
        var client1 = EtherClientBuilder
            .CreateForHttpRpc("http://localhost")
            .BuildReadClient();

        var client2 = EtherClientBuilder
            .CreateForHttpRpc("http://localhost")
            .BuildReadClient();

        var client1Options = GetSerializerOptions(client1);
        var client2Options = GetSerializerOptions(client2);

        Assert.NotSame(client1Options, client2Options);
        Assert.NotSame(client1Options, ParsingUtils.EvmSerializerOptions);
        Assert.NotSame(client2Options, ParsingUtils.EvmSerializerOptions);
    }

    private static JsonSerializerOptions GetSerializerOptions(IEtherClient client)
        => client.AsInternal().Provider.GetRequiredService<JsonSerializerOptions>();
}
