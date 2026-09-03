using EtherSharp.Interpreter.Runtime;
using EtherSharp.Types;
using System.Collections.Immutable;

namespace EtherSharp.Interpreter.Storage;

internal sealed class InterpreterStorage(
    InterpreterContext context,
    IGlobalStateProvider globalStateProvider
)
{
    public readonly record struct AccountSnapshot(
        Address Address,
        InterpreterAccountStorage.Snapshot StorageSnapshot
    );
    public readonly record struct Snapshot(
        ImmutableArray<AccountSnapshot> AccountSnapshots,
        int LogCount
    );
    private readonly record struct JournalLog(
        Address Address,
        ReadOnlyMemory<Bytes32> Topics,
        ReadOnlyMemory<byte> Data
    );

    private readonly Dictionary<Address, InterpreterAccountStorage> _accountStorages = [];
    private readonly List<JournalLog> _logs = [];

    public InterpreterAccountStorage GetAccountStorage(Address address)
    {
        if(!_accountStorages.TryGetValue(address, out var accountStorage))
        {
            accountStorage = new InterpreterAccountStorage(address, context, globalStateProvider);
            _accountStorages.Add(address, accountStorage);
        }

        return accountStorage;
    }

    public Snapshot TakeSnapshot()
    {
        var accountSnapshots = ImmutableArray.CreateBuilder<AccountSnapshot>(_accountStorages.Count);
        foreach(var (address, accountStorage) in _accountStorages)
        {
            accountSnapshots.Add(new AccountSnapshot(address, accountStorage.TakeSnapshot()));
        }

        return new Snapshot(accountSnapshots.MoveToImmutable(), _logs.Count);
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
    }

    public void Reset(Snapshot snapshot)
    {
        if(snapshot.AccountSnapshots.IsDefault)
        {
            throw new ArgumentException("Snapshot is uninitialized.", nameof(snapshot));
        }
        if(snapshot.LogCount < 0 || snapshot.LogCount > _logs.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(snapshot));
        }

        foreach(var accountSnapshot in snapshot.AccountSnapshots)
        {
            _accountStorages[accountSnapshot.Address].Reset(accountSnapshot.StorageSnapshot);
        }

        foreach(var (address, accountStorage) in _accountStorages)
        {
            if(!snapshot.AccountSnapshots.Any(accountSnapshot => accountSnapshot.Address == address))
            {
                accountStorage.Reset(default);
            }
        }

        _logs.RemoveRange(snapshot.LogCount, _logs.Count - snapshot.LogCount);
    }
}
