using EtherSharp.Crypto;
using EtherSharp.RLP;
using EtherSharp.Tx.Types;
using EtherSharp.Tx;
using EtherSharp.Wallet;
using System.Threading.Channels;

using QueueEntry = (
    EtherSharp.Tx.Types.ITxParams TxParams,
    EtherSharp.Tx.ITxInput TxInput,
    System.Threading.Tasks.TaskCompletionSource<string> CompletionSource
);
using EtherSharp.Client.Services.TxPublisher;
using EtherSharp.Tx.EIP1559;

namespace EtherSharp.Client.Services.TxScheduler;
public class SequentialTxScheduler : ITxScheduler, IInitializableService, IDisposable
{
    private readonly Channel<QueueEntry> _queue;

    private readonly IEtherSigner _signer;
    private readonly ITxPublisher _txPublisher;

    private ulong _chainId;

    public SequentialTxScheduler(IEtherSigner signer, ITxPublisher txPublisher)
    {
        _queue = Channel.CreateUnbounded<QueueEntry>(new UnboundedChannelOptions()
        {
            SingleReader = true,
            SingleWriter = false,
            AllowSynchronousContinuations = false
        });

        _signer = signer;
        _txPublisher = txPublisher;
    }

    public ValueTask InitializeAsync(ulong chainId)
    {
        _chainId = chainId;
        return ValueTask.CompletedTask;
    }

    public Task<string> PublishTxAsync<TTxParams>(TTxParams txParams, ITxInput txInput)
        where TTxParams : ITxParams
    {
        var tcs = new TaskCompletionSource<string>();

        return !_queue.Writer.TryWrite((txParams, txInput, tcs))
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
        var (txParams, txInput, tcs) = entry;

        string callData = txParams switch
        {
            EIP1559TxParams eip1559Params => EncodeCallData<EIP1559Transaction, EIP1559TxParams>(eip1559Params, txInput, 1),
            _ => throw new NotSupportedException($"TxParams type {txParams.GetType().Name} is not supported")
        };

        string txHash = await _txPublisher.PublishTxAsync(callData);
        tcs.SetResult(txHash);
    }

    internal string EncodeCallData<TTransaction, TTxParams>(TTxParams txParams, ITxInput txInput, uint nonce)
        where TTransaction : ITransaction<TTransaction, TTxParams>
        where TTxParams : ITxParams
    {
        var tx = TTransaction.Create(_chainId, txParams, txInput, nonce);

        Span<int> lengthBuffer = stackalloc int[TTransaction.NestedListCount];
        Span<byte> dataBuffer = txInput.DataLength > 4096
            ? new byte[txInput.DataLength]
            : stackalloc byte[txInput.DataLength];

        txInput.WriteDataTo(dataBuffer);
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

        int oldLengthBytes = RLPEncoder.GetSignificantByteCount((uint) lengthBuffer[0]);
        int newLengthBytes = RLPEncoder.GetSignificantByteCount((uint) (lengthBuffer[0] + signatureLength));

        if(newLengthBytes == oldLengthBytes)
        {
            //Dont need the extra byte for the length increase
            txBuffer = txBuffer[1..];
        }

        _ = new RLPEncoder(txBuffer[1..]).EncodeList(lengthBuffer[0] + signatureLength);
        txBuffer[0] = TTransaction.PrefixByte;

        var signedTxBuffer = txBuffer[..^(TxRLPEncoder.MaxEncodedSignatureLength - signatureLength)];

        return Convert.ToHexString(signedTxBuffer);
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
