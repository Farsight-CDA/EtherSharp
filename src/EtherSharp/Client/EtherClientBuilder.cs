using EtherSharp.Client.Modules.Blocks;
using EtherSharp.Client.Modules.Ether;
using EtherSharp.Client.Modules.Trace;
using EtherSharp.Client.Services;
using EtherSharp.Client.Services.ContractFactory;
using EtherSharp.Client.Services.FlashCallExecutor;
using EtherSharp.Client.Services.GasFeeProvider;
using EtherSharp.Client.Services.QueryExecutor;
using EtherSharp.Client.Services.ResiliencyLayer;
using EtherSharp.Client.Services.Subscriptions;
using EtherSharp.Client.Services.TxPublisher;
using EtherSharp.Client.Services.TxScheduler;
using EtherSharp.Client.Services.TxTypeHandler;
using EtherSharp.Common;
using EtherSharp.Common.Extensions;
using EtherSharp.RPC;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.RPC.Modules.Trace;
using EtherSharp.RPC.Transport;
using EtherSharp.Transport;
using EtherSharp.Tx.EIP1559;
using EtherSharp.Tx.Legacy;
using EtherSharp.Tx.Types;
using EtherSharp.Types;
using EtherSharp.Wallet;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics.Metrics;

namespace EtherSharp.Client;

/// <summary>
/// Builder for creating an EtherClient.
/// </summary>
public class EtherClientBuilder : IInternalEtherClientBuilder
{
    private readonly IServiceCollection _services = new ServiceCollection();
    IServiceCollection IInternalEtherClientBuilder.Services => _services;

    private Func<IServiceProvider, IRPCTransport>? _transportRegistration;

    private readonly List<(Type ServiceType, Type ActionType, Action<object> Action)> _configureActions = [];
    private Action<IContractFactory>? _contractConfigurationAction;

    private EtherClientBuilder() { }

    private void AddConfigureAction<TService, TActionParam>(Action<TActionParam>? configureAction)
    {
        if(configureAction is null)
        {
            return;
        }

        _configureActions.RemoveAll(x => x.ServiceType == typeof(TService));
        _configureActions.Add((typeof(TService), typeof(TActionParam), value => configureAction((TActionParam) value)));
    }

    /// <summary>
    /// Creates an empty unconfigured EtherClientBuilder.
    /// </summary>
    /// <returns></returns>
    public static EtherClientBuilder CreateEmpty()
        => new EtherClientBuilder();

    /// <summary>
    /// Creates an EtherClientBuilder preconfigured for a websocket RPC.
    /// </summary>
    /// <param name="websocketUrl"></param>
    /// <param name="requestTimeout"></param>
    /// <param name="signer"></param>
    /// <param name="loggerFactory"></param>
    /// <param name="configureGasProvider"></param>
    /// <returns></returns>
    public static EtherClientBuilder CreateForWebsocket(
        string websocketUrl, TimeSpan? requestTimeout = null,
        IEtherSigner? signer = null, ILoggerFactory? loggerFactory = null,
        Action<EIP1559GasFeeProvider>? configureGasProvider = null
    )
        => CreateForWebsocket(new Uri(websocketUrl, UriKind.Absolute), requestTimeout, signer, loggerFactory, configureGasProvider);

    /// <summary>
    /// Creates an EtherClientBuilder preconfigured for a websocket RPC.
    /// </summary>
    /// <param name="websocketUri"></param>
    /// <param name="requestTimeout"></param>
    /// <param name="signer"></param>
    /// <param name="loggerFactory"></param>
    /// <param name="configureGasProvider"></param>
    /// <returns></returns>
    public static EtherClientBuilder CreateForWebsocket(Uri websocketUri, TimeSpan? requestTimeout = null,
        IEtherSigner? signer = null, ILoggerFactory? loggerFactory = null,
        Action<EIP1559GasFeeProvider>? configureGasProvider = null
    )
    {
        requestTimeout ??= TimeSpan.FromSeconds(30);

        var builder = new EtherClientBuilder
        {
            _transportRegistration = provider =>
                new WssJsonRpcTransport(websocketUri, requestTimeout.Value, provider)
        };

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
            .WithTxScheduler<BlockingSequentialTxSchedulerV1>()
            .AddTxTypeHandler<EIP1559TxTypeHandler, EIP1559GasFeeProvider, EIP1559Transaction, EIP1559TxParams, EIP1559GasParams>(
                gasFeeProviderConfigureAction: configureGasProvider
            )
            .AddTxTypeHandler<LegacyTxTypeHandler, LegacyGasFeeProvider, LegacyTransaction, LegacyTxParams, LegacyGasParams>();
    }

