using EtherSharp.Interpreter.Runtime;
using EtherSharp.Interpreter.Runtime.Tracing;
using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Runtime.Storage;

internal sealed partial class InterpreterStorage(
    IInterpreterHost host
) : IInterpreterStorage
{
    public readonly record struct Snapshot(
        long Revision,
        int LogCount
    );
    private readonly record struct JournalLog(
        Address Address,
        ReadOnlyMemory<Bytes32> Topics,
        ReadOnlyMemory<byte> Data
    );

    private readonly Dictionary<Address, InterpreterAccountStorage> _accountStorages = [];
    private readonly List<JournalLog> _logs = [];
    private long _revision;

    public InterpreterAccountStorage GetAccountStorage(Address address)
    {
        if(!_accountStorages.TryGetValue(address, out var accountStorage))
        {
            accountStorage = new InterpreterAccountStorage(NextRevision);
            _accountStorages.Add(address, accountStorage);
        }

        return accountStorage;
    }

    private long NextRevision()
        => _revision = checked(_revision + 1);

    public void ApplyStateOverrides(IReadOnlyDictionary<Address, AccountOverride> stateOverrides)
    {
        foreach(var (address, accountOverride) in stateOverrides)
        {
            GetAccountStorage(address).ApplyOverride(accountOverride);
        }
    }

    public Snapshot TakeSnapshot()
        => new(_revision, _logs.Count);

    public void CopyTo(InterpreterStorage target)
    {
        foreach(var (address, accountStorage) in _accountStorages)
        {
            accountStorage.CopyTo(target.GetAccountStorage(address));
        }
    }

    public void AddLog(Address address, Bytes32[] topics, byte[] data)
        => _logs.Add(new JournalLog(address, topics, data));

    public void Commit()
    {
        foreach(var accountStorage in _accountStorages.Values)
        {
            accountStorage.Commit();
        }

        _logs.Clear();
        _revision = 0;
    }

    public void Reset(Snapshot snapshot)
    {
        if(snapshot.Revision < 0 || snapshot.Revision > _revision)
        {
            throw new ArgumentOutOfRangeException(nameof(snapshot));
        }
        if(snapshot.LogCount < 0 || snapshot.LogCount > _logs.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(snapshot));
        }

        foreach(var accountStorage in _accountStorages.Values)
        {
            accountStorage.Reset(snapshot.Revision);
        }

        _logs.RemoveRange(snapshot.LogCount, _logs.Count - snapshot.LogCount);
        _revision = snapshot.Revision;
    }

    ValueTask<UInt256> IInterpreterStorage.GetBalanceAsync(Address address)
        => GetAsync(StateRequest.Balance(address));

    ValueTask<ulong> IInterpreterStorage.GetNonceAsync(Address address)
        => GetAsync(StateRequest.Nonce(address));

    async ValueTask<ReadOnlyMemory<byte>> IInterpreterStorage.GetCodeAsync(Address address)
        => (await GetAsync(StateRequest.Code(address))).ByteCode;

    ValueTask<Bytes32> IInterpreterStorage.GetCodeHashAsync(Address address)
        => GetExtCodeHashAsync(address);

    ValueTask<Bytes32> IInterpreterStorage.GetStorageAsync(Address address, Bytes32 key)
        => GetAsync(StateRequest.Storage(address, key));

    Bytes32 IInterpreterStorage.GetTransientStorage(Address address, Bytes32 key)
        => GetAccountStorage(address).TLoad(in key);
}
