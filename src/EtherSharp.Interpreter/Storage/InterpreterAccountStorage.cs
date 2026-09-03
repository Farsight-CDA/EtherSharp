using EtherSharp.Contract;
using EtherSharp.Crypto;
using EtherSharp.Interpreter.Runtime;
using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Storage;

internal sealed class InterpreterAccountStorage(
    Address address,
    InterpreterContext context,
    IInterpreterHost host,
    Func<long> nextRevision
)
{
    private readonly Address _address = address;
    private readonly InterpreterContext _context = context;
    private readonly IInterpreterHost _host = host;
    private readonly Func<long> _nextRevision = nextRevision;

    private readonly JournaledMap<Bytes32, Bytes32> _persistentStorage = new();
    private readonly JournaledMap<Bytes32, Bytes32> _transientStorage = new();
    private readonly JournaledValue<UInt256> _balance = new();
    private readonly JournaledValue<ulong> _nonce = new();
    private readonly JournaledValue<EVMByteCode> _code = new();
    private readonly JournaledValue<Bytes32> _codeHash = new();
    private readonly JournaledFlag _persistentStorageReplaced = new();

    public async ValueTask<Bytes32> SLoadAsync(Bytes32 key)
    {
        if(_persistentStorage.TryGetValue(in key, out var value))
        {
            return value;
        }
        if(_persistentStorageReplaced.IsSet)
        {
            return Bytes32.Zero;
        }

        value = await _host.GetStorageAtAsync(_context, _address, key);
        _persistentStorage.Cache(in key, in value);
        return value;
    }

    public void SStore(in Bytes32 key, in Bytes32 value)
        => _persistentStorage.Set(_nextRevision(), in key, in value);

    public Bytes32 TLoad(in Bytes32 key)
        => _transientStorage.TryGetValue(in key, out var value)
            ? value
            : Bytes32.Zero;

    public void TStore(in Bytes32 key, in Bytes32 value)
        => _transientStorage.Set(_nextRevision(), in key, in value);

    public async ValueTask<UInt256> GetBalanceAsync()
    {
        if(_balance.TryGetValue(out var value))
        {
            return value;
        }

        value = await _host.GetBalanceAsync(_context, _address);
        _balance.Cache(in value);
        return value;
    }

    public void SetBalance(in UInt256 value)
        => _balance.Set(_nextRevision(), in value);

    public async ValueTask<ulong> GetNonceAsync()
    {
        if(_nonce.TryGetValue(out ulong value))
        {
            return value;
        }
        value = await _host.GetNonceAsync(_context, _address);
        _nonce.Cache(in value);
        return value;
    }

    public void SetNonce(ulong value)
        => _nonce.Set(_nextRevision(), in value);

    public async ValueTask<EVMByteCode> GetCodeAsync()
    {
        if(_code.TryGetValue(out var value))
        {
            return value;
        }

        value = await _host.GetCodeAsync(_context, _address);
        _code.Cache(in value);
        return value;
    }

    public void SetCode(in EVMByteCode value)
    {
        long revision = _nextRevision();
        var codeHash = Keccak256.HashData(value.ByteCode.Span);
        _code.Set(revision, in value);
        _codeHash.Set(revision, in codeHash);
    }

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

    public async ValueTask<Bytes32> GetCodeHashAsync()
    {
        if(_codeHash.TryGetValue(out var value))
        {
            return value;
        }

        value = await _host.GetCodeHashAsync(_context, _address);
        _codeHash.Cache(in value);
        return value;
    }

    public void Commit()
    {
        _persistentStorage.Commit();
        _transientStorage.Clear();
        _balance.Commit();
        _nonce.Commit();
        _code.Commit();
        _codeHash.Commit();
        _persistentStorageReplaced.Commit();
    }

    public void Reset(long revision)
    {
        _persistentStorage.Reset(revision);
        _transientStorage.Reset(revision);
        _balance.Reset(revision);
        _nonce.Reset(revision);
        _code.Reset(revision);
        _codeHash.Reset(revision);
        _persistentStorageReplaced.Reset(revision);
    }
}
