using EtherSharp.Client.Services.GasFeeProvider;
using EtherSharp.Client.Services.RPC;
using EtherSharp.Client.Services.TxConfirmer;
using EtherSharp.Client.Services.TxPublisher;
using EtherSharp.Common.Exceptions;
using EtherSharp.Crypto;
using EtherSharp.RLP;
using EtherSharp.Tx;
using EtherSharp.Tx.EIP1559;
using EtherSharp.Tx.Types;
using EtherSharp.Types;
using EtherSharp.Wallet;
using System.Threading.Channels;

using QueueEntry = (
    EtherSharp.Tx.Types.ITxParams TxParams,
    EtherSharp.Tx.ITxInput TxInput,
    System.Func<System.Threading.Tasks.ValueTask<EtherSharp.Tx.TxTimeoutAction>> OnTxTimeout,
    System.Threading.Tasks.TaskCompletionSource<EtherSharp.Types.TransactionReceipt> CompletionSource
);

namespace EtherSharp.Client.Services.TxScheduler;
public class BlockingSequentialTxScheduler : ITxScheduler, IInitializableService, IDisposable
{
    private readonly Channel<QueueEntry> _queue;

    private readonly IRpcClient _rpcClient;
    private readonly IEtherSigner _signer;
    private readonly ITxPublisher _txPublisher;
    private readonly ITxConfirmer _txConfirmer;
    private readonly IGasFeeProvider _gasFeeProvider;

    private readonly TimeSpan _txTimeout = TimeSpan.FromSeconds(30);

    private ulong _chainId;
    private uint _nonceCounter;

    public BlockingSequentialTxScheduler(IRpcClient rpcClient, IEtherSigner signer,
        ITxPublisher txPublisher, ITxConfirmer txConfirmer, IGasFeeProvider gasFeeProvider)
    {
        _queue = Channel.CreateUnbounded<QueueEntry>(new UnboundedChannelOptions()
        {
            SingleReader = true,
            SingleWriter = false,
            AllowSynchronousContinuations = false
        });

        _rpcClient = rpcClient;
        _signer = signer;
        _txPublisher = txPublisher;
        _txConfirmer = txConfirmer;
        _gasFeeProvider = gasFeeProvider;
    }

    public async ValueTask InitializeAsync(ulong chainId, CancellationToken cancellationToken)
    {
        _chainId = chainId;
        _nonceCounter = await _rpcClient.EthGetTransactionCount(
            _signer.Address.String, TargetBlockNumber.Latest, cancellationToken) - 1;

        _ = Task.Run(BackgroundTxProcessor, CancellationToken.None);
    }

    public Task<TransactionReceipt> PublishTxAsync<TTxParams>(TTxParams txParams, ITxInput txInput, Func<ValueTask<TxTimeoutAction>> onTxTimeout)
        where TTxParams : ITxParams
    {
        var tcs = new TaskCompletionSource<TransactionReceipt>();

        return !_queue.Writer.TryWrite((txParams, txInput, onTxTimeout, tcs))
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
        var (txParams, txInput, _, _) = entry;
        uint nonce = Interlocked.Increment(ref _nonceCounter);

        while(true)
        {
            var submissionResult = txParams switch
            {
                EIP1559TxParams eip1559Params
                    => await EncodeAndPublishTransactionAsync<EIP1559Transaction, EIP1559TxParams, EIP1559GasParams>(eip1559Params, txInput, nonce),
                _ => throw new NotSupportedException($"TxParams type {txParams.GetType().Name} is not supported")
            };

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
        var (_, _, onTxTimeout, tcs) = entry;

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

    private async Task<TxSubmissionResult> EncodeAndPublishTransactionAsync<TTransaction, TTxParams, TTxGasParams>(
        TTxParams txParams, ITxInput txInput, uint nonce
    )
        where TTxParams : ITxParams
        where TTxGasParams : ITxGasParams
        where TTransaction : ITransaction<TTransaction, TTxParams, TTxGasParams>
    {
        //ToDo: Consider avoiding this allocation
        byte[] dataBuffer = new byte[txInput.DataLength];
        txInput.WriteDataTo(dataBuffer);

        ulong gasEstimation = await _gasFeeProvider.EstimateGasAsync(txInput, dataBuffer);
        var gasParams = (TTxGasParams) await _gasFeeProvider.CalculateGasParamsAsync(txInput, txParams, gasEstimation);

        string calldata = EncodeCallData<TTransaction, TTxParams, TTxGasParams>(txInput, txParams, gasParams, dataBuffer, nonce, gasEstimation);

        return await _txPublisher.PublishTxAsync(calldata);
    }

    private string EncodeCallData<TTransaction, TTxParams, TTxGasParams>(
        ITxInput txInput, TTxParams txParams, TTxGasParams txGasParams,
        ReadOnlySpan<byte> dataBuffer,
        uint nonce, ulong gas
    )
        where TTransaction : ITransaction<TTransaction, TTxParams, TTxGasParams>
        where TTxParams : ITxParams
        where TTxGasParams : ITxGasParams
    {
        var tx = TTransaction.Create(_chainId, txParams, txGasParams, txInput, nonce, gas);

        Span<int> lengthBuffer = stackalloc int[TTransaction.NestedListCount];

        int txTemplateLength = tx.GetEncodedSize(dataBuffer, lengthBuffer);

        int txBufferLength = 2 + txTemplateLength + TxRLPEncoder.MaxEncodedSignatureLength;

        Span<byte> txBuffer = txBufferLength > 4096
            ? new byte[txBufferLength]
            : stackalloc byte[txBufferLength];

        var txTemplateBuffer = txBuffer[1..(txTemplateLength + 2)];
        var signatureBuffer = txBuffer[^TxRLPEncoder.MaxEncodedSignatureLength..];

        tx.Encode(lengthBuffer, dataBuffer, txTemplateBuffer[1..]);
        txTemplateBuffer[0] = TTransaction.PrefixByte;

        SignAndEncode(txTemplateBuffer, signatureBuffer, out int signatureLength);

        int oldLengthBytes = RLPEncoder.GetPrefixLength(lengthBuffer[0]);
        int newLengthBytes = RLPEncoder.GetPrefixLength(lengthBuffer[0] + signatureLength);

        if(newLengthBytes == oldLengthBytes)
        {
            //Dont need the extra byte for the length increase
            txBuffer = txBuffer[1..];
        }

        txBuffer[0] = TTransaction.PrefixByte;
        _ = new RLPEncoder(txBuffer[1..]).EncodeList(lengthBuffer[0] + signatureLength);

        var signedTxBuffer = txBuffer[..^(TxRLPEncoder.MaxEncodedSignatureLength - signatureLength)];

        return $"0x{Convert.ToHexString(signedTxBuffer)}";
    }

    internal void SignAndEncode(Span<byte> txTemplateBuffer, Span<byte> signatureBuffer, out int encodedSignatureLength)
    {
        Span<byte> hashBuffer = stackalloc byte[32];
        _ = Keccak256.TryHashData(txTemplateBuffer, hashBuffer);

        Span<byte> rawSignatureBuffer = stackalloc byte[65];
        if(!_signer!.TrySignRecoverable(hashBuffer, rawSignatureBuffer))
        {
            throw new NotImplementedException();
        }
        _ = new RLPEncoder(signatureBuffer).EncodeSignature(rawSignatureBuffer, out encodedSignatureLength);
    }

    public void Dispose()
    {
        _queue.Writer.TryComplete();
        GC.SuppressFinalize(this);
    }
}
