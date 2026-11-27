using EtherSharp.Client.Modules.Blocks;
using EtherSharp.Client.Modules.Debug;
using EtherSharp.Client.Modules.Ether;
using EtherSharp.Client.Modules.Mempool;
using EtherSharp.Client.Modules.Trace;
using EtherSharp.Client.Services;
using EtherSharp.Client.Services.ContractFactory;
using EtherSharp.Client.Services.FlashCall;
using EtherSharp.Client.Services.GasFeeProvider;
using EtherSharp.Client.Services.QueryExecutor;
using EtherSharp.Client.Services.ResiliencyLayer;
using EtherSharp.Client.Services.Subscriptions;
using EtherSharp.Common;
using EtherSharp.Common.Extensions;
using EtherSharp.RPC;
using EtherSharp.RPC.Modules.Debug;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.RPC.Modules.Trace;
using EtherSharp.RPC.Transport;
using EtherSharp.Tx.EIP1559;
using EtherSharp.Tx.Legacy;
using EtherSharp.Tx.Watcher;
using EtherSharp.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;
using System.Net.WebSockets;
using System.Text.Json;

namespace EtherSharp.Client;

/// <summary>
/// Builder for creating an EtherClient.
/// </summary>
public sealed class EtherClientBuilder : IInternalEtherClientBuilder
{
    private readonly IServiceCollection _services = new ServiceCollection();
    IServiceCollection IInternalEtherClientBuilder.Services => _services;
    private (ulong ChainId, ChainIdMode Mode)? _configuredChainId;
    private ulong? _ethCallGasLimit;
    private ulong? _flashCallGasLimit;

    private Func<IServiceProvider, IRPCTransport>? _transportRegistration;

    private EtherClientBuilder() { }

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
    /// <param name="loggerFactory"></param>
    /// <param name="configureWebSocketOptions">Optional callback invoked before every connection attempt, including reconnects. Compression is disabled unless configured through the socket options.</param>
    /// <returns></returns>
    public static EtherClientBuilder CreateForWebsocket(
        string websocketUrl, TimeSpan? requestTimeout = null, ILoggerFactory? loggerFactory = null,
        Action<ClientWebSocketOptions>? configureWebSocketOptions = null
    ) => CreateForWebsocket(
        new Uri(websocketUrl, UriKind.Absolute),
        requestTimeout,
        loggerFactory,
        configureWebSocketOptions
    );

    /// <summary>
    /// Creates an EtherClientBuilder preconfigured for a websocket RPC.
    /// </summary>
    /// <param name="websocketUri"></param>
    /// <param name="requestTimeout"></param>
    /// <param name="loggerFactory"></param>
    /// <param name="configureWebSocketOptions">Optional callback invoked before every connection attempt, including reconnects. Compression is disabled unless configured through the socket options.</param>
    /// <returns></returns>
    public static EtherClientBuilder CreateForWebsocket(
        Uri websocketUri, TimeSpan? requestTimeout = null, ILoggerFactory? loggerFactory = null,
        Action<ClientWebSocketOptions>? configureWebSocketOptions = null
    )
    {
        requestTimeout ??= TimeSpan.FromSeconds(30);

        var builder = new EtherClientBuilder
        {
            _transportRegistration = provider =>
                new WssJsonRpcTransport(websocketUri, requestTimeout.Value, provider, configureWebSocketOptions: configureWebSocketOptions)
        };

        return ConfigureDefaults(builder, loggerFactory);
    }

    /// <summary>
    /// Creates an EtherClientBuilder preconfigured for a HTTP RPC.
    /// </summary>
    /// <param name="rpcUrl"></param>
    /// <param name="loggerFactory"></param>
    /// <param name="httpMessageHandler">Optional caller-owned handler. When omitted, a handler with gzip, deflate, and Brotli response decompression enabled is created.</param>
    /// <returns></returns>
    public static EtherClientBuilder CreateForHttpRpc(
        string rpcUrl, ILoggerFactory? loggerFactory = null, HttpMessageHandler? httpMessageHandler = null
    ) => CreateForHttpRpc(
        new Uri(rpcUrl, UriKind.Absolute),
        loggerFactory,
        httpMessageHandler
    );

    /// <summary>
    /// Creates an EtherClientBuilder preconfigured for a HTTP RPC.
    /// </summary>
    /// <param name="rpcUri"></param>
    /// <param name="loggerFactory"></param>
    /// <param name="httpMessageHandler">Optional caller-owned handler. When omitted, a handler with gzip, deflate, and Brotli response decompression enabled is created.</param>
    /// <returns></returns>
    public static EtherClientBuilder CreateForHttpRpc(
        Uri rpcUri, ILoggerFactory? loggerFactory = null, HttpMessageHandler? httpMessageHandler = null
    )
    {
        var builder = new EtherClientBuilder()
        {
            _transportRegistration = provider =>
                new HttpJsonRpcTransport(rpcUri, provider, httpMessageHandler: httpMessageHandler)
        };

        return ConfigureDefaults(builder, loggerFactory);
    }

