using EtherSharp.RPC.Modules.Eth;
using EtherSharp.RPC.Transport;
using EtherSharp.Types;

namespace EtherSharp.Realtime.Events.Filter;

internal sealed class EventFilter<TLog>(IRPCTransport rpcTransport, IEthRpcModule ethRpcModule,
    TargetHeight fromBlock, TargetHeight toBlock,
    Address[]? addresses, string[]?[]? topics, RpcRequestOptions requestOptions
) : IEventFilter<TLog>
    where TLog : ITxLog<TLog>
{
    public string Id { get; private set; } = null!;

    private readonly IRPCTransport _rpcTransport = rpcTransport;
    private readonly IEthRpcModule _ethRpcModule = ethRpcModule;

    private readonly TargetHeight _fromBlock = fromBlock;
    private readonly TargetHeight _toBlock = toBlock;

    private readonly Address[]? _addresses = addresses;
    private readonly string[]?[]? _topics = topics;
    private readonly RpcRequestOptions _requestOptions = requestOptions;

    public async Task<TLog[]> GetChangesAsync(RpcRequestOptions requestOptions, CancellationToken cancellationToken)
    {
        var rawResults = await _ethRpcModule.GetEventFilterChangesAsync(Id, requestOptions, cancellationToken);

        if(rawResults.Length == 0)
        {
            return [];
        }

        var results = new TLog[rawResults.Length];

        for(int i = 0; i < rawResults.Length; i++)
        {
            results[i] = TLog.Decode(rawResults[i]);
        }

        return results;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        _rpcTransport.OnConnectionEstablished += HandleReconnect;
        await InstallAsync(_requestOptions, cancellationToken);
    }

    private async Task InstallAsync(RpcRequestOptions requestOptions, CancellationToken cancellationToken = default)
        => Id = await _ethRpcModule.NewFilterAsync(_fromBlock, _toBlock, _addresses, _topics, requestOptions, cancellationToken);

    private void HandleReconnect()
        => _ = Task.Run(() => InstallAsync(_requestOptions));

    public async ValueTask DisposeAsync()
    {
        _rpcTransport.OnConnectionEstablished -= HandleReconnect;
        await _ethRpcModule.UninstallFilterAsync(Id, _requestOptions);
    }
}
