using EtherSharp.Client.Services.RPC;
using EtherSharp.Common.Exceptions;
using System.Diagnostics;

namespace EtherSharp.Client.Services.TxConfirmer;
public class PollingTxConfirmer : ITxConfirmer
{
    private readonly IRpcClient _rpcClient;

    public PollingTxConfirmer(IRpcClient rpcClient)
    {
        _rpcClient = rpcClient;
    }

    public async Task<TxConfirmationResult> WaitForTxConfirmationAsync(string txHash, TimeSpan timeoutDuration, CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(750));
        long startTime = Stopwatch.GetTimestamp();

        while(await timer.WaitForNextTickAsync(cancellationToken))
        {
            if(timeoutDuration < Stopwatch.GetElapsedTime(startTime))
            {
                return new TxConfirmationResult.TimedOut();
            }

            var receipt = await _rpcClient.EthGetTransactionReceiptAsync(txHash);

            if(receipt is null)
            {
                continue;
            }

            return new TxConfirmationResult.Confirmed(receipt);
        }

        throw new ImpossibleException();
    }
}
