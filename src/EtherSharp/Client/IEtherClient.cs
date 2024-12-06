using EtherSharp.Client.Services.EtherApi;
using EtherSharp.Client.Services.LogsApi;
using EtherSharp.Contract;
using EtherSharp.Tx;
using EtherSharp.Types;
using System.Numerics;

namespace EtherSharp.Client;
public interface IEtherClient
{
    public ulong ChainId { get; }
    public IEtherApi ETH { get; }
    public ILogsApi Logs { get; }

    public Task InitializeAsync();

    public Task<long> GetPeakHeightAsync();

    public TContract Contract<TContract>(string address)
        where TContract : IEVMContract;
    public Task<T> CallAsync<T>(TxInput<T> call, TargetBlockNumber targetHeight = default);

    public Task<uint> GetTransactionCount(string address, TargetBlockNumber targetHeight = default);
}
