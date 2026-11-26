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
    private readonly IEtherClient _baseClient;
    private readonly IEtherSigner _signer;

    private ulong? _initialNonce;
    private TimeSpan _confirmationTimeout = TimeSpan.FromSeconds(30);
    private ITxPublisher? _txPublisher;
    private Func<IServiceProvider, ulong, INonceAllocator>? _nonceAllocatorFactory;

    internal EtherTxClientBuilder(IEtherClient baseClient, IEtherSigner signer)
    {
        ArgumentNullException.ThrowIfNull(baseClient);
        ArgumentNullException.ThrowIfNull(signer);

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

        _nonceAllocatorFactory = (provider, initialNonce) => new LocalNonceAllocator(
            provider.GetRequiredService<INonceWatcher>(),
            _signer.Address,
            initialNonce,
            new LocalNonceAllocator.Options { SyncInterval = syncInterval }
        );
        return this;
    }

    /// <summary>
    /// Configures <see cref="RefreshOnAllocateNonceAllocator"/>.
    /// </summary>
    public EtherTxClientBuilder WithRefreshOnAllocateNonceAllocator()
    {
        _nonceAllocatorFactory = (provider, initialNonce) => new RefreshOnAllocateNonceAllocator(
            provider.GetRequiredService<IEthRpcModule>(),
            _signer.Address,
            initialNonce
        );
        return this;
    }

    /// <summary>
    /// Configures a custom <see cref="INonceAllocator"/> instance.
    /// </summary>
    public EtherTxClientBuilder WithNonceAllocator(INonceAllocator nonceAllocator)
    {
        ArgumentNullException.ThrowIfNull(nonceAllocator);

        _nonceAllocatorFactory = (_, _) => nonceAllocator;
        return this;
    }

    /// <summary>
    /// Configures a custom <see cref="INonceAllocator"/> factory receiving the resolved initial nonce.
    /// </summary>
    public EtherTxClientBuilder WithNonceAllocator(Func<ulong, INonceAllocator> nonceAllocatorFactory)
    {
        ArgumentNullException.ThrowIfNull(nonceAllocatorFactory);

        _nonceAllocatorFactory = (_, initialNonce) => nonceAllocatorFactory(initialNonce);
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
    /// Overrides the <see cref="ITxPublisher"/> used for transactions sent by this signer.
    /// </summary>
    public EtherTxClientBuilder WithTxPublisher(ITxPublisher txPublisher)
    {
        ArgumentNullException.ThrowIfNull(txPublisher);

        _txPublisher = txPublisher;
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

        var nonceAllocator = _nonceAllocatorFactory is not null
            ? _nonceAllocatorFactory(provider, initialAllocatorNonce)
            : new LocalNonceAllocator(
                provider.GetRequiredService<INonceWatcher>(),
                _signer.Address,
                initialAllocatorNonce
            );

        var txScheduler = new BlockingSequentialTxScheduler(
            provider,
            ethRpcModule,
            _signer,
            _txPublisher ?? provider.GetRequiredService<ITxPublisher>(),
            provider.GetRequiredService<INonceWatcher>(),
            provider.GetRequiredService<ITxConfirmationWatcher>(),
            nonceAllocator,
            _baseClient.ChainId,
            confirmedNonce,
            _confirmationTimeout
        );

        return new EtherTxClient(_baseClient, _signer, txScheduler);
    }
}