    /// <summary>
    /// Creates an EtherClientBuilder preconfigured for a HTTP RPC.
    /// </summary>
    /// <param name="websocketUrl"></param>
    /// <param name="signer"></param>
    /// <param name="loggerFactory"></param>
    /// <returns></returns>
    public static EtherClientBuilder CreateForHttpRpc(string websocketUrl, IEtherSigner? signer = null, ILoggerFactory? loggerFactory = null)
    {
        var builder = new EtherClientBuilder()
        {
            _transportRegistration = provider =>
                new HttpJsonRpcTransport(new Uri(websocketUrl, UriKind.Absolute))
        };

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
            .WithTxScheduler<BlockingSequentialTxSchedulerV1>()
            .AddTxTypeHandler<EIP1559TxTypeHandler, EIP1559GasFeeProvider, EIP1559Transaction, EIP1559TxParams, EIP1559GasParams>()
            .AddTxTypeHandler<LegacyTxTypeHandler, LegacyGasFeeProvider, LegacyTransaction, LegacyTxParams, LegacyGasParams>();
    }

    /// <summary>
    /// Configures the RPC transport.
    /// </summary>
    /// <param name="transport"></param>
    /// <returns></returns>
    public EtherClientBuilder WithRPCTransport(IRPCTransport transport)
    {
        _transportRegistration = provider => transport;
        return this;
    }

    /// <summary>
    /// Configures the RPC transport via a registration action.
    /// </summary>
    /// <param name="transportRegistration"></param>
    /// <returns></returns>
    public EtherClientBuilder WithRPCTransport(Func<IServiceProvider, IRPCTransport> transportRegistration)
    {
        _transportRegistration = transportRegistration;
        return this;
    }

    /// <summary>
    /// Registers the RPC middleware.
    /// </summary>
    /// <typeparam name="TRpcMiddleware"></typeparam>
    /// <param name="middleware"></param>
    /// <returns></returns>
    public EtherClientBuilder AddRPCMiddleware<TRpcMiddleware>(TRpcMiddleware middleware)
         where TRpcMiddleware : class, IRpcMiddleware
    {
        _services.AddSingleton<IRpcMiddleware>(middleware);
        return this;
    }

    /// <summary>
    /// Registers the RPC middleware.
    /// </summary>
    /// <typeparam name="TRpcMiddleware"></typeparam>
    /// <returns></returns>
    public EtherClientBuilder AddRPCMiddleware<TRpcMiddleware>()
        where TRpcMiddleware : class, IRpcMiddleware
    {
        _services.AddSingleton<IRpcMiddleware, TRpcMiddleware>();
        return this;
    }

    /// <summary>
    /// Configures the signer.
    /// </summary>
    /// <param name="signer"></param>
    /// <returns></returns>
    public EtherClientBuilder WithSigner(IEtherSigner signer)
    {
        _services.AddOrReplaceSingleton<IEtherSigner, IEtherSigner>(signer);
        return this;
    }

    /// <summary>
    /// Configures the LoggerFactory.
    /// </summary>
    /// <param name="loggerFactory"></param>
    /// <returns></returns>
    public EtherClientBuilder WithLoggerFactory(ILoggerFactory loggerFactory)
    {
        _services.AddOrReplaceSingleton(loggerFactory);
        return this;
    }

    /// <summary>
    /// Configures the TxScheduler.
    /// </summary>
    /// <typeparam name="TTxScheduler"></typeparam>
    /// <param name="configureAction"></param>
    /// <returns></returns>
    public EtherClientBuilder WithTxScheduler<TTxScheduler>(Action<TTxScheduler>? configureAction = null)
        where TTxScheduler : class, ITxScheduler
    {
        _services.AddOrReplaceSingleton<ITxScheduler, TTxScheduler>();
        AddConfigureAction<ITxScheduler, TTxScheduler>(configureAction);
        return this;
    }

