using EtherSharp.Client.Services.ResiliencyLayer;
using EtherSharp.Client.Services.TxPublisher;
using EtherSharp.Client.Services.TxScheduler;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.RPC.Transport;
using EtherSharp.Tx.Nonce;
using EtherSharp.Tx.Watcher;
using EtherSharp.Types;
using EtherSharp.Wallet;
using Microsoft.Extensions.DependencyInjection;

namespace EtherSharp.Client;

/// <summary>
/// Builder for attaching an <see cref="IEtherSigner"/> to an initialized <see cref="IEtherClient"/>.
/// </summary>
public sealed class EtherTxClientBuilder
{
    private readonly EtherClient _ownerClient;
    private readonly IEtherClient _baseClient;
    private readonly IEtherSigner _signer;

    private ulong? _initialNonce;
    private TimeSpan _confirmationTimeout = TimeSpan.FromSeconds(30);
    private Func<IServiceProvider, CancellationToken, ValueTask<ITxPublisher>>? _txPublisherFactory;
    private Func<IServiceProvider, ulong, CancellationToken, ValueTask<INonceAllocator>>? _nonceAllocatorFactory;

    internal EtherTxClientBuilder(EtherClient baseClient, IEtherSigner signer)
    {
        ArgumentNullException.ThrowIfNull(baseClient);
        ArgumentNullException.ThrowIfNull(signer);

        _ownerClient = baseClient;
        _baseClient = baseClient;
        _signer = signer;
    }

    /// <summary>
    /// Seeds the initial on-chain nonce to avoid an <c>eth_getTransactionCount</c> call during <see cref="BuildAsync"/>.
    /// </summary>
    public EtherTxClientBuilder WithInitialNonce(ulong initialNonce)
    {
        _initialNonce = initialNonce;
        return this;
    }

