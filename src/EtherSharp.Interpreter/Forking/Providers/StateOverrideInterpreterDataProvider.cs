using EtherSharp.Client;
using EtherSharp.Client.Modules.Query;
using EtherSharp.Interpreter.Runtime;
using EtherSharp.Query;
using EtherSharp.RPC.Transport;
using EtherSharp.Tx;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Forking.Providers;

/// <summary>
/// Resolves interpreter state through state-override queries, with exact RPC fallback for nonce-search misses.
/// </summary>
/// <remarks>
/// Each instance belongs to one fork at a fixed numeric RPC height and must not fetch concurrently.
/// Code and code-hash reads cannot share a batch with code overrides at the same address.
/// Requests are grouped by address, with code/hash reads and precompile calls taking priority over code overrides.
/// Storage slots at an address share one caller context, without prefetching. Nonce probes and
/// precompile calls are isolated so their simulated changes do not affect subsequent operations.
/// A lone nonce request uses exact RPC immediately; unsuccessful bounded probes are not repeated.
/// The provider borrows its client and does not protect numeric block targets against reorganizations.
/// </remarks>
internal sealed class StateOverrideInterpreterDataProvider : IInterpreterDataProvider
{
    private readonly IEtherClient _client;
    private readonly TargetHeight _targetHeight;
    private readonly RpcRequestOptions _requestOptions;
    private readonly HashSet<Address> _nonceProbeMisses = [];

    /// <summary>
    /// Creates a provider pinned to a numeric RPC block height.
    /// </summary>
    /// <param name="client">A client configured for state-override query execution.</param>
    /// <param name="targetHeight">The fixed RPC height used for all reads, including nonce fallback.</param>
    /// <param name="requestOptions">Options applied to upstream requests.</param>
    internal StateOverrideInterpreterDataProvider(
        IEtherClient client,
        TargetHeight targetHeight,
        RpcRequestOptions requestOptions
    )
    {
        _client = client;
        _targetHeight = targetHeight;
        _requestOptions = requestOptions;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<InterpreterDataResult>> FetchAsync(
        InterpreterContext context,
        ReadOnlyMemory<InterpreterDataRequest> requests
    )
    {
        if(requests.IsEmpty)
        {
            return [];
        }
        if(requests.Length == 1 && requests.Span[0] is InterpreterDataRequest.Nonce onlyNonce)
        {
            return [new InterpreterDataResult.Nonce(
                onlyNonce.Address,
                await _client.GetTransactionCount(onlyNonce.Address, _targetHeight, _requestOptions)
            )];
        }

        Dictionary<Address, List<InterpreterDataRequest>> requestsByAddress = [];
        foreach(var request in requests.Span)
        {
            if(!requestsByAddress.TryGetValue(request.GetAddress(), out var group))
            {
                group = [];
                requestsByAddress.Add(request.GetAddress(), group);
            }
            group.Add(request);
        }

        List<IQuery<List<InterpreterDataResult?>>> batch = [];
        List<Address> nonceFallbacks = [];

        foreach(var (address, group) in requestsByAddress)
        {
            // Native precompiles cannot execute the helper code installed by WithCaller either.
            bool requiresOriginalCode = group.Any(static request => request is
                InterpreterDataRequest.Code or InterpreterDataRequest.CodeHash or InterpreterDataRequest.PrecompileCall
            );
            var queries = new QueryBuilder<InterpreterDataResult?>();
            foreach(var request in group)
            {
                switch(request)
                {
                    case InterpreterDataRequest.Balance balance:
                        queries.AddQuery(
                            IQuery.GetBalance(balance.Address),
                            value => new InterpreterDataResult.Balance(balance.Address, value)
                        );
                        break;
                    case InterpreterDataRequest.Code code:
                        queries.AddQuery(
                            IQuery.GetCode(code.Address),
                            value => new InterpreterDataResult.Code(code.Address, value)
                        );
                        break;
                    case InterpreterDataRequest.CodeHash codeHash:
                        queries.AddQuery(
                            IQuery.GetCodeHash(codeHash.Address),
                            value => new InterpreterDataResult.CodeHash(codeHash.Address, Bytes32.FromBytes(value))
                        );
                        break;
                    case InterpreterDataRequest.Storage slot:
                        if(requiresOriginalCode)
                        {
                            continue;
                        }
                        queries.AddQuery(
                            IQuery.ReadStorage(slot.Key),
                            value => new InterpreterDataResult.Storage(slot.Address, slot.Key, value)
                        );
                        break;
                    case InterpreterDataRequest.Nonce nonce:
                        if(_nonceProbeMisses.Contains(nonce.Address))
                        {
                            nonceFallbacks.Add(nonce.Address);
                            break;
                        }
                        if(requiresOriginalCode)
                        {
                            continue;
                        }
                        queries.AddQuery(
                            IQuery.Isolate(IQuery.GetNonce(nonce.Address)),
                            result =>
                            {
                                switch(result)
                                {
                                    case NonceSearchResult.Found found:
                                        return new InterpreterDataResult.Nonce(nonce.Address, found.Nonce);
                                    case NonceSearchResult.NotFound:
                                        _nonceProbeMisses.Add(nonce.Address);
                                        nonceFallbacks.Add(nonce.Address);
                                        return null;
                                    default:
                                        throw new NotSupportedException();
                                }
                            }
                        );
                        break;
                    case InterpreterDataRequest.PrecompileCall call:
                        queries.AddQuery(
                            IQuery.Isolate(
                                IQuery.SafeCall(IContractCall.ForRawContractCall(call.Target, call.Value, call.Input))
                            ),
                            result => new InterpreterDataResult.PrecompileCall(
                                call.Caller, call.Target, call.Value, call.Input,
                                result switch
                                {
                                    CallResult<ReadOnlyMemory<byte>>.Success success => new TxCallResult(true, success.Value),
                                    CallResult<ReadOnlyMemory<byte>>.Reverted reverted => new TxCallResult(false, reverted.Data),
                                    CallResult<ReadOnlyMemory<byte>>.Malformed malformed => throw malformed.Exception,
                                    _ => throw new NotSupportedException(),
                                }
                            )
                        );
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            if(queries.Queries.Count != 0)
            {
                batch.Add(
                    requiresOriginalCode
                        ? queries
                        : IQuery.WithCaller(address, queries)
                );
            }
        }

        List<InterpreterDataResult> resolved = [];
        if(batch.Count != 0)
        {
            var queryResults = await _client.QueryAsync(
                IQuery.Range(batch),
                options: new CallOptions { TargetHeight = _targetHeight },
                requestOptions: _requestOptions
            );
            foreach(var result in queryResults.SelectMany(static results => results))
            {
                if(result is null)
                {
                    continue;
                }
                resolved.Add(result);
            }
        }

        if(nonceFallbacks.Count != 0)
        {
            var results = await Task.WhenAll(nonceFallbacks.Select(async address => new InterpreterDataResult.Nonce(
                address,
                await _client.GetTransactionCount(address, _targetHeight, _requestOptions)
            )));
            resolved.AddRange(results);
        }
        return resolved;
    }
}
