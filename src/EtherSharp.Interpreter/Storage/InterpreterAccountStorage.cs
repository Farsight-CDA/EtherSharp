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
    private readonly struct BaseAccount
    {
        public InterpreterAccountInfo Info { get; }
        public bool IsPresent { get; }

        public BaseAccount(InterpreterAccountInfo? account)
        {
            if(account is { CodeHash: var codeHash } && codeHash == Bytes32.Zero)
            {
                throw new InvalidOperationException("Existing accounts must have a canonical nonzero code hash.");
            }

            Info = account ?? InterpreterAccountInfo.Empty;
            IsPresent = account.HasValue;
        }
    }

    private readonly Address _address = address;
    private readonly InterpreterContext _context = context;
    private readonly IInterpreterHost _host = host;
    private readonly Func<long> _nextRevision = nextRevision;

    private BaseAccount? _baseAccount;

    private readonly JournaledMap<Bytes32, Bytes32> _persistentStorage = new();
    private readonly JournaledMap<Bytes32, Bytes32> _transientStorage = new();
    private readonly JournaledValue<UInt256> _balance = new();
    private readonly JournaledValue<ulong> _nonce = new();
    private readonly JournaledValue<EVMByteCode> _code = new();
    private readonly JournaledValue<Bytes32> _codeHash = new();
    private readonly JournaledFlag _present = new();
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
    {
        long revision = _nextRevision();
        _present.Set(revision);
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
            : (await GetBaseAccountAsync()).Info.Balance;

    public void SetBalance(in UInt256 value)
    {
        long revision = _nextRevision();
        _present.Set(revision);
        _balance.Set(revision, in value);
    }

    public async ValueTask<ulong> GetNonceAsync()
        => _nonce.TryGetValue(out ulong value)
            ? value
            : (await GetBaseAccountAsync()).Info.Nonce;

    public void SetNonce(ulong value)
    {
        long revision = _nextRevision();
        _present.Set(revision);
        _nonce.Set(revision, in value);
    }

    public async ValueTask<EVMByteCode> GetCodeAsync()
        => _code.TryGetValue(out var value)
            ? value
            : (await GetBaseAccountAsync()).Info.Code;

    public void SetCode(in EVMByteCode value)
    {
        long revision = _nextRevision();
        var codeHash = Keccak256.HashData(value.ByteCode.Span);
        _present.Set(revision);
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

    public async ValueTask<Bytes32> GetExtCodeHashAsync()
    {
        if(!_present.IsSet && !(await GetBaseAccountAsync()).IsPresent)
        {
            return Bytes32.Zero;
        }

        var codeHash = await GetStateCodeHashAsync();
        return codeHash == InterpreterAccountInfo.EmptyCodeHash
            && await GetNonceAsync() == 0
            && await GetBalanceAsync() == UInt256.Zero
                ? Bytes32.Zero
                : codeHash;
    }

    public async ValueTask<bool> HasCreateCollisionAsync()
        => await GetNonceAsync() != 0
            || await GetStateCodeHashAsync() != InterpreterAccountInfo.EmptyCodeHash;

    public void Commit()
    {
        _persistentStorage.Commit();
        _transientStorage.Clear();
        _balance.Commit();
        _nonce.Commit();
        _code.Commit();
        _codeHash.Commit();
        _present.Commit();
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
        _present.Reset(revision);
        _persistentStorageReplaced.Reset(revision);
    }

    private async ValueTask<Bytes32> GetStateCodeHashAsync()
        => _codeHash.TryGetValue(out var value)
            ? value
            : (await GetBaseAccountAsync()).Info.CodeHash;

    private async ValueTask<BaseAccount> GetBaseAccountAsync()
    {
        if(_baseAccount is not BaseAccount account)
        {
            account = new BaseAccount(await _host.GetAccountAsync(_context, _address));
            _baseAccount = account;
        }

        return account;
    }
}
