using EtherSharp.RPC;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.Types;

namespace EtherSharp.Realtime.Events.Filter;

internal class EventFilter<TLog>(IRpcClient rpcClient, IEthRpcModule ethRpcModule,
    TargetBlockNumber fromBlock, TargetBlockNumber toBlock,
    Address[]? addresses, string[]?[]? topics
) : IEventFilter<TLog>
    where TLog : ITxLog<TLog>
{
    public string Id { get; private set; } = null!;

    private readonly IRpcClient _rpcClient = rpcClient;
    private readonly IEthRpcModule _ethRpcModule = ethRpcModule;

    private readonly TargetBlockNumber _fromBlock = fromBlock;
    private readonly TargetBlockNumber _toBlock = toBlock;

    private readonly Address[]? _addresses = addresses;
    private readonly string[]?[]? _topics = topics;

    public async Task<TLog[]> GetChangesAsync(CancellationToken cancellationToken)
    {
        var rawResults = await _ethRpcModule.GetEventFilterChangesAsync(Id, cancellationToken);
        return [.. rawResults.Select(TLog.Decode)];
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        _rpcClient.OnConnectionEstablished += HandleReconnect;
        await InstallAsync(cancellationToken);
    }

    private async Task InstallAsync(CancellationToken cancellationToken = default)
        => Id = await _ethRpcModule.NewFilterAsync(_fromBlock, _toBlock, _addresses, _topics, cancellationToken);

    private void HandleReconnect()
        => _ = Task.Run(() => InstallAsync());

    public async ValueTask DisposeAsync()
    {
        _rpcClient.OnConnectionEstablished -= HandleReconnect;
        await _ethRpcModule.UninstallFilterAsync(Id);
    }
}
