using EtherSharp.Client.Services.RPC;
using EtherSharp.Client.Services.TxConfirmer;
using EtherSharp.Client.Services.TxPublisher;
using EtherSharp.Client.Services.TxTypeHandler;
using EtherSharp.Common.Exceptions;
using EtherSharp.Tx;
using EtherSharp.Tx.Types;
using EtherSharp.Types;
using EtherSharp.Wallet;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Channels;

using QueueEntry = (
    EtherSharp.Client.Services.TxTypeHandler.ITxTypeHandler TxTypeHandler,
    EtherSharp.Tx.ITxInput TxInput,
    EtherSharp.Tx.Types.ITxParams TxParams,
    EtherSharp.Tx.Types.ITxGasParams? TxGasParams,
    System.Func<System.Threading.Tasks.ValueTask<EtherSharp.Tx.TxTimeoutAction>> OnTxTimeout,
    System.Threading.Tasks.TaskCompletionSource<EtherSharp.Types.TransactionReceipt> CompletionSource
);

namespace EtherSharp.Client.Services.TxScheduler;
public class BlockingSequentialTxScheduler : ITxScheduler, IInitializableService, IDisposable
{
    private readonly Channel<QueueEntry> _queue;

    private readonly IServiceProvider _provider;
    private readonly IRpcClient _rpcClient;
    private readonly IEtherSigner _signer;
    private readonly ITxPublisher _txPublisher;
    private readonly ITxConfirmer _txConfirmer;

    private readonly TimeSpan _txTimeout = TimeSpan.FromSeconds(30);

    private ulong _chainId;
    private uint _nonceCounter;

    public BlockingSequentialTxScheduler(IServiceProvider provider, IRpcClient rpcClient, IEtherSigner signer,
        ITxPublisher txPublisher, ITxConfirmer txConfirmer)
    {
        _queue = Channel.CreateUnbounded<QueueEntry>(new UnboundedChannelOptions()
        {
            SingleReader = true,
            SingleWriter = false,
            AllowSynchronousContinuations = false
        });

        _provider = provider;
        _rpcClient = rpcClient;
        _signer = signer;
        _txPublisher = txPublisher;
        _txConfirmer = txConfirmer;
    }

    public async ValueTask InitializeAsync(ulong chainId, CancellationToken cancellationToken)
    {
        _chainId = chainId;
        _nonceCounter = await _rpcClient.EthGetTransactionCount(
            _signer.Address.String, TargetBlockNumber.Latest, cancellationToken) - 1;

        _ = Task.Run(BackgroundTxProcessor, CancellationToken.None);
    }

    //Only generic for type safety
    public Task<TransactionReceipt> PublishTxAsync<TTxTypeHandler, TTransaction, TTxParams, TTxGasParams>(
        ITxInput txInput, ITxParams txParams, ITxGasParams? txGasParams,
        Func<ValueTask<TxTimeoutAction>> onTxTimeout
    )
        where TTxTypeHandler : class, ITxTypeHandler<TTransaction, TTxParams, TTxGasParams>
        where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
        where TTxParams : ITxParams
        where TTxGasParams : ITxGasParams
    {
        var tcs = new TaskCompletionSource<TransactionReceipt>();
        var handler = _provider.GetService<TTxTypeHandler>()
            ?? throw new InvalidOperationException($"TxTypeHandler {typeof(TTxTypeHandler).FullName} is not registered");

        return !_queue.Writer.TryWrite((handler, txInput, txParams, txGasParams, onTxTimeout, tcs))
            ? throw new NotImplementedException()
            : tcs.Task;
    }

    private async Task BackgroundTxProcessor()
    {
        while(await _queue.Reader.WaitToReadAsync())
        {
            _queue.Reader.TryRead(out var entry);

            try
            {
                await ProcessTxAsync(entry);
            }
            catch(Exception ex)
            {
                entry.CompletionSource.SetException(ex);
            }
        }
    }

    private async Task ProcessTxAsync(QueueEntry entry)
    {
        var (handler, txInput, txParams, txGasParams, _, _) = entry;
        uint nonce = Interlocked.Increment(ref _nonceCounter);

        //ToDo: Consider avoiding this allocation
        byte[] inputData = new byte[txInput.DataLength];
        txInput.WriteDataTo(inputData);
        
        while(true)
        {
            txGasParams ??= await handler.CalculateGasParamsAsync(txInput, txParams, inputData);
            string txBytes = handler.EncodeTxToBytes(txInput, txParams, txGasParams, inputData, nonce);
            var submissionResult = await _txPublisher.PublishTxAsync(txBytes);

            switch(submissionResult)
            {
                case TxSubmissionResult.Success successResult:
                    await EnsureTransactionGetsConfirmedAsync(successResult.TxHash, entry);
                    return;
                case TxSubmissionResult.NonceTooLow nonceTooLowResult:
                    if(nonceTooLowResult.TxNonce > nonceTooLowResult.NextNonce)
                    {
                        throw new NotSupportedException("Resubmitting of transactions is not supported for this TxScheduler");
                    }

                    _nonceCounter = nonceTooLowResult.NextNonce;
                    nonce = nonceTooLowResult.NextNonce;
                    break;
                case TxSubmissionResult.Failure failureResult:
                    throw new TxPublishException(failureResult.Message);
                default:
                    throw new NotSupportedException($"TxSubmissionResult of type {submissionResult.GetType()} is not supported for this TxScheduler");
            }
        }
    }

    private async Task EnsureTransactionGetsConfirmedAsync(string txHash, QueueEntry entry)
    {
        var (_, _, _, _, onTxTimeout, tcs) = entry;

        var txResult = await _txConfirmer.WaitForTxConfirmationAsync(txHash, _txTimeout);

        while(true)
        {
            if(txResult is TxConfirmationResult.Confirmed confirmedResult)
            {
                tcs.SetResult(confirmedResult.Receipt);
                return;
            }
            if(txResult is not TxConfirmationResult.TimedOut timeoutResult)
            {
                throw new ImpossibleException();
            }

            TxTimeoutAction action;

            while(true)
            {
                try
                {
                    action = await onTxTimeout();
                    break;
                }
                catch
                {
                }
            }

            txResult = action switch
            {
                TxTimeoutAction.ContinueWaiting waitAction => await _txConfirmer.WaitForTxConfirmationAsync(txHash, waitAction.Duration),

                _ => throw new ImpossibleException(),
            };
        }
    }

    public void Dispose()
    {
        _queue.Writer.TryComplete();
        GC.SuppressFinalize(this);
    }
}
