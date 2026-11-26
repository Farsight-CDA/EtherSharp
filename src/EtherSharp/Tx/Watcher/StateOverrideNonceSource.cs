using EtherSharp.Client.Services.FlashCall;
using EtherSharp.Client.Services.QueryExecutor;
using EtherSharp.Query;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.RPC.Transport;
using EtherSharp.Types;
using Microsoft.Extensions.Logging;

namespace EtherSharp.Tx.Watcher;

/// <summary>
/// Reads the nonces of all senders and the block number in a single <c>eth_call</c>. Requires state override support.
/// </summary>
public sealed class StateOverrideNonceSource(
    IEthRpcModule ethRpcModule,
    RpcRequestOptions requestOptions = default,
    ILogger<StateOverrideNonceSource>? logger = null
) : NonceSource
{
    private const ulong NONCE_SEARCH_BELOW = 256;
    private const int NONCE_SEARCH_COUNT = 4096;

    private readonly IEthRpcModule _ethRpcModule = ethRpcModule ?? throw new ArgumentNullException(nameof(ethRpcModule));
    private readonly QueryExecutor _queryExecutor = new QueryExecutor(new StateOverrideFlashCallExecutor(ethRpcModule));
    private readonly RpcRequestOptions _requestOptions = requestOptions;
    private readonly ILogger<StateOverrideNonceSource>? _logger = logger;

    /// <inheritdoc/>
    public override async Task<Snapshot> ReadAsync(ReadOnlyMemory<Request> requests, CancellationToken cancellationToken)
    {
        var nonceQueries = new IQuery<NonceSearchResult>[requests.Length];
        for(int i = 0; i < nonceQueries.Length; i++)
        {
            var (sender, startNonce) = requests.Span[i];
            ulong searchStart = startNonce - Math.Min(startNonce, NONCE_SEARCH_BELOW);
            nonceQueries[i] = IQuery.WithCaller(sender, IQuery.GetNonce(searchStart, NONCE_SEARCH_COUNT));
        }

        var (blockNumber, searchResults) = await _queryExecutor.ExecuteQueryAsync(
            IQuery.Combine(IQuery.GetBlockHeight(), IQuery.Range<NonceSearchResult>(nonceQueries)),
            null,
            TargetHeight.Latest,
            _requestOptions,
            cancellationToken
        );

        ulong[] nonces = new ulong[searchResults.Length];
        for(int i = 0; i < nonces.Length; i++)
        {
            if(searchResults[i] is NonceSearchResult.Found found)
            {
                nonces[i] = found.Nonce;
                continue;
            }

            // The query only yields a nonce that lies within its search range.
            var (sender, startNonce) = requests.Span[i];
            ulong searchStart = startNonce - Math.Min(startNonce, NONCE_SEARCH_BELOW);
            nonces[i] = await _ethRpcModule.GetTransactionCountAsync(
                in sender, TargetHeight.Height(blockNumber), _requestOptions, cancellationToken
            );

            if(_logger?.IsEnabled(LogLevel.Warning) == true)
            {
                _logger.LogWarning(
                    "Nonce {Nonce} of {Sender} is outside the search range starting at {SearchStart}; read it with eth_getTransactionCount at block {BlockNumber}",
                    nonces[i], sender, searchStart, blockNumber
                );
            }
        }

        return new Snapshot(blockNumber, nonces);
    }
}
