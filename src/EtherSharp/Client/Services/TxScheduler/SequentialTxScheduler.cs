using EtherSharp.Crypto;
using EtherSharp.RLP;
using EtherSharp.Tx.Types;
using EtherSharp.Tx;
using EtherSharp.Wallet;
using System.Threading.Channels;

using QueueEntry = (
    EtherSharp.Tx.Types.TxType TxType,
    EtherSharp.Tx.ITxInput TxInput,
    System.Threading.Tasks.TaskCompletionSource<string> CompletionSource
);
using EtherSharp.Client.Services.TxPublisher;

namespace EtherSharp.Client.Services.TxScheduler;
public class SequentialTxScheduler : ITxScheduler, IDisposable
{
    private readonly Channel<QueueEntry> _queue;

    private readonly IEtherSigner _signer;
    private readonly ITxPublisher _txPublisher;

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

    public Task<string> PublishTxAsync(TxType txType, ITxInput txInput)
    {
        var tcs = new TaskCompletionSource<string>();

        return !_queue.Writer.TryWrite((txType, txInput, tcs))
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
        var (txType, txInput, tcs) = entry;

        string callData;

        switch(txType)
        {
            case TxType.EIP1559:
            {
                var tx = new EIP1559Transaction(137, 38154, 103, txInput.To, txInput.Value, 45201065989, 27278237335, []);
                callData = EncodeCallData(tx, txInput);
                break;
            }
            default:
                throw new NotSupportedException($"TxType {txType} is not supported");
        }

        string txHash = await _txPublisher.PublishTxAsync(callData);
        tcs.SetResult(txHash);
    }

    internal string EncodeCallData<TTransaction>(TTransaction tx, ITxInput txInput)
        where TTransaction : ITransaction
    {
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
