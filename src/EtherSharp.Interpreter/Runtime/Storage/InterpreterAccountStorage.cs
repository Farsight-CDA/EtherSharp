using EtherSharp.Contract;
using EtherSharp.Crypto;
using EtherSharp.Interpreter.Runtime;
using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Runtime.Storage;

internal sealed class InterpreterAccountStorage(
    Address address,
    IInterpreterHost host,
    Func<long> nextRevision
)
{
    private static Bytes32 EmptyCodeHash { get; } = Keccak256.HashData([]);

    private readonly Address _address = address;
    private readonly IInterpreterHost _host = host;
    private readonly Func<long> _nextRevision = nextRevision;

    private readonly JournaledMap<Bytes32, Bytes32> _persistentStorage = new();
    private readonly JournaledMap<Bytes32, Bytes32> _transientStorage = new();
    private readonly JournaledValue<UInt256> _balance = new();
    private readonly JournaledValue<ulong> _nonce = new();
    private readonly JournaledValue<EVMByteCode> _code = new();
    private readonly JournaledValue<Bytes32> _codeHash = new();
    private readonly JournaledValue<bool> _present = new();
    private readonly JournaledFlag _persistentStorageReplaced = new();
    private readonly JournaledFlag _createdInTransaction = new();
    private readonly JournaledFlag _scheduledForDeletion = new();

    public bool IsCreatedInTransaction
        => _createdInTransaction.IsSet;

    public async ValueTask<Bytes32> SLoadAsync(Bytes32 key)
        => _persistentStorage.TryGetValue(in key, out var value)
            ? value
            : _persistentStorageReplaced.IsSet
                ? Bytes32.Zero
                : await _host.GetStorageAtAsync(_address, key);

    public void SStore(in Bytes32 key, in Bytes32 value)
    {
        long revision = _nextRevision();
        _present.Set(revision, true);
        _persistentStorage.Set(revision, in key, in value);
    }

    public Bytes32 TLoad(in Bytes32 key)
        => _transientStorage.TryGetValue(in key, out var value)
            ? value
            : Bytes32.Zero;

    public void TStore(in Bytes32 key, in Bytes32 value)
        => _transientStorage.Set(_nextRevision(), in key, in value);

    public async ValueTask<UInt256> GetBalanceAsync()
        => _balance.TryGetValue(out var value)
            ? value
            : await _host.GetBalanceAsync(_address);

    public void SetBalance(in UInt256 value)
    {
        long revision = _nextRevision();
        _present.Set(revision, true);
        _balance.Set(revision, in value);
    }

    public async ValueTask<ulong> GetNonceAsync()
        => _nonce.TryGetValue(out ulong value)
            ? value
            : await _host.GetNonceAsync(_address);

    public void SetNonce(ulong value)
    {
        long revision = _nextRevision();
        _present.Set(revision, true);
        _nonce.Set(revision, in value);
    }

    public async ValueTask<EVMByteCode> GetCodeAsync()
        => _code.TryGetValue(out var value)
            ? value
            : await _host.GetCodeAsync(_address);

    public void SetCode(in EVMByteCode value)
    {
        long revision = _nextRevision();
        var codeHash = Keccak256.HashData(value.ByteCode.Span);
        _present.Set(revision, true);
        _code.Set(revision, in value);
        _codeHash.Set(revision, in codeHash);
    }

    public void InitializeCreatedContract()
    {
        long revision = _nextRevision();
        _present.Set(revision, true);
        _nonce.Set(revision, 1);
        _code.Set(revision, EVMByteCode.Empty);
        _codeHash.Set(revision, EmptyCodeHash);
        _persistentStorage.Clear(revision);
        _persistentStorageReplaced.Set(revision);
        _createdInTransaction.Set(revision);
    }

    public void ScheduleDeletion()
        => _scheduledForDeletion.Set(_nextRevision());

    public void ApplyOverride(AccountOverride accountOverride)
    {
        if(accountOverride.Balance is { } balance)
        {
            SetBalance(in balance);
        }
        if(accountOverride.Nonce is { } nonce)
        {
            SetNonce(nonce);
        }
        if(accountOverride.Code is { } code)
        {
            SetCode(new EVMByteCode(code));
        }
        if(accountOverride.State is { } state)
        {
            long revision = _nextRevision();
            _persistentStorage.Clear(revision);
            _persistentStorageReplaced.Set(revision);
            foreach(var (key, value) in state)
            {
                SStore(in key, in value);
            }
        }
        else if(accountOverride.StateDiff is { } stateDiff)
        {
            foreach(var (key, value) in stateDiff)
            {
                SStore(in key, in value);
            }
        }
    }

    public async ValueTask<Bytes32> GetExtCodeHashAsync()
    {
        bool hasLocalPresence = _present.TryGetValue(out bool isPresent);
        if(hasLocalPresence && !isPresent)
        {
            return Bytes32.Zero;
        }

        var codeHash = _codeHash.TryGetValue(out var localCodeHash)
            ? localCodeHash
            : await _host.GetCodeHashAsync(_address);
        if(codeHash is null && !hasLocalPresence)
        {
            return Bytes32.Zero;
        }

        var effectiveCodeHash = codeHash ?? EmptyCodeHash;
        return effectiveCodeHash == EmptyCodeHash
            && await GetNonceAsync() == 0
            && await GetBalanceAsync() == UInt256.Zero
                ? Bytes32.Zero
                : effectiveCodeHash;
    }

    public async ValueTask<bool> HasCreateCollisionAsync()
        => await GetNonceAsync() != 0
            || await GetStateCodeHashAsync() != EmptyCodeHash;

    public void Commit()
    {
        if(_scheduledForDeletion.IsSet)
        {
            long revision = _nextRevision();
            _persistentStorage.Clear(revision);
            _persistentStorageReplaced.Set(revision);
            _balance.Set(revision, UInt256.Zero);
            _nonce.Set(revision, 0);
            _code.Set(revision, EVMByteCode.Empty);
            _codeHash.Set(revision, EmptyCodeHash);
            _present.Set(revision, false);
        }

        _persistentStorage.Commit();
        _transientStorage.Clear();
        _balance.Commit();
        _nonce.Commit();
        _code.Commit();
        _codeHash.Commit();
        _present.Commit();
        _persistentStorageReplaced.Commit();
        _createdInTransaction.Clear();
        _scheduledForDeletion.Clear();
    }

    public void Reset(long revision)
    {
        _persistentStorage.Reset(revision);
        _transientStorage.Reset(revision);
        _balance.Reset(revision);
        _nonce.Reset(revision);
        _code.Reset(revision);
        _codeHash.Reset(revision);
        _present.Reset(revision);
        _persistentStorageReplaced.Reset(revision);
        _createdInTransaction.Reset(revision);
        _scheduledForDeletion.Reset(revision);
    }

    private async ValueTask<Bytes32> GetStateCodeHashAsync()
    {
        var codeHash = _codeHash.TryGetValue(out var value)
            ? value
            : await _host.GetCodeHashAsync(_address);

        return codeHash ?? EmptyCodeHash;
    }
}
