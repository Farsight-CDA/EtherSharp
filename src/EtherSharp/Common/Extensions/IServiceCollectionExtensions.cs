using Microsoft.Extensions.DependencyInjection;

namespace EtherSharp.Common.Extensions;

internal static class IServiceCollectionExtensions
{
    internal static void AddOrReplaceSingleton<TService, TImplementation>(this IServiceCollection services)
        where TImplementation : class, TService
        where TService : class
    {
        var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(TService));
        if(descriptor is not null)
        {
            services.Remove(descriptor);
        }

        services.AddSingleton<TService, TImplementation>();
    }

    internal static void AddOrReplaceSingleton<TService, TImplementation>(this IServiceCollection services, TImplementation instance)
        where TImplementation : class, TService
        where TService : class
    {
        var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(TService));
        if(descriptor is not null)
        {
            services.Remove(descriptor);
        }

        services.AddSingleton<TService>(instance);
    }

    internal static void AddOrReplaceSingleton<TImplementation>(this IServiceCollection services, TImplementation instance)
        where TImplementation : class
    {
        var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(TImplementation));
        if(descriptor is not null)
        {
            services.Remove(descriptor);
        }

        services.AddSingleton(instance);
    }

    internal static void AddOrReplaceSingleton<TService>(this IServiceCollection services, Func<IServiceProvider, TService> implementationFactory)
        where TService : class
    {
        var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(TService));
        if(descriptor is not null)
        {
            services.Remove(descriptor);
        }

        services.AddSingleton(implementationFactory);
    }
}
