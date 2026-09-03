using EtherSharp.Contract;
using EtherSharp.Crypto;
using EtherSharp.Interpreter.Runtime;
using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Storage;

internal sealed class InterpreterAccountStorage
{
    public readonly record struct Snapshot(int Revision);

    private readonly Address _address;
    private readonly InterpreterContext _context;
    private readonly IGlobalStateProvider _globalStateProvider;
    private readonly JournaledMap<Bytes32, Bytes32> _persistentStorage = new();
    private readonly JournaledMap<Bytes32, Bytes32> _transientStorage = new();
    private readonly JournaledValue<UInt256> _balance = new();
    private readonly JournaledValue<ulong> _nonce = new();
    private readonly JournaledValue<AccountCode> _code = new();
    private int _revision;

    public Snapshot TakeSnapshot()
        => new(_revision);

    public InterpreterAccountStorage(
        Address address,
        InterpreterContext context,
        IGlobalStateProvider globalStateProvider
    )
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(globalStateProvider);
        _address = address;
        _context = context;
        _globalStateProvider = globalStateProvider;
    }

    public async ValueTask<Bytes32> SLoadAsync(Bytes32 key)
    {
        if(_persistentStorage.TryGetValue(in key, out var value))
        {
            return value;
        }

        value = await _globalStateProvider.GetStorageAtAsync(_context, _address, key);
        _persistentStorage.Cache(in key, in value);
        return value;
    }

    public void SStore(in Bytes32 key, in Bytes32 value)
        => _persistentStorage.Set(NextRevision(), in key, in value);

    public Bytes32 TLoad(in Bytes32 key)
        => _transientStorage.TryGetValue(in key, out var value)
            ? value
            : Bytes32.Zero;

    public void TStore(in Bytes32 key, in Bytes32 value)
        => _transientStorage.Set(NextRevision(), in key, in value);

    public async ValueTask<UInt256> GetBalanceAsync()
    {
        if(_balance.TryGetValue(out var value))
        {
            return value;
        }

        value = await _globalStateProvider.GetBalanceAsync(_context, _address);
        _balance.Cache(in value);
        return value;
    }

    public void SetBalance(in UInt256 value)
        => _balance.Set(NextRevision(), in value);

    public async ValueTask<ulong> GetNonceAsync()
    {
        if(_nonce.TryGetValue(out ulong value))
        {
            return value;
        }
        value = await _globalStateProvider.GetNonceAsync(_context, _address);
        _nonce.Cache(in value);
        return value;
    }

    public void SetNonce(ulong value)
        => _nonce.Set(NextRevision(), in value);

    public async ValueTask<EVMByteCode> GetCodeAsync()
        => _code.TryGetValue(out var value)
            ? value.Code
            : (await GetAccountCodeAsync()).Code;

    public void SetCode(in EVMByteCode value)
    {
        var accountCode = new AccountCode(value, Keccak256.HashData(value.ByteCode.Span));
        _code.Set(NextRevision(), in accountCode);
    }

    public async ValueTask<Bytes32> GetCodeHashAsync()
        => _code.TryGetValue(out var value)
            ? value.Hash
            : (await GetAccountCodeAsync()).Hash;

    private async ValueTask<AccountCode> GetAccountCodeAsync()
    {
        if(_code.TryGetValue(out var value))
        {
            return value;
        }

        value = await _globalStateProvider.GetAccountCodeAsync(_context, _address);
        _code.Cache(in value);
        return value;
    }

    public void Commit()
    {
        _persistentStorage.Commit();
        _transientStorage.Clear();
        _balance.Commit();
        _nonce.Commit();
        _code.Commit();
        _revision = 0;
    }

    public void Reset(Snapshot snapshot)
    {
        if(snapshot.Revision < 0 || snapshot.Revision > _revision)
        {
            throw new ArgumentOutOfRangeException(nameof(snapshot));
        }

        _persistentStorage.Reset(snapshot.Revision);
        _transientStorage.Reset(snapshot.Revision);
        _balance.Reset(snapshot.Revision);
        _nonce.Reset(snapshot.Revision);
        _code.Reset(snapshot.Revision);
        _revision = snapshot.Revision;
    }

    private int NextRevision()
        => _revision = checked(_revision + 1);
}
