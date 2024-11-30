using EtherSharp.Client.Services.EtherApi;
using EtherSharp.Contract;
using EtherSharp.Tx;
using EtherSharp.Types;

namespace EtherSharp.Client;
public interface IEtherClient
{
    public ulong ChainId { get; }
    public IEtherApi ETH { get; }

    public Task InitializeAsync();

    public TContract Contract<TContract>(string address)
        where TContract : IEVMContract;
    public Task<T> CallAsync<T>(TxInput<T> call, TargetBlockNumber targetHeight = default);

    public Task<uint> GetTransactionCount(string address, TargetBlockNumber targetHeight = default);
}
