using EtherSharp.Client.Services;
using EtherSharp.Client.Services.ContractFactory;
using EtherSharp.Client.Services.EtherApi;
using EtherSharp.Client.Services.GasFeeProvider;
using EtherSharp.Client.Services.RPC;
using EtherSharp.Client.Services.TxConfirmer;
using EtherSharp.Client.Services.TxPublisher;
using EtherSharp.Client.Services.TxScheduler;
using EtherSharp.Common.Extensions;
using EtherSharp.Transport;
using EtherSharp.Wallet;
using Microsoft.Extensions.DependencyInjection;

namespace EtherSharp.Client;
public class EtherClientBuilder
{
    private readonly IServiceCollection _services = new ServiceCollection();

    private IRPCTransport? _transport;
    private Action<IContractFactory>? _contractConfigurationAction;

    public EtherClientBuilder WithRPCTransport(IRPCTransport transport)
    {
        _transport = transport;
        return this;
    }

    public EtherClientBuilder WithSigner(IEtherSigner signer)
    {
        _services.AddOrReplaceSingleton<IEtherSigner, IEtherSigner>(signer);
        return this;
    }

    public EtherClientBuilder WithTxScheduler<TTxScheduler>()
        where TTxScheduler : class, ITxScheduler
    {
        _services.AddOrReplaceSingleton<ITxScheduler, TTxScheduler>();
        return this;
    }

    public EtherClientBuilder WithTxPublisher<TTxPublisher>()
        where TTxPublisher : class, ITxPublisher
    {
        _services.AddOrReplaceSingleton<ITxPublisher, TTxPublisher>();
        return this;
    }

    public EtherClientBuilder WithTxConfirmer<TTxConfirmer>()
        where TTxConfirmer : class, ITxConfirmer
    {
        _services.AddOrReplaceSingleton<ITxConfirmer, TTxConfirmer>();
        return this;
    }

    public EtherClientBuilder WithGasFeeProvider<TGasFeeProvider>()
        where TGasFeeProvider : class, IGasFeeProvider
    {
        _services.AddOrReplaceSingleton<IGasFeeProvider, TGasFeeProvider>();
        return this;
    }

    public EtherClientBuilder WithContractConfiguration(Action<IContractFactory> contractSetupAction)
    {
        _contractConfigurationAction = contractSetupAction;
        return this;
    }

    private void AssertReadClientConfiguration()
    {
        if(_transport is null)
        {
            throw new InvalidOperationException($"No RPCTransport configured. Call the {nameof(WithRPCTransport)} method prior to {nameof(BuildReadClient)}.");
        }

        _services.AddSingleton(_transport);
        _services.AddSingleton<EtherApi>();
        _services.AddSingleton<IRpcClient, EvmRpcClient>();
        _services.AddSingleton<ContractFactory>();

        foreach(var service in _services.ToArray())
        {
            var serviceType = service.ImplementationType
                ?? service.ImplementationInstance?.GetType()
                ?? service.ServiceType;

            if (serviceType.GetInterface(nameof(IInitializableService)) is null)
            {
                continue;
            }

            _services.AddSingleton(typeof(IInitializableService), provider => provider.GetRequiredService(service.ServiceType));
        }
    }
    public IEtherClient BuildReadClient()
    {
        AssertReadClientConfiguration();

        _services.AddSingleton<IEtherClient>(provider => new EtherClient(provider, false));

        var provider = _services.BuildServiceProvider();

        _contractConfigurationAction?.Invoke(provider.GetRequiredService<ContractFactory>());

        return provider.GetRequiredService<IEtherClient>();
    }

    private void AssertTxClientConfiguration()
    {
        AssertReadClientConfiguration();

        if(!_services.Any(x => x.ServiceType == typeof(IEtherSigner)))
        {
            throw new InvalidOperationException($"No {nameof(IEtherSigner)} configured. Call the {nameof(WithSigner)} method prior to {nameof(BuildTxClient)}");
        }
        if(!_services.Any(x => x.ServiceType == typeof(ITxScheduler)))
        {
            throw new InvalidOperationException($"No {nameof(ITxScheduler)} configured. Call the {nameof(WithTxScheduler)} method prior to {nameof(BuildTxClient)}");
        }
        if(!_services.Any(x => x.ServiceType == typeof(ITxPublisher)))
        {
            throw new InvalidOperationException($"No {nameof(ITxPublisher)} configured. Call the {nameof(WithTxPublisher)} method prior to {nameof(BuildTxClient)}");
        }
        if(!_services.Any(x => x.ServiceType == typeof(ITxConfirmer)))
        {
            throw new InvalidOperationException($"No {nameof(ITxConfirmer)} configured. Call the {nameof(WithTxConfirmer)} method prior to {nameof(BuildTxClient)}");
        }
        if(!_services.Any(x => x.ServiceType == typeof(IGasFeeProvider)))
        {
            throw new InvalidOperationException($"No {nameof(IGasFeeProvider)} configured. Call the {nameof(WithGasFeeProvider)} method prior to {nameof(BuildTxClient)}");
        }
    }
    public IEtherTxClient BuildTxClient()
    {
        AssertTxClientConfiguration();

        _services.AddSingleton<IEtherClient>(provider => provider.GetRequiredService<IEtherTxClient>());
        _services.AddSingleton<IEtherTxClient>(provider => new EtherClient(provider, true));

        var provider = _services.BuildServiceProvider();

        _contractConfigurationAction?.Invoke(provider.GetRequiredService<ContractFactory>());

        return provider.GetRequiredService<IEtherTxClient>();
    }
}
