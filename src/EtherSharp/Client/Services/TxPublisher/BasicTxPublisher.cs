using EtherSharp.Common.Exceptions;
using EtherSharp.RPC.Modules.Eth;

namespace EtherSharp.Client.Services.TxPublisher;

public class BasicTxPublisher(IEthRpcModule ethRpcModule) : ITxPublisher
{
    private readonly IEthRpcModule _ethRpcModule = ethRpcModule;

    /// <inheritdoc/>
    public async Task<TxSubmissionResult> PublishTxAsync(string transactionHex, CancellationToken cancellationToken)
    {
        try
        {
            return await _ethRpcModule.SendRawTransactionAsync(transactionHex, cancellationToken);
        }
        catch(RPCException ex)
        {
            if(ex.Message.Contains("ALREADY_EXISTS"))
            {
                return new TxSubmissionResult.AlreadyExists();
            }
            else if(ex.Message.Contains("transaction underpriced") || ex.Message.Contains("max fee per gas less than block base fee"))
            {
                return new TxSubmissionResult.TransactionUnderpriced();
            }
            else if(ex.Message.Contains("nonce too low") || ex.Message.Contains("next nonce"))
            {
                return new TxSubmissionResult.NonceTooLow();
            }

            return new TxSubmissionResult.UnhandledException(ex);
        }
        catch(Exception ex)
        {
            return new TxSubmissionResult.UnhandledException(ex);
        }
    }
}