    /// <summary>
    /// Configures the SubscriptionsManager.
    /// </summary>
    /// <typeparam name="TSusbcriptionManager"></typeparam>
    /// <param name="configureAction"></param>
    /// <returns></returns>
    public EtherClientBuilder WithSubscriptionsManager<TSusbcriptionManager>(Action<TSusbcriptionManager>? configureAction = null)
        where TSusbcriptionManager : class, ISubscriptionsManager
    {
        _services.AddOrReplaceSingleton<ISubscriptionsManager, TSusbcriptionManager>();
        AddConfigureAction<ISubscriptionsManager, TSusbcriptionManager>(configureAction);
        return this;
    }

    /// <summary>
    /// Configures the TxPublisher.
    /// </summary>
    /// <typeparam name="TTxPublisher"></typeparam>
    /// <param name="configureAction"></param>
    /// <returns></returns>
    public EtherClientBuilder WithTxPublisher<TTxPublisher>(Action<TTxPublisher>? configureAction = null)
        where TTxPublisher : class, ITxPublisher
    {
        _services.AddOrReplaceSingleton<ITxPublisher, TTxPublisher>();
        AddConfigureAction<ITxPublisher, TTxPublisher>(configureAction);
        return this;
    }

    /// <summary>
    /// Configures the ResiliencyLayer.
    /// </summary>
    /// <typeparam name="TResiliencyLayer"></typeparam>
    /// <param name="configureAction"></param>
    /// <returns></returns>
    public EtherClientBuilder WithResiliencyLayer<TResiliencyLayer>(Action<TResiliencyLayer>? configureAction = null)
        where TResiliencyLayer : class, IResiliencyLayer
    {
        _services.AddOrReplaceSingleton<IResiliencyLayer, TResiliencyLayer>();
        AddConfigureAction<IResiliencyLayer, TResiliencyLayer>(configureAction);
        return this;
    }

    /// <summary>
    /// Configures a Transaction type.
    /// </summary>
    /// <typeparam name="TTxTypeHandler"></typeparam>
    /// <typeparam name="TGasFeeProvider"></typeparam>
    /// <typeparam name="TTransaction"></typeparam>
    /// <typeparam name="TTxParams"></typeparam>
    /// <typeparam name="TTxGasParams"></typeparam>
    /// <param name="handlerConfigureAction"></param>
    /// <param name="gasFeeProviderConfigureAction"></param>
    /// <returns></returns>
    public EtherClientBuilder AddTxTypeHandler<TTxTypeHandler, TGasFeeProvider, TTransaction, TTxParams, TTxGasParams>(
        Action<TTxTypeHandler>? handlerConfigureAction = null,
        Action<TGasFeeProvider>? gasFeeProviderConfigureAction = null
    )
        where TTxTypeHandler : class, ITxTypeHandler<TTransaction, TTxParams, TTxGasParams>
        where TGasFeeProvider : class, IGasFeeProvider<TTxParams, TTxGasParams>
        where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams
    {
        _services.AddOrReplaceSingleton<ITxTypeHandler<TTransaction, TTxParams, TTxGasParams>, TTxTypeHandler>();
        _services.AddOrReplaceSingleton<IGasFeeProvider<TTxParams, TTxGasParams>, TGasFeeProvider>();

        AddConfigureAction<ITxTypeHandler<TTransaction, TTxParams, TTxGasParams>, TTxTypeHandler>(handlerConfigureAction);
        AddConfigureAction<IGasFeeProvider<TTxParams, TTxGasParams>, TGasFeeProvider>(gasFeeProviderConfigureAction);
        return this;
    }

    /// <summary>
    /// Configures the instrumentation.
    /// </summary>
    /// <param name="meterFactory"></param>
    /// <param name="instrumentNamePrefix"></param>
    /// <param name="tags"></param>
    /// <returns></returns>
    public EtherClientBuilder WithInstrumentation(IMeterFactory meterFactory, string instrumentNamePrefix = "ethersharp.", IReadOnlyDictionary<string, object?>? tags = null)
    {
        _services.AddOrReplaceSingleton(meterFactory);
        _services.AddOrReplaceSingleton(new InstrumentationOptions(instrumentNamePrefix, [.. tags ?? new Dictionary<string, object?>()]));
        return this;
    }