    /// <summary>
    /// Configures the default <see cref="LocalNonceAllocator"/>.
    /// </summary>
    public EtherTxClientBuilder WithLocalNonceAllocator(TimeSpan syncInterval = default)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(syncInterval, TimeSpan.Zero, nameof(syncInterval));
        return WithNonceAllocator((provider, initialNonce) => new LocalNonceAllocator(
            provider.GetRequiredService<INonceWatcher>(),
            _signer.Address,
            initialNonce,
            new LocalNonceAllocator.Options { SyncInterval = syncInterval }
        ));
    }

    /// <summary>
    /// Configures <see cref="RefreshOnAllocateNonceAllocator"/>.
    /// </summary>
    public EtherTxClientBuilder WithRefreshOnAllocateNonceAllocator()
        => WithNonceAllocator((provider, initialNonce) => new RefreshOnAllocateNonceAllocator(
            provider.GetRequiredService<IEthRpcModule>(),
            _signer.Address,
            initialNonce
        ));

    /// <summary>
    /// Configures a custom <see cref="INonceAllocator"/> factory receiving the resolved initial nonce.
    /// </summary>
    public EtherTxClientBuilder WithNonceAllocator(Func<IServiceProvider, ulong, INonceAllocator> nonceAllocatorFactory)
    {
        ArgumentNullException.ThrowIfNull(nonceAllocatorFactory);
        _nonceAllocatorFactory = (provider, initialNonce, _) => ValueTask.FromResult(nonceAllocatorFactory(provider, initialNonce));
        return this;
    }

    /// <summary>
    /// Configures a custom asynchronous <see cref="INonceAllocator"/> factory receiving the resolved initial nonce.
    /// </summary>
    public EtherTxClientBuilder WithNonceAllocator(
        Func<IServiceProvider, ulong, CancellationToken, ValueTask<INonceAllocator>> nonceAllocatorFactory
    )
    {
        ArgumentNullException.ThrowIfNull(nonceAllocatorFactory);
        _nonceAllocatorFactory = nonceAllocatorFactory;
        return this;
    }

    /// <summary>
    /// Configures the confirmation timeout before invoking the error callback with a timeout.
    /// </summary>
    public EtherTxClientBuilder WithConfirmationTimeout(TimeSpan confirmationTimeout)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(confirmationTimeout, TimeSpan.Zero, nameof(confirmationTimeout));
        _confirmationTimeout = confirmationTimeout;
        return this;
    }

    /// <summary>
    /// Configures the <see cref="ITxPublisher"/> used for transactions sent by this signer.
    /// </summary>
    public EtherTxClientBuilder WithTxPublisher(Func<IServiceProvider, ITxPublisher> txPublisherFactory)
    {
        ArgumentNullException.ThrowIfNull(txPublisherFactory);
        _txPublisherFactory = (provider, _) => ValueTask.FromResult(txPublisherFactory(provider));
        return this;
    }

    /// <summary>
    /// Configures the <see cref="ITxPublisher"/> used for transactions sent by this signer via an asynchronous factory.
    /// </summary>
    public EtherTxClientBuilder WithTxPublisher(Func<IServiceProvider, CancellationToken, ValueTask<ITxPublisher>> txPublisherFactory)
    {
        ArgumentNullException.ThrowIfNull(txPublisherFactory);
        _txPublisherFactory = txPublisherFactory;
        return this;
    }

    /// <summary>
    /// Builds and initializes the <see cref="IEtherTxClient"/> bound to the configured signer.
    /// </summary>
    public async Task<IEtherTxClient> BuildAsync(
        RpcRequestOptions requestOptions = default,
        CancellationToken cancellationToken = default
    )
    {
        ObjectDisposedException.ThrowIf(_ownerClient.IsDisposed, _ownerClient);

        if(!_baseClient.IsInitialized)
        {
            throw new InvalidOperationException("Client not initialized");
        }

        var provider = _baseClient.AsInternal().Provider;
        var ethRpcModule = provider.GetRequiredService<IEthRpcModule>();
        var resiliencyLayer = provider.GetService<IResiliencyLayer>();

        ulong confirmedNonce = _initialNonce ?? await ethRpcModule.GetTransactionCountAsync(
            _signer.Address, TargetHeight.Latest, requestOptions, cancellationToken
        );

        ulong initialAllocatorNonce = confirmedNonce;
        if(resiliencyLayer is not null)
        {
            ulong? dbNonce = await resiliencyLayer.GetLastSubmittedNonceAsync(
                new TxSenderKey(_baseClient.ChainId, _signer.Address), cancellationToken
            );
            if(dbNonce is { } lastSubmitted)
            {
                initialAllocatorNonce = Math.Max(lastSubmitted + 1, confirmedNonce);
            }
        }

        INonceAllocator? nonceAllocator = null;
        ITxPublisher? txPublisher = null;
        BlockingSequentialTxScheduler txScheduler;

        try
        {
            nonceAllocator = _nonceAllocatorFactory is not null
                ? await _nonceAllocatorFactory(provider, initialAllocatorNonce, cancellationToken)
                : new LocalNonceAllocator(
                    provider.GetRequiredService<INonceWatcher>(),
                    _signer.Address,
                    initialAllocatorNonce
                );

            txPublisher = _txPublisherFactory is not null
                ? await _txPublisherFactory(provider, cancellationToken)
                : new BasicTxPublisher(ethRpcModule);

            txScheduler = new BlockingSequentialTxScheduler(
                provider,
                ethRpcModule,
                _signer,
                txPublisher,
                provider.GetRequiredService<INonceWatcher>(),
                provider.GetRequiredService<ITxConfirmationWatcher>(),
                nonceAllocator,
                _baseClient.ChainId,
                confirmedNonce,
                _confirmationTimeout
            );
        }
        catch
        {
            nonceAllocator?.Dispose();
            if(txPublisher is not null)
            {
                await txPublisher.DisposeAsync();
            }

            throw;
        }

        var txClient = new EtherTxClient(_ownerClient, _signer, txScheduler);

        try
        {
            _ownerClient.RegisterTxClient(txClient);
        }
        catch
        {
            await txClient.DisposeAsync();
            throw;
        }

        return txClient;
    }
}
