using EtherSharp.Client;
using EtherSharp.Querier.Config;
using EtherSharp.Types;
using Microsoft.Extensions.DependencyInjection;

namespace EtherSharp.Querier.Extensions;

public static class Extensions
{
    public static EtherClientBuilder AddQuerier(this EtherClientBuilder builder, Address querierAddress)
    {
        var b = (IInternalEtherClientBuilder) builder;

        if(b.Services.Any(x => x.ServiceType == typeof(QuerierConfiguration)))
        {
            throw new InvalidOperationException("Querier already registered");
        }

        b.Services.AddSingleton(new QuerierConfiguration(querierAddress));
        return builder;
    }

    public static IQueryBuilder<T> Query<T>(this IEtherClient client)
    {
        var c = client.AsInternal();
        var config = c.Provider.GetService<QuerierConfiguration>()
            ?? throw new InvalidOperationException($"Querier not configured, make sure to call {nameof(AddQuerier)} during setup");

        var querier = client.Contract<IQuerier>(config.QuerierAddress);

        return new QueryBuilder<T>(client, querier);
    }
}
