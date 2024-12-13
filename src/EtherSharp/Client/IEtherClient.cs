using EtherSharp.Client.Services.EtherApi;
using EtherSharp.Client.Services.LogsApi;
using EtherSharp.Contract;
using EtherSharp.Events;
using EtherSharp.Tx;
using EtherSharp.Types;

namespace EtherSharp.Client;
public interface IEtherClient
{
    public ulong ChainId { get; }
    public IEtherApi ETH { get; }

    public ILogsApi<TEvent> Logs<TEvent>() where TEvent : ITxEvent<TEvent>;
    public ILogsApi<Log> Logs() => Logs<Log>();

    public Task InitializeAsync(CancellationToken cancellationToken = default);

    public Task<long> GetPeakHeightAsync();

    public TContract Contract<TContract>(string address)
        where TContract : IEVMContract;
    public TContract Contract<TContract>(Address address)
        where TContract : IEVMContract;

    public Task<T> CallAsync<T>(TxInput<T> call, TargetBlockNumber targetHeight = default);

    public Task<uint> GetTransactionCount(string address, TargetBlockNumber targetHeight = default);
}
