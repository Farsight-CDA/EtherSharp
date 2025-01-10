﻿using EtherSharp.Client.Services;
using EtherSharp.Client.Services.ContractFactory;
using EtherSharp.Client.Services.EtherApi;
using EtherSharp.Client.Services.GasFeeProvider;
using EtherSharp.Client.Services.RPC;
using EtherSharp.Client.Services.TxConfirmer;
using EtherSharp.Client.Services.TxPublisher;
using EtherSharp.Client.Services.TxScheduler;
using EtherSharp.Client.Services.TxTypeHandler;
using EtherSharp.Common.Extensions;
using EtherSharp.Transport;
using EtherSharp.Tx.EIP1559;
using EtherSharp.Tx.Types;
using EtherSharp.Wallet;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EtherSharp.Client;
public class EtherClientBuilder
{
    private readonly IServiceCollection _services = new ServiceCollection();

    private IRPCTransport? _transport;

    private Action<IContractFactory>? _contractConfigurationAction;
    private Action<ITxConfirmer>? _txConfirmerConfigureAction;
    private Action<ITxPublisher>? _txPublisherConfigureAction;
    private Action<ITxScheduler>? _txSchedulerConfigureAction;

    private EtherClientBuilder() { }

    public static EtherClientBuilder CreateEmpty() => new EtherClientBuilder();
    public static EtherClientBuilder CreateForWebsocket(string websocketUrl, TimeSpan? requestTimeout = null, IEtherSigner? signer = null)
        => CreateForWebsocket(new Uri(websocketUrl, UriKind.Absolute), requestTimeout, signer);
    public static EtherClientBuilder CreateForWebsocket(Uri websocketUri, TimeSpan? requestTimeout = null, 
        IEtherSigner? signer = null, ILoggerFactory? loggerFactory = null)
    {
        requestTimeout ??= TimeSpan.FromSeconds(30);

        var builder = new EtherClientBuilder()
            .WithRPCTransport(new WssJsonRpcTransport(websocketUri, requestTimeout.Value, loggerFactory?.CreateLogger<WssJsonRpcTransport>()));

        if(loggerFactory is not null)
        {
            builder.WithLoggerFactory(loggerFactory);
        }
        if (signer is null)
        {
            return builder;
        }
        //
        return builder
            .WithSigner(signer)
            .WithTxPublisher<BasicTxPublisher>()
            .WithTxConfirmer<PollingTxConfirmer>()
            .WithTxScheduler<BlockingSequentialTxScheduler>()
            .AddTxTypeHandler<EIP1559TxTypeHandler, EIP1559GasFeeProvider, EIP1559Transaction, EIP1559TxParams, EIP1559GasParams>();
    }
    public static EtherClientBuilder CreateForHttpRpc(string websocketUrl, IEtherSigner? signer = null, ILoggerFactory? loggerFactory = null)
    {
        var builder = new EtherClientBuilder()
            .WithRPCTransport(new HttpJsonRpcTransport(new Uri(websocketUrl, UriKind.Absolute)));

        if(loggerFactory is not null)
        {
            builder.WithLoggerFactory(loggerFactory);
        }
        if(signer is null)
        {
            return builder;
        }
        //
        return builder
            .WithSigner(signer)
            .WithTxPublisher<BasicTxPublisher>()
            .WithTxConfirmer<PollingTxConfirmer>()
            .WithTxScheduler<BlockingSequentialTxScheduler>()
            .AddTxTypeHandler<EIP1559TxTypeHandler, EIP1559GasFeeProvider, EIP1559Transaction, EIP1559TxParams, EIP1559GasParams>();
    }

    public EtherClientBuilder WithOPStackConfiguration() 
        => AddTxTypeHandler<EIP1559TxTypeHandler, OpStackEIP1559GasFeeProvider, EIP1559Transaction, EIP1559TxParams, EIP1559GasParams>();

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

    public EtherClientBuilder WithLoggerFactory(ILoggerFactory loggerFactory)
    {
        _services.AddOrReplaceSingleton(loggerFactory);
        return this;
    }

    public EtherClientBuilder WithTxScheduler<TTxScheduler>(Action<ITxScheduler>? configureAction = null)
        where TTxScheduler : class, ITxScheduler
    {
        _services.AddOrReplaceSingleton<ITxScheduler, TTxScheduler>();
        _txSchedulerConfigureAction = configureAction;
        return this;
    }

    public EtherClientBuilder WithTxPublisher<TTxPublisher>(Action<ITxPublisher>? configureAction = null)
        where TTxPublisher : class, ITxPublisher
    {
        _services.AddOrReplaceSingleton<ITxPublisher, TTxPublisher>();
        _txPublisherConfigureAction = configureAction;
        return this;
    }

    public EtherClientBuilder WithTxConfirmer<TTxConfirmer>(Action<ITxConfirmer>? configureAction = null)
        where TTxConfirmer : class, ITxConfirmer
    {
        _services.AddOrReplaceSingleton<ITxConfirmer, TTxConfirmer>();
        _txConfirmerConfigureAction = configureAction;
        return this;
    }

    public EtherClientBuilder AddTxTypeHandler<TTxTypeHandler, TGasFeeProvider, TTransaction, TTxParams, TTxGasParams>()
        where TTxTypeHandler : class, ITxTypeHandler<TTransaction, TTxParams, TTxGasParams>
        where TGasFeeProvider : class, IGasFeeProvider<TTxParams, TTxGasParams>
        where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams
    {
        _services.AddOrReplaceSingleton<ITxTypeHandler<TTransaction, TTxParams, TTxGasParams>, TTxTypeHandler>();
        _services.AddOrReplaceSingleton<IGasFeeProvider<TTxParams, TTxGasParams>, TGasFeeProvider>();
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

            if(serviceType.GetInterface(nameof(IInitializableService)) is null)
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
    }
    public IEtherTxClient BuildTxClient()
    {
        AssertTxClientConfiguration();

        _services.AddSingleton<IEtherClient>(provider => provider.GetRequiredService<IEtherTxClient>());
        _services.AddSingleton<IEtherTxClient>(provider => new EtherClient(provider, true));

        var provider = _services.BuildServiceProvider();

        _contractConfigurationAction?.Invoke(provider.GetRequiredService<ContractFactory>());
        _txConfirmerConfigureAction?.Invoke(provider.GetRequiredService<ITxConfirmer>());
        _txPublisherConfigureAction?.Invoke(provider.GetRequiredService<ITxPublisher>());
        _txSchedulerConfigureAction?.Invoke(provider.GetRequiredService<ITxScheduler>());

        return provider.GetRequiredService<IEtherTxClient>();
    }
}
