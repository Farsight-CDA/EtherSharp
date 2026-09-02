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
        StorageJournal.Snapshot JournalSnapshot
    );
    public readonly record struct Snapshot(ImmutableArray<AccountSnapshot> AccountSnapshots);

    private readonly Dictionary<Address, InterpreterAccountStorage> _accountStorages = [];

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
            accountSnapshots.Add(new AccountSnapshot(address, accountStorage.CurrentSnapshot));
        }

        return new Snapshot(accountSnapshots.MoveToImmutable());
    }

    public void Reset(Snapshot snapshot)
    {
        if(snapshot.AccountSnapshots.IsDefault)
        {
            throw new ArgumentException("Snapshot is uninitialized.", nameof(snapshot));
        }

        var accountSnapshots = snapshot.AccountSnapshots;
        foreach(var accountSnapshot in accountSnapshots)
        {
            _accountStorages[accountSnapshot.Address].Reset(accountSnapshot.JournalSnapshot);
        }
        foreach(var (address, accountStorage) in _accountStorages)
        {
            if(!accountSnapshots.Any(accountSnapshot => accountSnapshot.Address == address))
            {
                accountStorage.Reset(default);
            }
        }
    }
}
