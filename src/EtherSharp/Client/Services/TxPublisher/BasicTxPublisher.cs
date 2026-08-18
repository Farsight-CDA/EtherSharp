using EtherSharp.Common.Exceptions;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.RPC.Transport;
using System.Buffers;

namespace EtherSharp.Client.Services.TxPublisher;

/// <summary>
/// Publishes signed transactions through the configured Ethereum RPC module.
/// </summary>
/// <param name="ethRpcModule">RPC module used to submit raw transactions.</param>
public sealed class BasicTxPublisher(IEthRpcModule ethRpcModule) : ITxPublisher
{
    private static readonly SearchValues<string> _alreadyExistsMessages = SearchValues.Create(
        ["ALREADY_EXISTS", "already known", "tx already exists in cache", "known transaction"],
        StringComparison.OrdinalIgnoreCase
    );

    private static readonly SearchValues<string> _transactionUnderpricedMessages = SearchValues.Create(
        ["transaction underpriced", "max fee per gas less than block base fee"],
        StringComparison.Ordinal
    );

    private static readonly SearchValues<string> _nonceTooLowMessages = SearchValues.Create(
        ["nonce too low", "next nonce"],
        StringComparison.Ordinal
    );

    private readonly IEthRpcModule _ethRpcModule = ethRpcModule;

    /// <inheritdoc/>
    public async Task<TxSubmissionResult> PublishTxAsync(
        string transactionHex, RpcRequestOptions requestOptions = default,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _ethRpcModule.SendRawTransactionAsync(transactionHex, requestOptions, cancellationToken);
        }
        catch(RPCException ex)
        {
            if(ex.Message.AsSpan().ContainsAny(_alreadyExistsMessages))
            {
                return new TxSubmissionResult.AlreadyExists();
            }
            else if(ex.Message.AsSpan().ContainsAny(_transactionUnderpricedMessages))
            {
                return new TxSubmissionResult.TransactionUnderpriced();
            }
            else if(ex.Message.AsSpan().ContainsAny(_nonceTooLowMessages))
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