    private static EtherClientBuilder ConfigureDefaults(EtherClientBuilder builder, ILoggerFactory? loggerFactory)
    {
        if(loggerFactory is not null)
        {
            builder.WithLoggerFactory(loggerFactory);
        }

        return builder;
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
    public EtherClientBuilder AddRPCMiddleware<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TRpcMiddleware>()
        where TRpcMiddleware : class, IRpcMiddleware
    {
        _services.AddSingleton<IRpcMiddleware, TRpcMiddleware>();
        return this;
    }

    /// <summary>
    /// Registers the RPC middleware.
    /// </summary>
    /// <typeparam name="TRpcMiddleware"></typeparam>
    /// <param name="middlewareFactory"></param>
    /// <returns></returns>
    public EtherClientBuilder AddRPCMiddleware<TRpcMiddleware>(Func<IServiceProvider, TRpcMiddleware> middlewareFactory)
        where TRpcMiddleware : class, IRpcMiddleware
    {
        _services.AddSingleton<IRpcMiddleware>(middlewareFactory);
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
    /// Configures the JSON serializer options used by this client instance.
    /// </summary>
    /// <remarks>
    /// Use <see cref="ParsingUtils.CreateDefaultEvmSerializerOptions"/> as a baseline when only minor customization is required.
    /// </remarks>
    /// <param name="jsonSerializerOptions"></param>
    /// <returns></returns>
    public EtherClientBuilder WithJsonSerializerOptions(JsonSerializerOptions jsonSerializerOptions)
    {
        ArgumentNullException.ThrowIfNull(jsonSerializerOptions);
        _services.AddOrReplaceSingleton(jsonSerializerOptions);
        return this;
    }

    /// <summary>
    /// Configures retry and backoff behavior for transient RPC failures in background watchers.
    /// </summary>
    public EtherClientBuilder WithRpcRetryOptions(Action<RpcRetryOptions> configureAction)
    {
        ArgumentNullException.ThrowIfNull(configureAction);

        var options = new RpcRetryOptions();
        configureAction(options);
        options.Validate(nameof(configureAction));

        _services.AddOrReplaceSingleton(options);
        return this;
    }

    /// <summary>
    /// Configures fee and gas limit estimation of the built-in gas fee providers.
    /// </summary>
    /// <param name="configureAction">Callback to configure the options, starting from the defaults.</param>
    /// <returns>This builder.</returns>
    public EtherClientBuilder WithGasFeeOptions(Action<GasFeeOptions> configureAction)
    {
        ArgumentNullException.ThrowIfNull(configureAction);

        var options = new GasFeeOptions();
        configureAction(options);
        options.Validate(nameof(configureAction));

        _services.AddOrReplaceSingleton(options);
        return this;
    }

    /// <summary>
    /// Registers a ResiliencyLayer on this EtherClient.
    /// </summary>
    /// <param name="resiliencyLayer">The caller-owned instance</param>
    /// <returns>This builder.</returns>
    public EtherClientBuilder WithResiliencyLayer(IResiliencyLayer resiliencyLayer)
    {
        ArgumentNullException.ThrowIfNull(resiliencyLayer);
        _services.AddOrReplaceSingleton(resiliencyLayer);
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
    /// Configures a known chain ID for client initialization.
    /// </summary>
    /// <param name="chainId">Target chain ID.</param>
    /// <param name="mode">How the configured chain ID is handled during initialization.</param>
    /// <returns></returns>
    public EtherClientBuilder WithChainId(ulong chainId, ChainIdMode mode)
    {
        if(!Enum.IsDefined(mode))
        {
            throw new ArgumentOutOfRangeException(nameof(mode), mode, "Unsupported chain ID mode.");
        }

        _configuredChainId = (chainId, mode);
        return this;
    }

    /// <summary>
    /// Configures the initial default gas limit applied to <c>eth_call</c> execution.
    /// </summary>
    /// <param name="gasLimit">Optional fallback gas limit applied to <c>eth_call</c> requests when no explicit gas is provided.</param>
    /// <returns></returns>
    public EtherClientBuilder WithEthCallGasLimit(ulong? gasLimit = null)
    {
        CallGasLimitSettings.Validate(gasLimit, _flashCallGasLimit);
        _ethCallGasLimit = gasLimit;
        return this;
    }

    /// <summary>
    /// Configures the initial default gas limit forwarded by the flash caller to temporary contracts.
    /// </summary>
    /// <param name="gasLimit">Optional fallback when no per-call limit is provided.</param>
    /// <remarks>For state-override flash calls, use <see cref="WithEthCallGasLimit"/> instead.</remarks>
    /// <returns></returns>
    public EtherClientBuilder WithFlashCallGasLimit(ulong? gasLimit = null)
    {
        CallGasLimitSettings.Validate(_ethCallGasLimit, gasLimit);
        _flashCallGasLimit = gasLimit;
        return this;
    }

    /// <summary>
    /// Configures flash calls and nonce polling to use <c>eth_call</c> state overrides.
    /// </summary>
    /// <remarks>Requires an RPC endpoint that supports state overrides.</remarks>
    /// <returns></returns>
    public EtherClientBuilder WithStateOverrides()
    {
        _services.RemoveAll<FlashCallExecutor>();
        _services.AddSingleton<FlashCallExecutor, StateOverrideFlashCallExecutor>();
        _services.AddOrReplaceSingleton<NonceSource, StateOverrideNonceSource>();
        return this;
    }

    private void AssertConfiguration()
    {
        if(_transportRegistration is null)
        {
            throw new InvalidOperationException($"No RPCTransport configured. Call the {nameof(WithRPCTransport)} method prior to {nameof(Build)}.");
        }

        _services.AddSingleton(_transportRegistration);
        _services.AddOrReplaceSingleton(new CallGasLimitSettings(_ethCallGasLimit, _flashCallGasLimit));

        _services.AddSingleton<RpcClient>();
        _services.AddSingleton<IEthRpcModule, EthRpcModule>();
        _services.AddSingleton<ITraceRpcModule, TraceRpcModule>();
        _services.AddSingleton<IDebugRpcModule, DebugRpcModule>();
        _services.TryAddSingleton<RpcRetryOptions>();
        _services.TryAddSingleton<GasFeeOptions>();
        _services.TryAddSingleton<NonceSource, ConstructorCallNonceSource>();
        _services.TryAddSingleton<INonceWatcher, PollingNonceWatcher>();
        _services.TryAddSingleton<ITxConfirmationWatcher, PollingTxConfirmationWatcher>();
        _services.TryAddSingleton<IGasFeeProvider<EIP1559TxParams, EIP1559GasParams>, EIP1559GasFeeProvider>();
        _services.TryAddSingleton<IGasFeeProvider<LegacyTxParams, LegacyGasParams>, LegacyGasFeeProvider>();

        _services.AddSingleton<IEtherModule, EtherModule>();
        _services.AddSingleton<ITraceModule, TraceModule>();
        _services.AddSingleton<IBlocksModule, BlocksModule>();
        _services.AddSingleton<IMempoolModule, MempoolModule>();
        _services.AddSingleton<IDebugModule, DebugModule>();

        _services.AddSingleton<ContractFactory>();
        _services.TryAddSingleton<ISubscriptionsManager, SubscriptionsManager>();
        _services.AddSingleton<QueryExecutor>();

        _services.TryAddSingleton<FlashCallExecutor, DeployedFlashCallExecutor>();

        if(!_services.Any(x => x.ServiceType == typeof(JsonSerializerOptions)))
        {
            _services.AddSingleton(ParsingUtils.CreateDefaultEvmSerializerOptions());
        }

        _services.AddSingleton(provider => new EtherSharpJsonSerializerContext(
            new JsonSerializerOptions(provider.GetRequiredService<JsonSerializerOptions>())
        ));

        foreach(var service in _services.ToArray())
        {
            if(service.ServiceType == typeof(IResiliencyLayer) && service.ImplementationInstance is not null)
            {
                continue;
            }

            var serviceType = service.ImplementationType
                ?? service.ImplementationInstance?.GetType()
                ?? service.ServiceType;

            if(!typeof(IInitializableService).IsAssignableFrom(serviceType))
            {
                continue;
            }

            _services.AddSingleton(typeof(IInitializableService), provider => provider.GetRequiredService(service.ServiceType));
        }
    }

    /// <summary>
    /// Builds an <see cref="IEtherClient"/>.
    /// </summary>
    /// <returns></returns>
    public IEtherClient Build()
    {
        AssertConfiguration();

        var configuredChainId = _configuredChainId;
        _services.AddSingleton<IEtherClient>(provider => new EtherClient(provider, configuredChainId));

        var provider = _services.BuildServiceProvider();

        _ = provider.GetRequiredService<ITxConfirmationWatcher>();

        return provider.GetRequiredService<IEtherClient>();
    }
}
