using EtherSharp.Client.Services.RPC;
using EtherSharp.Common.Exceptions;
using Keysmith.Net.Wallet;

namespace EtherSharp.Client.Services.TxPublisher;
public class BasicTxPublisher(IRpcClient rpcClient) : ITxPublisher
{
    private readonly IRpcClient _rpcClient = rpcClient;

    /// <inheritdoc/>
    public async Task<TxSubmissionResult> PublishTxAsync(string transactionHex, CancellationToken cancellationToken)
    {
        try
        {
            return await _rpcClient.EthSendRawTransactionAsync(transactionHex, cancellationToken);
        }
        catch (RPCException ex) 
        {
            if (ex.Message.Contains("ALREADY_EXISTS"))
            {
                return new TxSubmissionResult.AlreadyExists();
            }
            else if (ex.Message.Contains("transaction underpriced"))
            {
                return new TxSubmissionResult.TransactionUnderpriced();
            }

            return new TxSubmissionResult.UnhandledException(ex);
        }
        catch(Exception ex)
        {
            return new TxSubmissionResult.UnhandledException(ex);
        }
    }
}