    /// <summary>
    /// Configures the ContractFactory.
    /// </summary>
    /// <param name="contractSetupAction"></param>
    /// <returns></returns>
    public EtherClientBuilder WithContractConfiguration(Action<IContractFactory> contractSetupAction)
    {
        _contractConfigurationAction = contractSetupAction;
        return this;
    }

    /// <summary>
    /// Configures the client to use a deployed FlashCall contract.
    /// </summary>
    /// <param name="contractAddress"></param>
    /// <param name="allowFallback"></param>
    /// <param name="maxPayloadSize"></param>
    /// <param name="maxResultSize"></param>
    /// <returns></returns>
    public EtherClientBuilder WithFlashCallContract(Address contractAddress, bool allowFallback = true,
        int maxPayloadSize = 3 * 1024 * 1024, int maxResultSize = 3 * 1024 * 1024)
    {
        _services.AddOrReplaceSingleton(new DeployedFlashCallExecutorConfiguration(contractAddress, allowFallback, maxPayloadSize, maxResultSize));
        _services.AddOrReplaceSingleton<IFlashCallExecutor, DeployedFlashCallExecutor>();
        return this;
    }

    private void RunConfigureActions(IServiceProvider provider)
    {
        foreach(var (serviceType, _, action) in _configureActions)
        {
            action(provider.GetRequiredService(serviceType));
        }
    }

    private void AssertReadClientConfiguration()
    {
        if(_transportRegistration is null)
        {
            throw new InvalidOperationException($"No RPCTransport configured. Call the {nameof(WithRPCTransport)} method prior to {nameof(BuildReadClient)}.");
        }

        _services.AddSingleton(_transportRegistration);

        _services.AddSingleton<IRpcClient, RpcClient>();
        _services.AddSingleton<IEthRpcModule, EthRpcModule>();
        _services.AddSingleton<ITraceRpcModule, TraceRpcModule>();
        _services.AddSingleton<IEtherModule, EtherModule>();
        _services.AddSingleton<IEtherTxModule, EtherModule>();
        _services.AddSingleton<ITraceModule, TraceModule>();
        _services.AddSingleton<IBlocksModule, BlocksModule>();

        _services.AddSingleton<ContractFactory>();

        if(!_services.Any(x => x.ServiceType == typeof(ISubscriptionsManager)))
        {
            _services.AddSingleton<ISubscriptionsManager, SubscriptionsManager>();
        }
        if(!_services.Any(x => x.ServiceType == typeof(IQueryExecutor)))
        {
            _services.AddSingleton<IQueryExecutor, FlashCallQueryExecutor>();
        }
        if(!_services.Any(x => x.ServiceType == typeof(IFlashCallExecutor)))
        {
            _services.AddSingleton<IFlashCallExecutor, ConstructorFlashCallExecutor>();
        }

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

    /// <summary>
    /// Builds a client supporting read-only operations.
    /// </summary>
    /// <returns></returns>
    public IEtherClient BuildReadClient()
    {
        AssertReadClientConfiguration();

        _services.AddSingleton<IEtherClient>(provider => new EtherClient(provider, false));

        var provider = _services.BuildServiceProvider();

        _contractConfigurationAction?.Invoke(provider.GetRequiredService<ContractFactory>());
        RunConfigureActions(provider);

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
    }

    /// <summary>
    /// Creates a client supporting read and write operations.
    /// </summary>
    /// <returns></returns>
    public IEtherTxClient BuildTxClient()
    {
        AssertTxClientConfiguration();

        _services.AddSingleton<IEtherClient>(provider => provider.GetRequiredService<IEtherTxClient>());
        _services.AddSingleton<IEtherTxClient>(provider => new EtherClient(provider, true));

        var provider = _services.BuildServiceProvider();

        _contractConfigurationAction?.Invoke(provider.GetRequiredService<ContractFactory>());
        RunConfigureActions(provider);

        return provider.GetRequiredService<IEtherTxClient>();
    }
}
