using EtherSharp.Client.Services.GasFeeProvider;
using EtherSharp.Client.Services.ResiliencyLayer;
using EtherSharp.Client.Services.RPC;
using EtherSharp.Client.Services.TxConfirmer;
using EtherSharp.Client.Services.TxPublisher;
using EtherSharp.Client.Services.TxTypeHandler;
using EtherSharp.Common.Exceptions;
using EtherSharp.Crypto;
using EtherSharp.Tx;
using EtherSharp.Tx.PendingHandler;
using EtherSharp.Tx.Types;
using EtherSharp.Types;
using EtherSharp.Wallet;
using Microsoft.Extensions.DependencyInjection;

namespace EtherSharp.Client.Services.TxScheduler;

public class BlockingSequentialResumableTxScheduler : ITxScheduler, IInitializableService
{
    private readonly IServiceProvider _provider;
    private readonly IRpcClient _rpcClient;
    private readonly IEtherSigner _signer;
    private readonly ITxPublisher _txPublisher;
    private readonly ITxConfirmer _txConfirmer;

    private readonly IResiliencyLayer? _resiliencyLayer;

    private readonly TimeSpan _txTimeout = TimeSpan.FromSeconds(30);

    private readonly Lock _pendingTxHandlersLock = new Lock();
    private readonly Dictionary<uint, IInternalPendingTxHandler> _pendingTxHandlers = [];

    private ulong _chainId;

    private SemaphoreSlim _pendingNoncesSemaphore;

    /// <summary>
    /// Highest nonce that is not confirmed on-chain yet.
    /// </summary>
    private uint _activeNonce;
    /// <summary>
    /// Highest nonce that is still unallocated.
    /// </summary>
    private uint _peakNonce;

    public BlockingSequentialResumableTxScheduler(IServiceProvider provider, IRpcClient rpcClient, IEtherSigner signer,
        ITxPublisher txPublisher, ITxConfirmer txConfirmer)
    {
        _provider = provider;
        _rpcClient = rpcClient;
        _signer = signer;
        _txPublisher = txPublisher;
        _txConfirmer = txConfirmer;

        _resiliencyLayer = _provider.GetService<IResiliencyLayer>();
    }

    ValueTask<IPendingTxHandler<TTxParams, TTxGasParams>> ITxScheduler.AttachPendingTxAsync<TTransaction, TTxParams, TTxGasParams>(uint nonce) => throw new NotImplementedException();

    async ValueTask IInitializableService.InitializeAsync(ulong chainId, CancellationToken cancellationToken)
    {
        _chainId = chainId;

        _activeNonce = await _rpcClient.EthGetTransactionCount(
            _signer.Address.String, TargetBlockNumber.Latest, cancellationToken
        );

        if(_resiliencyLayer is null)
        {
            _peakNonce = _activeNonce;
            _pendingNoncesSemaphore = new SemaphoreSlim(0);
        }
        else
        {
            _peakNonce = Math.Max(_activeNonce, await _resiliencyLayer.GetLastSubmittedNonceAsync(cancellationToken) + 1);
            _pendingNoncesSemaphore = new SemaphoreSlim((int) (_peakNonce - _activeNonce));

            for(uint nonce = _activeNonce; nonce < _peakNonce; nonce++)
            {
                var txSubmissions = await _resiliencyLayer.FetchTxSubmissionsAsync(nonce);
                _pendingTxHandlers.Add(nonce, new PendingTxHandler(nonce, txSubmissions));
            }
        }

        _ = Task.Run(NonceProcessor, default);
    }

    private async Task NonceProcessor()
    {
        while(true)
        {
            //Wait for _peakNonce + 1 > _activeNonce
            await _pendingNoncesSemaphore.WaitAsync();

            IInternalPendingTxHandler? txHandler;
            lock(_pendingTxHandlersLock)
            {
                _pendingTxHandlers.Remove(_activeNonce, out txHandler);
            }

            if(txHandler is null)
            {
                throw new ImpossibleException();
            }

            var returnCts = await txHandler.WaitForConfirmCallAsync();

            var submissionResult = await txHandler.PublishFunc.Invoke();

            //var confirmCall = txHandler.WaitForConfirmCallAsync();
            //var scanCall = StartScanningForTx();

            //var completedTask = Task.WhenAny(confirmCall, scanCall);

            //Start Scan Task



            //Wait for confirm call 

            //Confirm Call Received -> Wait for Scan Task Completed -> Send result to confirm call -> Done
            //Scan Task Completed -> Done



            _activeNonce++;
        }
    }

    ValueTask<IPendingTxHandler<TTxParams, TTxGasParams>> ITxScheduler.PrepareTxAsync<TTransaction, TTxParams, TTxGasParams>(
        ITxInput call, TTxParams? txParams, TTxGasParams? txGasParams
    )
        where TTxParams : class
        where TTxGasParams : class
    {
        uint nextNonce = Interlocked.Increment(ref _peakNonce) - 1;
        var handler = new PendingTxHandler<TTxParams, TTxGasParams>(nextNonce, []);

        lock(_pendingTxHandlersLock)
        {
            _pendingTxHandlers.Add(nextNonce, handler);
        }

        _pendingNoncesSemaphore.Release();
        return handler;
    }

    private async Task<TxSubmissionResult> PublishTxAsync<TTransaction, TTxParams, TTxGasParams>(
        IInternalPendingTxHandler txHandler, ITxInput txInput, TTxParams txParams, TTxGasParams txGasParams
    )
        where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams<TTxGasParams>
    {
        try
        {
            var handler = _provider.GetService<ITxTypeHandler<TTransaction, TTxParams, TTxGasParams>>()
                ?? throw new InvalidOperationException(
                    $"No ITxTypeHandler found that supports {typeof(TTxParams).FullName};{typeof(TTxGasParams).FullName} is not registered");
            var gasFeeProvider = _provider.GetService<IGasFeeProvider<TTxParams, TTxGasParams>>()
                ?? throw new InvalidOperationException(
                    $"No IGasFeeProvider found that supports {typeof(TTxParams).FullName};{typeof(TTxGasParams).FullName} is not registered");

            txParams ??= TTxParams.Default;
            txGasParams ??= await gasFeeProvider.EstimateGasParamsAsync(txInput.To, txInput.Value, txInput.Data, txParams, default);

            string txBytes = handler.EncodeTxToBytes(txInput, txParams, txGasParams, txHandler.Nonce, out string txHash);

            var submission = new TxSubmission(txHash, txInput.To, txInput.Value, txInput.Data, txParams.Encode(), txGasParams.Encode());
            txHandler.AddTxSubmission(submission);

            if(_resiliencyLayer is not null)
            {
                await _resiliencyLayer.StoreTxSubmissionAsync(txHandler.Nonce, submission);
            }

            return await _txPublisher.PublishTxAsync(txBytes);
        }
        catch(Exception ex)
        {
            return new TxSubmissionResult.UnhandledException(ex);
        }
    }
}
